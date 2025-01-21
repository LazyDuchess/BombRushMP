using Reptile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using BombRushMP.Common;
using MonoMod.Cil;
using UnityEngine.UIElements;
using ch.sycoforge.Decal.Projectors.Geometry;
using System.Drawing.Text;
using HarmonyLib;

namespace BombRushMP.Plugin
{
    public class PlayerComponent : MonoBehaviour
    {
        public bool LOD = false;
        private const float HelloCooldown = 2f;
        private const float AFKEnterSpeed = 4f;
        private const float AFKExitSpeed = 8f;
        public SpecialSkins SpecialSkin = SpecialSkins.None;
        public int SpecialSkinVariant = -1;
        public SkinnedMeshRenderer MainRenderer = null;
        public bool AFK = false;
        public bool Local => _player == WorldHandler.instance.GetCurrentPlayer();
        private ParticleSystem _afkParticles = null;
        private Player _player = null;
        private float _afkWeight = 0f;
        private float _afkTimer = 0f;
        private bool _afkForced = false;
        private float _helloTimer = 0f;
        public Material SkateboardMaterial = null;
        public Material InlineMaterial = null;
        public Material BMXMaterial = null;
        public Material BMXSpokesMaterial = null;
        public MPMoveStyleSkin MovestyleSkin = null;
        public bool Chibi = false;

        public void RefreshSkin()
        {
            if (MovestyleSkin != null)
                MovestyleSkin.ApplyToPlayer(_player);
        }

        private void OnDestroy()
        {
            if (SkateboardMaterial != null)
                Destroy(SkateboardMaterial);
            if (BMXMaterial != null)
                Destroy(BMXMaterial);
            if (InlineMaterial != null)
                Destroy(InlineMaterial);
            if (BMXSpokesMaterial != null)
                Destroy(BMXSpokesMaterial);
        }

        public void ApplyMoveStyleMaterial(Material[] customMaterials)
        {
            if (SkateboardMaterial == null)
                throw CreateException("SkateboardMaterial");
            if (InlineMaterial == null)
                throw CreateException("InlineMaterial");
            if (BMXMaterial == null)
                throw CreateException("BMXMaterial");
            if (BMXSpokesMaterial == null)
                throw CreateException("BMXSpokesMaterial");

            var moveStyleProps = _player.characterVisual.moveStyleProps;

            var allRenderers = moveStyleProps.bmxPedalR.GetComponentsInChildren<MeshRenderer>().ToList();
            allRenderers.AddRange(moveStyleProps.bmxPedalL.GetComponentsInChildren<MeshRenderer>());
            allRenderers.AddRange(moveStyleProps.bmxFrame.GetComponentsInChildren<MeshRenderer>());
            allRenderers.AddRange(moveStyleProps.bmxGear.GetComponentsInChildren<MeshRenderer>());
            allRenderers.AddRange(moveStyleProps.bmxHandlebars.GetComponentsInChildren<MeshRenderer>());
            allRenderers.AddRange(moveStyleProps.bmxWheelF.GetComponentsInChildren<MeshRenderer>());
            allRenderers.AddRange(moveStyleProps.bmxWheelR.GetComponentsInChildren<MeshRenderer>());

            foreach (var renderer in allRenderers)
            {
                renderer.sharedMaterials = customMaterials;
            }

            allRenderers = moveStyleProps.skateL.GetComponentsInChildren<MeshRenderer>().ToList();
            allRenderers.AddRange(moveStyleProps.skateR.GetComponentsInChildren<MeshRenderer>());

            foreach (var renderer in allRenderers)
            {
                renderer.sharedMaterials = customMaterials;
            }

            allRenderers = moveStyleProps.skateboard.GetComponentsInChildren<MeshRenderer>().ToList();

            foreach (var renderer in allRenderers)
            {
                renderer.sharedMaterials = customMaterials;
            }

            Exception CreateException(string variableName)
            {
                return new Exception($"Tried to apply a custom MoveStyle material before caching {variableName} in PlayerComponent. Call ApplyMoveStyleSkin first!");
            }
        }

