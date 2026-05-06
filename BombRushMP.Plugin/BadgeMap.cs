using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore;
using UnityEngine.TextCore.Text;

namespace BombRushMP.Plugin
{
    public class BadgeMap
    {
        private TMP_SpriteAsset _spriteAsset;

        public BadgeMap(TMP_SpriteAsset spriteAsset)
        {
            _spriteAsset = spriteAsset;
        }

        public void ParseFromDirectory(string path, int resolution, int padding)
        {
            var pngFiles = Directory.GetFiles(path, "*.png");

            var textures = new List<Texture2D>();
            var indices = new List<int>();

            foreach(var pngFile in pngFiles)
            {
                var tex = new Texture2D(2, 2);
                tex.LoadImage(File.ReadAllBytes(pngFile), false);
                var index = int.Parse(Path.GetFileNameWithoutExtension(pngFile));
                textures.Add(tex);
                indices.Add(index);
            }

            var atlas = new Texture2D(resolution, resolution);
            var rects = atlas.PackTextures(textures.ToArray(), padding);

            _spriteAsset.spriteSheet = atlas;

            _spriteAsset.spriteGlyphTable.Clear();
            _spriteAsset.spriteCharacterTable.Clear();

            for(var i = 0; i < textures.Count; i++)
            {
                var glyph = new TMP_SpriteGlyph()
                {
                    index = (uint)indices[i],
                    glyphRect = new GlyphRect(
                        (int)(rects[i].x * atlas.width),
                        (int)(rects[i].y * atlas.height),
                        (int)(rects[i].width * atlas.width),
                        (int)(rects[i].height * atlas.height)
                    ),
                    scale = 1.0f
                };

                var character = new TMP_SpriteCharacter((uint)indices[i], glyph);

                _spriteAsset.spriteGlyphTable.Add(glyph);
                _spriteAsset.spriteCharacterTable.Add(character);
                UnityEngine.Object.Destroy(textures[i]);
            }
        }
    }
}
