using System.Collections.Generic;
using UnityEngine;

namespace BombRushMP.Plugin
{
    public class MPUnlockManager
    {
        public static MPUnlockManager Instance { get; private set; }
        public Dictionary<string, MPUnlockable> UnlockByID = new();
        public MPUnlockManager()
        {
            Instance = this;
            RegisterUnlock(new MPSkateboardSkin("Freesoul", "Win 1 Score Battle", "freesoulskateboard", true, GetTextureFromAssets("FreesoulSkateboardTex"), GetMeshFromAssets("FreesoulSkateboardMesh")));
        }

        private Texture2D GetTextureFromAssets(string name)
        {
            var mpAssets = MPAssets.Instance;
            return mpAssets.Bundle.LoadAsset<Texture2D>(name);
        }

        private Mesh GetMeshFromAssets(string name)
        {
            var mpAssets = MPAssets.Instance;
            return mpAssets.Bundle.LoadAsset<GameObject>(name).GetComponentInChildren<MeshFilter>().sharedMesh;
        }

        private void RegisterUnlock(MPUnlockable unlock)
        {
            UnlockByID[unlock.Identifier] = unlock;
        }
    }
}
