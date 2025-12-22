#if PLUGIN
using BombRushMP.PluginCommon;
using Reptile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BombRushMP.Mono.Runtime
{
    public class MinecraftPlayer : MonoBehaviour
    {
        public static MinecraftPlayer Instance { get; private set; }
        public int CollectedObsidian = 0;
        public bool CollectedFlintAndSteel = false;

        private void Awake()
        {
            Instance = this;
        }

        private void OnDestroy()
        {
            Instance = null;
        }

        public void Kill()
        {
            SharedUtils.SetLocalCharacter(Characters.metalHead);
            Destroy(gameObject);
        }

        public static void Create()
        {
            if (Instance != null) return;
            var go = new GameObject("Minecraft Player");
            go.AddComponent<MinecraftPlayer>();
        }
    }
}
#endif