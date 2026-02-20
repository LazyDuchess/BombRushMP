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
            RegisterUnlock(new MPSkateboardSkin("Freesoul", "By: Pupsi", Animator.StringToHash("freesoulskateboard"), true, GetTextureFromAssets("FreesoulSkateboardTex"), GetMeshFromAssets("FreesoulSkateboardMesh")));
            RegisterUnlock(new MPSkateboardSkin("Dreamcast", "By: Pupsi", Animator.StringToHash("dreamcastskateboard"), true, GetTextureFromAssets("DreamcastSkateboardTex")));
            RegisterUnlock(new MPSkateboardSkin("Knuckles", "By: Pupsi", Animator.StringToHash("knucklesskateboard"), true, GetTextureFromAssets("KnucklesSkateboardTex")));
            RegisterUnlock(new MPSkateboardSkin("Moe Shop", "By: Pupsi", Animator.StringToHash("moeshopskateboard"), true, GetTextureFromAssets("MoeShopSkateboardTex")));
            RegisterUnlock(new MPSkateboardSkin("Glassboard", "By: Woodztock", Animator.StringToHash("glassboardskateboard"), true, GetTextureFromAssets("cottoncandy"), GetMeshFromAssets("GlassboardMesh")));
            RegisterUnlock(new MPSkateboardSkin("Iced Board", "By: Stony", Animator.StringToHash("icedboardskateboard"), true, GetMaterialFromAssets("IcedBoardMat"), GetMeshFromAssets("IcedBoard")));
            RegisterUnlock(new MPSkateboardSkin("Crystal Board", "By: Woodztock", Animator.StringToHash("crystalboardskateboard"), true, GetMaterialFromAssets("CrystalBoardMat"), GetMeshFromAssets("CrystalBoardMesh")));
            RegisterUnlock(new MPSkateboardSkin("Goonie", "By: Pupsi", Animator.StringToHash("goonieskateboard"), true, [
                GetMaterialFromAssets("GoonieEmerald"),
                GetMaterialFromAssets("GoonieMetal"),
                GetMaterialFromAssets("GoonieGold"),
                GetMaterialFromAssets("GoonieEmission"),
                GetMaterialFromAssets("GoonieGold2"),
                GetMaterialFromAssets("GoonieOutline")
            ], GetMeshFromAssets("GoonieMesh")));
            RegisterUnlock(new MPSkateboardSkin("GWMA", "By: Yoshi", Animator.StringToHash("gwmaskateboard"), true, GetTextureFromAssets("GWMASkateboardTex")));
            RegisterUnlock(new MPSkateboardSkin("BAKER", "By: Fay", Animator.StringToHash("bakerskateboard"), true, GetTextureFromAssets("BAKERSkateboardTex")));
            RegisterUnlock(new MPSkateboardSkin("Smash", "By: Fay", Animator.StringToHash("smashskateboard"), true, GetTextureFromAssets("SmashSkateboardTex")));
            RegisterUnlock(new MPSkateboardSkin("Aqua", "By: Fay", Animator.StringToHash("aquaskateboard"), true, GetTextureFromAssets("AquaSkateboardTex")));
            RegisterUnlock(new MPSkateboardSkin("Rave", "By: Undad", Animator.StringToHash("raveskateboard"), true, GetMaterialFromAssets("RaveboardMat"), GetMeshFromAssets("RaveboardMesh")));
            RegisterUnlock(new MPSkateboardSkin("Futurism", "By: Kris The Fox and Stony", Animator.StringToHash("fismboard"), true, GetMaterialFromAssets("fismboardMat"), GetMeshFromAssets("fismboardMesh")));
            RegisterUnlock(new MPSkateboardSkin("Candyman", "By: Hellfiend", Animator.StringToHash("candymanboard"), true, GetTextureFromAssets("CandymanBoard")));
            RegisterUnlock(new MPSkateboardSkin("enjoi", "cute cats", Animator.StringToHash("enjoiboard"), true, GetTextureFromAssets("EnjoiSkateboardTex")));
            RegisterUnlock(new MPSkateboardSkin("Cherry Blossom Bomber Barbara", "By: LadyAnn45 and Woodztock", Animator.StringToHash("cbbbboard"), true, GetTextureFromAssets("cbbTex"), GetMeshFromAssets("cbbBoard")));
            RegisterUnlock(new MPSkateboardSkin("Slice of Life", "By: Kogey", Animator.StringToHash("pizzadeck"), true, GetTextureFromAssets("pizzadeck"), GetMeshFromAssets("pizza")));
            RegisterUnlock(new MPSkateboardSkin("Goonie (Real)", "By: Keziah_L", Animator.StringToHash("realgoonie"), true, GetTextureFromAssets("goonietex"), GetMeshFromAssets("realgoonieboard")));
            RegisterUnlock(new MPSkateboardSkin("Goonie (Chibi)", "By: Keziah_L", Animator.StringToHash("gooniechibi"), true, GetTextureFromAssets("goonietex"), GetMeshFromAssets("gooniechibi")));
            RegisterUnlock(new MPSkateboardSkin("Thunder Kick", "By: Thunder Kick", Animator.StringToHash("thunderkickdeck"), true, GetTextureFromAssets("thunderkickdeck")));
            RegisterUnlock(new MPSkateboardSkin("Junkyard Dog", "By: Yurok and Keziah_L", Animator.StringToHash("yurokBoard"), true, GetTextureFromAssets("yurokBoardTex"), GetMeshFromAssets("yurokBoard")));
            RegisterUnlock(new MPSkateboardSkin("Shadow SA2", "By: Lance", Animator.StringToHash("shadowsa2"), true, GetMaterialFromAssets("ShadowSA2Mat")));
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
