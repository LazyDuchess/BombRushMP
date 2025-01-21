﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace BombRushMP.Plugin
{
    public class MPAssets
    {
        public static MPAssets Instance { get; private set; }
        public AssetBundle Bundle;
        public TMP_SpriteAsset Sprites;
        public Emojis Emojis;
        public Material LODMaterial;

        public MPAssets(string path)
        {
            Instance = this;
            Bundle = AssetBundle.LoadFromFile(path);
            Sprites = Bundle.LoadAsset<TMP_SpriteAsset>("badges");
            LODMaterial = Bundle.LoadAsset<Material>("LODMaterial");
            Emojis = new Emojis();
        }
    }
}
