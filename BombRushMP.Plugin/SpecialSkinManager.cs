using BombRushMP.Mono;
using Reptile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using BombRushMP.Common;
using BombRushMP.PluginCommon;

namespace BombRushMP.Plugin
{
    public class SpecialSkinManager
    {
        public static SpecialSkinManager Instance { get; private set; }
        private Dictionary<SpecialSkins, string> _specialSkinPrefabs = new()
        {
            {SpecialSkins.FemaleCop, "PlayerFemaleCopPrefab" },
            {SpecialSkins.MaleCop, "PlayerMaleCopPrefab" },
            {SpecialSkins.SpecialPlayer, "SpecialPlayerPrefab" },
            {SpecialSkins.SeanKingston, "SeanKingstonPrefab" },
            {SpecialSkins.Forklift, "ForkliftPrefab" },
            {SpecialSkins.Jill, "JillPrefab" }
        };
        private Dictionary<SpecialSkins, GameObject> _specialSkinVisuals = new();
        private Dictionary<SpecialSkins, AudioLibrary> _specialSkinAudio = new();
        private Dictionary<SpecialSkins, SpecialSkinDefinition> _specialSkinDefinitions = new();
        private MPAssets _assets;

        public SpecialSkinManager()
        {
            Instance = this;
            _assets = MPAssets.Instance;
            foreach(var specialskin in _specialSkinPrefabs)
            {
                Construct(specialskin.Key, specialskin.Value);
            }
        }
        
        private void Construct(SpecialSkins skinEnum, string prefabName)
        {
            var prefab = _assets.Bundle.LoadAsset<GameObject>(prefabName);
            var definition = prefab.GetComponent<SpecialSkinDefinition>();
            var library = AudioLibraryUtils.CreateFromSpecialSkin(definition);
            _specialSkinAudio[skinEnum] = library;
            var prefabInstance = GameObject.Instantiate<GameObject>(prefab);
            var visual = new GameObject($"{skinEnum} Visual");
            prefabInstance.transform.SetParent(visual.transform, false);
            prefabInstance.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            GameObject.DontDestroyOnLoad(visual);
            visual.SetActive(false);
            _specialSkinVisuals[skinEnum] = visual;
            _specialSkinDefinitions[skinEnum] = definition;
        }

        public SpecialSkinDefinition GetDefinition(SpecialSkins skin)
        {
            if (_specialSkinDefinitions.ContainsKey(skin))
                return _specialSkinDefinitions[skin];
            return null;
        }

        public AudioLibrary GetAudioLibrary(SpecialSkins skin)
        {
            if (_specialSkinAudio.ContainsKey(skin))
                return _specialSkinAudio[skin];
            return null;
        }

        public void RemoveSpecialSkinFromPlayer(Player player)
        {
            var playerComponent = PlayerComponent.Get(player);
            if (playerComponent.SpecialSkin == SpecialSkins.None) return;
            playerComponent.UnloadStreamedCharacter();
            playerComponent.SpecialSkin = SpecialSkins.None;
            playerComponent.MainRenderer = null;
            playerComponent.SpecialSkinVariant = -1;
            var oldChar = player.character;
            player.character = Characters.NONE;
            player.SetCharacter(oldChar, Core.Instance.SaveManager.CurrentSaveSlot.GetCharacterProgress(oldChar).outfit);
            player.InitVisual();
            player.usingEquippedMovestyle = false;
            var clientController = ClientController.Instance;
            if (clientController != null && playerComponent.Local)
                clientController.SendClientState();
        }

        public void ApplyRandomVariantToPlayer(Player player)
        {
            var playerComponent = PlayerComponent.Get(player);
            if (playerComponent.SpecialSkin == SpecialSkins.None) return;
            if (playerComponent.MainRenderer == null) return;
            var definition = GetDefinition(playerComponent.SpecialSkin);
            if (definition.Variants.Length == 0) return;
            var variant = UnityEngine.Random.Range(0, definition.Variants.Length);
            ApplySpecialSkinVariantToPlayer(player, variant);
        }

        public void ApplySpecialSkinVariantToPlayer(Player player, int variant)
        {
            var playerComponent = PlayerComponent.Get(player);
            if (playerComponent.SpecialSkin == SpecialSkins.None) return;
            if (playerComponent.SpecialSkinVariant == variant) return;
            var definition = GetDefinition(playerComponent.SpecialSkin);
            var variantMaterial = definition.Variants[variant];
            playerComponent.MainRenderer.sharedMaterial = variantMaterial;
            playerComponent.SpecialSkinVariant = variant;
            var clientController = ClientController.Instance;
            if (clientController != null && playerComponent.Local)
                clientController.SendClientState();
        }

