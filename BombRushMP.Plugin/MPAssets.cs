using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

namespace BombRushMP.Plugin
{
    public class MPAssets
    {
        public static MPAssets Instance { get; private set; }
        public AssetBundle Bundle;
        public TMP_SpriteAsset Sprites;
        public Emojis Emojis;
        public Material LODMaterial;
        public Material AimOutlineMaterial;
        public AudioClip[] DeathMusic;
        public GameObject XmasHideoutPrefab;
        public Texture2D XmasSky;

        public MPAssets(string rootPath)
        {
            Instance = this;
            Bundle = AssetBundle.LoadFromFile(Path.Combine(rootPath, "assets"));
            Sprites = Bundle.LoadAsset<TMP_SpriteAsset>("badges");
            LODMaterial = Bundle.LoadAsset<Material>("LODMaterial");
            Emojis = JsonConvert.DeserializeObject<Emojis>(File.ReadAllText(Path.Combine(rootPath, "emojimap.json")));
            DeathMusic = [Bundle.LoadAsset<AudioClip>("badfinger"), Bundle.LoadAsset<AudioClip>("drs")];
            AimOutlineMaterial = Bundle.LoadAsset<Material>("Aim Outline Material");
            var badgeMap = new BadgeMap(Sprites);
            badgeMap.ParseFromDirectory(Path.Combine(rootPath, "badgemap"), 32);
            if (MPUtility.IsChristmas())
            {
                XmasHideoutPrefab = Bundle.LoadAsset<GameObject>("XmasHideout");
                XmasSky = Bundle.LoadAsset<Texture2D>("NightSky");
            }
        }
    }
}
