using System.Collections.Generic;
using UnityEngine;

namespace BombRushMP.Plugin
{
    public class MPUnlockManager
    {
        public static MPUnlockManager Instance { get; private set; }
        public Dictionary<int, MPUnlockable> UnlockByID = new();
        public MPUnlockManager()
        {
            Instance = this;
            RegisterUnlock(new MPSkateboardSkin("Freesoul", "Win 1 Score Battle", Animator.StringToHash("freesoulskateboard"), true, GetTextureFromAssets("FreesoulSkateboardTex"), GetMeshFromAssets("FreesoulSkateboardMesh")));
            RegisterUnlock(new MPSkateboardSkin("Dreamcast", "Goon into the bollards at Versum Hill", Animator.StringToHash("dreamcastskateboard"), true, GetTextureFromAssets("DreamcastSkateboardTex")));
            RegisterUnlock(new MPSkateboardSkin("Knuckles", "Win 1 Graffiti Race", Animator.StringToHash("knucklesskateboard"), true, GetTextureFromAssets("KnucklesSkateboardTex")));
            RegisterUnlock(new MPSkateboardSkin("Moe Shop", "Win 1 Pro Skater Score Battle", Animator.StringToHash("moeshopskateboard"), true, GetTextureFromAssets("MoeShopSkateboardTex")));
            RegisterUnlock(new MPSkateboardSkin("Glassboard", "Win 1 Score Battle against 5 players or more", Animator.StringToHash("glassboardskateboard"), true, GetTextureFromAssets("DreamcastSkateboardTex"), GetMeshFromAssets("GlassboardMesh")));
            RegisterUnlock(new MPSkateboardSkin("Iced Board", "Kiss me on the lips", Animator.StringToHash("icedboardskateboard"), true, GetMaterialFromAssets("IcedBoardMat"), GetMeshFromAssets("IcedBoard")));
            RegisterUnlock(new MPSkateboardSkin("Crystal Board", "Smoke 12 blunts in a row without bathroom breaks", Animator.StringToHash("crystalboardskateboard"), true, GetMaterialFromAssets("CrystalBoardMat"), GetMeshFromAssets("CrystalBoardMesh")));
        }

        private Material GetMaterialFromAssets(string name)
        {
            var mpAssets = MPAssets.Instance;
            return mpAssets.Bundle.LoadAsset<Material>(name);
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