        public void ApplyMoveStyleSkin(int skinIndex)
        {
            var moveStyleProps = _player.characterVisual.moveStyleProps;
            var deckMesh = _player.MoveStylePropsPrefabs.skateboard.GetComponent<MeshFilter>().sharedMesh;
            moveStyleProps.skateboard.GetComponent<MeshFilter>().sharedMesh = deckMesh;
            if (SkateboardMaterial == null)
            {
                SkateboardMaterial = new Material(moveStyleProps.skateboard.GetComponentInChildren<MeshRenderer>().sharedMaterial);
            }
            if (InlineMaterial == null)
            {
                InlineMaterial = new Material(moveStyleProps.skateL.GetComponentInChildren<MeshRenderer>().sharedMaterial);
            }
            if (BMXMaterial == null)
            {
                BMXMaterial = new Material(moveStyleProps.bmxFrame.GetComponentInChildren<MeshRenderer>().sharedMaterial);
            }
            if (BMXSpokesMaterial == null)
            {
                BMXSpokesMaterial = new Material(moveStyleProps.bmxWheelF.GetComponentInChildren<MeshRenderer>().sharedMaterials[1]);
            }

            var allRenderers = moveStyleProps.bmxPedalR.GetComponentsInChildren<MeshRenderer>().ToList();
            allRenderers.AddRange(moveStyleProps.bmxPedalL.GetComponentsInChildren<MeshRenderer>());
            allRenderers.AddRange(moveStyleProps.bmxFrame.GetComponentsInChildren<MeshRenderer>());
            allRenderers.AddRange(moveStyleProps.bmxGear.GetComponentsInChildren<MeshRenderer>());
            allRenderers.AddRange(moveStyleProps.bmxHandlebars.GetComponentsInChildren<MeshRenderer>());
            allRenderers.AddRange(moveStyleProps.bmxWheelF.GetComponentsInChildren<MeshRenderer>());
            allRenderers.AddRange(moveStyleProps.bmxWheelR.GetComponentsInChildren<MeshRenderer>());

            foreach (var renderer in allRenderers)
            {
                renderer.sharedMaterials = [BMXMaterial, BMXSpokesMaterial];
            }

            allRenderers = moveStyleProps.skateL.GetComponentsInChildren<MeshRenderer>().ToList();
            allRenderers.AddRange(moveStyleProps.skateR.GetComponentsInChildren<MeshRenderer>());

            foreach (var renderer in allRenderers)
            {
                renderer.sharedMaterials = [InlineMaterial];
            }

            allRenderers = moveStyleProps.skateboard.GetComponentsInChildren<MeshRenderer>().ToList();

            foreach (var renderer in allRenderers)
            {
                renderer.sharedMaterials = [SkateboardMaterial];
            }

            for (var i=MoveStyle.BMX; i<=MoveStyle.INLINE; i++)
            {
                var moveStyleSkinTexture = _player.CharacterConstructor.GetMoveStyleSkinTexture(i, skinIndex);
                if (moveStyleSkinTexture == null) continue;
                var mats = MoveStyleLoader.GetMoveStyleMaterials(_player, i);
                if (mats == null) continue;
                foreach (var mat in mats)
                    mat.mainTexture = moveStyleSkinTexture;
            }
        }

        public bool CanSayHello()
        {
            if (_helloTimer > 0f) return false;
            if (_player.IsBusyWithSequence()) return false;
            if (_player.IsDead()) return false;
            return true;
        }

        public void DoSayHello()
        {
            var saveData = MPSaveData.Instance;
            saveData.Stats.TimesSaidHello++;
            Core.Instance.SaveManager.SaveCurrentSaveSlot();
            _player.PlayVoice(AudioClipID.VoiceTalk, VoicePriority.COMBAT);
        }

        private void Awake()
        {
            _player = GetComponent<Player>();
            var afkParticlesPrefab = MPAssets.Instance.Bundle.LoadAsset<GameObject>("AFK Particles");
            _afkParticles = Instantiate(afkParticlesPrefab).GetComponent<ParticleSystem>();
            _afkParticles.transform.SetParent(transform);
        }

        public void StopAFK()
        {
            _afkTimer = 0f;
            AFK = false;
            _afkForced = false;
        }

        public void ForceAFK()
        {
            if (!AFK)
            {
                MPSaveData.Instance.Stats.TimesFallenAsleep++;
                Core.Instance.SaveManager.SaveCurrentSaveSlot();
            }
            _afkTimer = ClientConstants.AFKTime;
            AFK = true;
            _afkForced = true;
        }

        public void UpdateChibi()
        {
            if (Chibi)
            {
                var vis = _player.characterVisual;
                vis.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                vis.head.localScale = new Vector3(2f, 2f, 2f);
                vis.handR.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                vis.handL.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                vis.footR.localScale = new Vector3(1.25f, 1.25f, 1.25f);
                vis.footL.localScale = new Vector3(1.25f, 1.25f, 1.25f);
                _player.playerGameplayVoicesAudioSource.pitch = 1.5f;
            }
            else
            {
                _player.playerGameplayVoicesAudioSource.pitch = 1f;
            }
        }

