using BombRushMP.Common;
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

        public void ParseFromDirectory(string path, int padding)
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

            var atlas = new Texture2D(2, 2);
            var rects = atlas.PackTextures(textures.ToArray(), padding);
            atlas.filterMode = MPSettings.Instance.SmoothSprites ? FilterMode.Bilinear : FilterMode.Point;
            atlas.wrapMode = TextureWrapMode.Clamp;

            _spriteAsset.spriteSheet = atlas;

            var max = indices.Max() + 1;

            var glyphTable = new TMP_SpriteGlyph[max];
            var charTable = new TMP_SpriteCharacter[max];

            _spriteAsset.spriteGlyphTable.Clear();
            _spriteAsset.spriteCharacterTable.Clear();
            _spriteAsset.spriteGlyphTable.AddRange(glyphTable);
            _spriteAsset.spriteCharacterTable.AddRange(charTable);

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
                    metrics = new GlyphMetrics(rects[i].width * atlas.width, rects[i].height * atlas.height, 0, (rects[i].height * atlas.height) * MPSettings.Instance.SpriteBaseline, rects[i].width * atlas.width),
                    scale = 1.2f
                };

                var character = new TMP_SpriteCharacter(TMPFilter.PrivateUseAreaBegin + (uint)indices[i], glyph);

                _spriteAsset.spriteGlyphTable[indices[i]] = glyph;
                _spriteAsset.spriteCharacterTable[indices[i]] = character;
                UnityEngine.Object.Destroy(textures[i]);
            }

            _spriteAsset.UpdateLookupTables();
            _spriteAsset.material.mainTexture = atlas;
        }
    }
}