        public void ApplySpecialSkinToPlayer(Player player, SpecialSkins skin)
        {
            if (skin == SpecialSkins.None)
            {
                RemoveSpecialSkinFromPlayer(player);
                return;
            }
            player.character = Characters.metalHead;
            if (player.visualTf != null)
                GameObject.Destroy(player.visualTf.gameObject);
            var visual = GameObject.Instantiate<GameObject>(_specialSkinVisuals[skin]).AddComponent<CharacterVisual>();
            visual.Init(Characters.NONE, player.animatorController, true, player.motor.groundDetection.groundLimit);
            var definition = GetDefinition(skin);
            visual.canBlink = definition.CanBlink;
            visual.gameObject.SetActive(true);
            player.characterVisual = visual;
            player.characterMesh = player.characterVisual.mainRenderer.sharedMesh;
            player.characterVisual.transform.SetParent(player.transform.GetChild(0), false);
            player.characterVisual.transform.localPosition = Vector3.zero;
            player.characterVisual.transform.rotation = Quaternion.LookRotation(player.transform.forward);
            player.characterVisual.anim.gameObject.AddComponent<AnimationEventRelay>().Init();
            player.visualTf = player.characterVisual.transform;
            player.headTf = player.visualTf.FindRecursive("head");
            player.phoneDirBone = player.visualTf.FindRecursive("phoneDirection");
            player.heightToHead = (player.headTf.position - player.visualTf.position).y;
            player.isGirl = false;
            player.anim = player.characterVisual.anim;
            if (player.curAnim != 0)
            {
                var anim = player.curAnim;
                player.curAnim = 0;
                player.PlayAnim(anim, false, false, -1f);
            }
            player.characterVisual.InitVFX(player.VFXPrefabs);
            player.characterVisual.InitMoveStyleProps(player.MoveStylePropsPrefabs);
            player.characterConstructor.SetMoveStyleSkinsForCharacter(player, player.character);
            if (player.characterVisual.hasEffects)
            {
                player.boostpackTrail = player.characterVisual.VFX.boostpackTrail.GetComponent<TrailRenderer>();
                player.boostpackTrailDefaultWidth = player.boostpackTrail.startWidth;
                player.boostpackTrailDefaultTime = player.boostpackTrail.time;
                player.spraypaintParticles = player.characterVisual.VFX.spraypaint.GetComponent<ParticleSystem>();
                player.characterVisual.VFX.spraypaint.transform.localScale = Vector3.one * 0.5f;
                player.SetDustEmission(0);
                player.ringParticles = player.characterVisual.VFX.ring.GetComponent<ParticleSystem>();
                player.SetRingEmission(0);
            }
            var wasMovestyleEquipped = player.usingEquippedMovestyle;
            player.SetCurrentMoveStyleEquipped(player.moveStyleEquipped, true, true);
            player.InitVisual();
            PlayerComponent.Get(player).RefreshSkin();
            if (!wasMovestyleEquipped)
                player.SetMoveStyle(MoveStyle.ON_FOOT, true, true);
            if (!player.isAI)
            {
                var saveData = MPSaveData.Instance.GetCharacterData(player.character);
                if (saveData.MPMoveStyleSkin != -1)
                {
                    var mpUnlockManager = MPUnlockManager.Instance;
                    if (mpUnlockManager.UnlockByID.ContainsKey(saveData.MPMoveStyleSkin))
                    {
                        var mskin = mpUnlockManager.UnlockByID[saveData.MPMoveStyleSkin] as MPMoveStyleSkin;
                        if (mskin != null)
                            mskin.ApplyToPlayer(player);
                    }
                }
            }
            var playerComponent = PlayerComponent.Get(player);
            playerComponent.UnloadStreamedCharacter();
            playerComponent.SpecialSkin = skin;
            player.usingEquippedMovestyle = false;
            playerComponent.MainRenderer = null;
            playerComponent.SpecialSkinVariant = -1;
            if (!string.IsNullOrEmpty(definition.MainRendererName))
            {
                playerComponent.MainRenderer = player.visualTf.FindRecursive(definition.MainRendererName).GetComponent<SkinnedMeshRenderer>();
                ApplySpecialSkinVariantToPlayer(player, 0);
            }
            var clientController = ClientController.Instance;
            if (clientController != null && playerComponent.Local)
                clientController.SendClientState();
        }
    }
}
