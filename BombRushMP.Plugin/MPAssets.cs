using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BombRushMP.Plugin
{
    public class MPAssets
    {
        public static MPAssets Instance { get; private set; }
        public AssetBundle Bundle;

        public MPAssets(string path)
        {
            Instance = this;
            Bundle = AssetBundle.LoadFromFile(path);
        }
    }
}
