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
            RegisterUnlock(new MPSkateboardSkin("Goonie", "Beat a Freesoul Elite in any gamemode", Animator.StringToHash("goonieskateboard"), true, [
                GetMaterialFromAssets("GoonieEmerald"),
                GetMaterialFromAssets("GoonieMetal"),
                GetMaterialFromAssets("GoonieGold"),
                GetMaterialFromAssets("GoonieEmission"),
                GetMaterialFromAssets("GoonieGold2"),
                GetMaterialFromAssets("GoonieOutline")
            ], GetMeshFromAssets("GoonieMesh")));
            RegisterUnlock(new MPSkateboardSkin("GWMA", "hehe", Animator.StringToHash("gwmaskateboard"), true, GetTextureFromAssets("GWMASkateboardTex")));
            RegisterUnlock(new MPSkateboardSkin("BAKER", "get baked", Animator.StringToHash("bakerskateboard"), true, GetTextureFromAssets("BAKERSkateboardTex")));
            RegisterUnlock(new MPSkateboardSkin("Smash", "OR PASS!", Animator.StringToHash("smashskateboard"), true, GetTextureFromAssets("SmashSkateboardTex")));
            RegisterUnlock(new MPSkateboardSkin("Aqua", "hello i'm lazy duchess", Animator.StringToHash("aquaskateboard"), true, GetTextureFromAssets("AquaSkateboardTex")));
            RegisterUnlock(new MPSkateboardSkin("Rave", "yeaaa this one don't need no trucks", Animator.StringToHash("raveskateboard"), true, GetMaterialFromAssets("RaveboardMat"), GetMeshFromAssets("RaveboardMesh")));
            RegisterUnlock(new MPSkateboardSkin("Futurism", "how the fuck do you pop the tail", Animator.StringToHash("fismboard"), true, GetMaterialFromAssets("fismboardMat"), GetMeshFromAssets("fismboardMesh")));
            RegisterUnlock(new MPSkateboardSkin("Candyman", "i love candy... and men!", Animator.StringToHash("candymanboard"), true, GetTextureFromAssets("CandymanBoard")));
            RegisterUnlock(new MPSkateboardSkin("enjoi", "i enjoi cute cat deck", Animator.StringToHash("enjoiboard"), true, GetTextureFromAssets("EnjoiSkateboardTex")));
            RegisterUnlock(new MPSkateboardSkin("Cherry Blossom Bomber Barbara", "petal tree coooll", Animator.StringToHash("cbbbboard"), true, GetTextureFromAssets("cbbTex"), GetMeshFromAssets("cbbBoard")));
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