        private void LateUpdate()
        {
            UpdateChibi();
            var mpSettings = MPSettings.Instance;
            if (Local)
            {
                _helloTimer -= Core.dt;
                if (_helloTimer <= 0f)
                    _helloTimer = 0f;

                var gameInput = Core.Instance.GameInput;
                var controllerMaps = gameInput.GetAllCurrentEnabledControllerMapCategoryIDs();
                if (controllerMaps.controllerMapCategoryIDs.Contains(0))
                {
                    if (Input.GetKeyDown(MPSettings.Instance.TalkKey))
                    {
                        if (CanSayHello())
                        {
                            DoSayHello();
                            _helloTimer = HelloCooldown;
                        }
                    }
                }

                if (!_afkForced)
                {
                    var axisArray = new float[] { gameInput.GetAxis(13), gameInput.GetAxis(14), gameInput.GetAxis(8), gameInput.GetAxis(18) };
                    foreach (var axis in axisArray)
                    {
                        if (axis != 0f)
                            StopAFK();
                    }
                    if (gameInput.GetAnyButtonNew())
                    {
                        StopAFK();
                    }
                }
                _afkTimer += Core.dt;

                if (_afkTimer >= ClientConstants.AFKTime)
                {
                    if (!AFK)
                    {
                        MPSaveData.Instance.Stats.TimesFallenAsleep++;
                        Core.Instance.SaveManager.SaveCurrentSaveSlot();
                    }
                    AFK = true;
                    _afkTimer = ClientConstants.AFKTime;
                }
                else
                {
                    AFK = false;
                    _afkForced = false;
                }

                if (AFK)
                    MPSaveData.Instance.Stats.TimeSpentAsleep += Core.dt;
            }
            if (mpSettings.ShowAFKEffects)
            {
                if (AFK)
                {
                    _afkWeight += AFKEnterSpeed * Core.dt;
                    if (_afkWeight >= 1f)
                        _afkWeight = 1f;
                }
                else
                {
                    _afkWeight -= AFKExitSpeed * Core.dt;
                    if (_afkWeight <= 0f)
                        _afkWeight = 0f;
                }
            }
            else
                _afkWeight = 0f;
            var charVisual = _player.characterVisual;
            if (_afkWeight >= 1f)
            {
                var emission = _afkParticles.emission;
                emission.enabled = true;
                if (charVisual != null && charVisual.canBlink)
                {
                    charVisual.mainRenderer.SetBlendShapeWeight(0, 100f);
                }
            }
            else
            {
                var emission = _afkParticles.emission;
                emission.enabled = false;
            }
            if (charVisual != null)
            {
                _afkParticles.transform.position = charVisual.head.transform.position;
                _afkParticles.transform.rotation = Quaternion.LookRotation(_player.transform.forward);

                charVisual.head.transform.rotation *= Quaternion.Euler(charVisual.head.transform.right * -45f * _afkWeight);
            }
        }

        public static PlayerComponent Get(Player player)
        {
            if (player == null) return null;
            return player.GetComponent<PlayerComponent>();
        }

        public static PlayerComponent GetLocal()
        {
            return ClientController.Instance.LocalPlayerComponent;
        }

        public void MakeLOD()
        {
            var lodMat = MPAssets.Instance.LODMaterial;
            var renderers = _player.characterVisual.GetComponentsInChildren<Renderer>(true);
            var mainTexId = Shader.PropertyToID("_MainTex");
            _player.anim.cullingMode = AnimatorCullingMode.CullCompletely;
            foreach(var renderer in renderers)
            {
                var transp = true;
                if (renderer.sharedMaterials != null && renderer.sharedMaterials.Length > 0)
                {
                    var matAmount = renderer.sharedMaterials.Length;
                    var newMats = new Material[matAmount];
                    for (var i = 0; i < matAmount; i++)
                    {
                        newMats[i] = lodMat;
                        if (renderer.sharedMaterials[i] != null)
                        {
                            if (renderer.sharedMaterials[i].mainTexture != null)
                            {
                                if (renderer.sharedMaterials[i].renderQueue < 3000)
                                    transp = false;
                                var propBlock = new MaterialPropertyBlock();
                                propBlock.SetTexture(mainTexId, renderer.sharedMaterials[i].mainTexture);
                                renderer.SetPropertyBlock(propBlock, i);
                            }
                        }
                    }
                    if (transp && renderer != _player.characterVisual.mainRenderer)
                    {
                        renderer.forceRenderingOff = true;
                        renderer.enabled = false;
                    }
                    renderer.sharedMaterials = newMats;
                }
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                renderer.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
                var skinned = (renderer as SkinnedMeshRenderer);
                if (skinned != null)
                    skinned.quality = SkinQuality.Bone1;

                var stuffToRemove = _player.characterVisual.GetComponentsInChildren(typeof(DynamicBone), true);
                stuffToRemove = stuffToRemove.AddRangeToArray(_player.characterVisual.GetComponentsInChildren(typeof(DynamicBoneCollider), true));
                
                foreach(var thingToRemove in stuffToRemove)
                {
                    Destroy(thingToRemove);
                }

                var ik = _player.anim.GetComponent<InverseKinematicsRelay>();
                if (ik != null)
                    Destroy(ik);
            }
        }
    }
}
