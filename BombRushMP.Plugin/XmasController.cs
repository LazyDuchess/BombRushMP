using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reptile;
using UnityEngine;

namespace BombRushMP.Plugin
{
    public class XmasController : MonoBehaviour
    {
        public static XmasController Instance { get; private set; }
        private MPAssets _mpAssets;
        private void Awake()
        {
            Instance = this;
            _mpAssets = MPAssets.Instance;
            if (gameObject.scene.name == "hideout")
                DoHideoutChanges();
        }

        private void DoHideoutChanges()
        {
            LightmapSettings.lightmaps = new LightmapData[] { };
            QualitySettings.shadowDistance = 150f;
            RenderSettings.skybox.mainTexture = _mpAssets.XmasSky;
            Instantiate(_mpAssets.XmasHideoutPrefab);
        }

        public static void Create()
        {
            var go = new GameObject("Xmas Controller");
            go.AddComponent<XmasController>();
        }
    }
}
