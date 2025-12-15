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
                DoHideoutAwake();
        }

        private void Start()
        {
            if (gameObject.scene.name == "hideout")
                DoHideoutStart();
        }

        private void DoHideoutStart()
        {
            LightmapSettings.lightmaps = new LightmapData[] { };
            QualitySettings.shadowDistance = 150f;
            RenderSettings.skybox.mainTexture = _mpAssets.XmasSky;
        }

        private void DoHideoutAwake()
        {
            var hideout = Instantiate(_mpAssets.XmasHideoutPrefab);
            var chunkParent = hideout.transform.Find("Chunk");
            if (chunkParent == null) return;
            var stageChunks = FindObjectsOfType<StageChunk>();
            StageChunk stageChunk = null;
            foreach (var chunk in stageChunks)
            {
                if (chunk.name == "Hideout_Props")
                {
                    stageChunk = chunk;
                    break;
                }
            }
            if (stageChunk == null) return;
            chunkParent.transform.SetParent(stageChunk.transform, true);
        }

        public static void Create()
        {
            var go = new GameObject("Xmas Controller");
            go.AddComponent<XmasController>();
        }
    }
}
