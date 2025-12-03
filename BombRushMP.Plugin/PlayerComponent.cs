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
using BombRushMP.CrewBoom;
using BombRushMP.Plugin.Gamemodes;

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
        public StreamedCharacterInstance StreamedCharacter = null;
        public Action SkinLoaded;

        public bool ChallengeIndicatorVisible = false;
        private GameObject _challengeIndicator;

        public bool HasPropDisguise => DisguiseGameObject != null;
        public GameObject DisguiseGameObject = null;
        public int DisguiseID = 0;

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
            UnloadStreamedCharacter();
        }
        
        public void RemovePropDisguise()
        {
            if (!HasPropDisguise) return;
            Destroy(DisguiseGameObject);
            DisguiseGameObject = null;
            _player.visualTf.gameObject.SetActive(true);
        }

        public void ApplyPropDisguise(int disguiseId)
        {
            var disguiseController = PropDisguiseController.Instance;
            if (!disguiseController.Props.TryGetValue(disguiseId, out var prop))
                return;
            DisguiseID = disguiseId;
            if (HasPropDisguise)
            {
                Destroy(DisguiseGameObject);
            }
            _player.visualTf.gameObject.SetActive(false);
            var disguise = Instantiate(prop);
            DisguiseGameObject = disguise;
            var isJunk = false;
            var junk = disguise.GetComponent<Junk>();
            if (junk != null)
            {
                Destroy(junk);
                isJunk = true;
            }
            var rb = disguise.GetComponent<Rigidbody>();
            if (rb != null)
                Destroy(rb);
            var ped = disguise.GetComponent<StreetLife>();
            if (ped != null)
            {
                var anim = disguise.GetComponent<Animator>();
                if (anim != null)
                {
                    var animHash = Animator.StringToHash(ped.idleAnimation.ToString());
                    anim.Play(animHash, 0);
                }
                Destroy(ped);
            }
            disguise.transform.SetParent(_player.transform);
            disguise.transform.SetLocalPositionAndRotation(Vector2.zero, Quaternion.identity);
            if (isJunk)
                disguise.transform.rotation = prop.transform.rotation;
            if (Local)
            {
                disguise.gameObject.layer = Layers.Player;
                var colliders = disguise.GetComponents<Collider>();
                foreach(var collider in colliders)
                {
                    Destroy(collider);
                }
            }
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
            var indicatorPrefab = MPAssets.Instance.Bundle.LoadAsset<GameObject>("Challenge Indicator");
            _challengeIndicator = Instantiate(indicatorPrefab);
            _challengeIndicator.transform.SetParent(transform);
            _challengeIndicator.transform.SetLocalPositionAndRotation(Vector3.up * 2.5f, Quaternion.identity);
            _challengeIndicator.SetActive(false);
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
            if (!Local && ChallengeIndicatorVisible && _player.characterVisual.gameObject.activeSelf)
            {
                if (!_challengeIndicator.gameObject.activeSelf)
                    _challengeIndicator.SetActive(true);
                _challengeIndicator.transform.Rotate(Vector3.up * 150f * Core.dt, Space.Self);
                var indicatorHeight = Vector3.up * 2.5f + (Mathf.Sin(Time.realtimeSinceStartup * 5f) * Vector3.up * 0.1f);
                _challengeIndicator.transform.localPosition = indicatorHeight;
            }
            else
            {
                if (_challengeIndicator.gameObject.activeSelf)
                    _challengeIndicator.SetActive(false);
            }
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

        public CharacterHandle StreamedCharacterHandle = null;
        public int StreamedOutfit;

        public bool SetStreamedCharacter(Guid guid, int outfit)
        {
            if (StreamedCharacterHandle != null)
                StreamedCharacterHandle.Release();
            StreamedCharacterHandle = CrewBoomStreamer.RequestCharacter(guid, MPSettings.Instance.LoadCharactersAsync);
            StreamedOutfit = outfit;
            if (StreamedCharacterHandle == null) return false;
            if (StreamedCharacterHandle.Finished)
            {
                OnSkinLoadFinished();
            }
            else
            {
                StreamedCharacterHandle.OnLoadFinished += OnSkinLoadFinished;
            }
            return true;
        }

        private void UnloadShownStreamedCharacter()
        {
            if (StreamedCharacter != null)
            {
                StreamedCharacter.Handle.Release();
                StreamedCharacter = null;
            }
        }

        public void UnloadStreamedCharacter()
        {
            if (StreamedCharacterHandle != null)
            {
                StreamedCharacterHandle.OnLoadFinished -= OnSkinLoadFinished;
                StreamedCharacterHandle.Release();
                StreamedCharacterHandle = null;
            }
            if (StreamedCharacter != null)
            {
                UnloadShownStreamedCharacter();
                StreamedCharacter = null;
            }
        }

        public void ApplyStreamedCharacter()
        {
            _player.character = Characters.metalHead;
            if (_player.visualTf != null)
                GameObject.Destroy(_player.visualTf.gameObject);
            var visual = StreamedCharacter.Handle.ConstructVisual();
            visual.Init(Characters.NONE, _player.animatorController, true, _player.motor.groundDetection.groundLimit);
            _player.characterVisual = visual;
            _player.characterMesh = _player.characterVisual.mainRenderer.sharedMesh;
            _player.characterVisual.transform.SetParent(_player.transform.GetChild(0), false);
            _player.characterVisual.transform.localPosition = Vector3.zero;
            _player.characterVisual.transform.rotation = Quaternion.LookRotation(_player.transform.forward);
            _player.characterVisual.anim.gameObject.AddComponent<AnimationEventRelay>().Init();
            _player.visualTf = _player.characterVisual.transform;
            _player.headTf = _player.visualTf.FindRecursive("head");
            _player.phoneDirBone = _player.visualTf.FindRecursive("phoneDirection");
            _player.heightToHead = (_player.headTf.position - _player.visualTf.position).y;
            _player.anim = _player.characterVisual.anim;
            if (_player.curAnim != 0)
            {
                var anim = _player.curAnim;
                _player.curAnim = 0;
                _player.PlayAnim(anim, false, false, -1f);
            }
            _player.characterVisual.InitVFX(_player.VFXPrefabs);
            _player.characterVisual.InitMoveStyleProps(_player.MoveStylePropsPrefabs);
            _player.characterConstructor.SetMoveStyleSkinsForCharacter(_player, _player.character);
            if (_player.characterVisual.hasEffects)
            {
                _player.boostpackTrail = _player.characterVisual.VFX.boostpackTrail.GetComponent<TrailRenderer>();
                _player.boostpackTrailDefaultWidth = _player.boostpackTrail.startWidth;
                _player.boostpackTrailDefaultTime = _player.boostpackTrail.time;
                _player.spraypaintParticles = _player.characterVisual.VFX.spraypaint.GetComponent<ParticleSystem>();
                _player.characterVisual.VFX.spraypaint.transform.localScale = Vector3.one * 0.5f;
                _player.SetDustEmission(0);
                _player.ringParticles = _player.characterVisual.VFX.ring.GetComponent<ParticleSystem>();
                _player.SetRingEmission(0);
            }
            var wasMovestyleEquipped = _player.usingEquippedMovestyle;
            _player.SetCurrentMoveStyleEquipped(_player.moveStyleEquipped, true, true);
            _player.InitVisual();
            RefreshSkin();
            if (!wasMovestyleEquipped)
                _player.SetMoveStyle(MoveStyle.ON_FOOT, true, true);
            if (!_player.isAI)
            {
                var saveData = MPSaveData.Instance.GetCharacterData(_player.character);
                if (saveData.MPMoveStyleSkin != -1)
                {
                    var mpUnlockManager = MPUnlockManager.Instance;
                    if (mpUnlockManager.UnlockByID.ContainsKey(saveData.MPMoveStyleSkin))
                    {
                        var mskin = mpUnlockManager.UnlockByID[saveData.MPMoveStyleSkin] as MPMoveStyleSkin;
                        if (mskin != null)
                            mskin.ApplyToPlayer(_player);
                    }
                }
            }
            _player.usingEquippedMovestyle = false;
            MainRenderer = null;
            SpecialSkinVariant = -1;
            SpecialSkin = SpecialSkins.None;
            UpdateSkateOffsets();
            StreamedCharacter.SetVisual(visual);
            StreamedCharacter.ApplyOutfit(StreamedCharacter.Outfit);
        }

        public void UpdateSkateOffsets()
        {
            var inlineOffsetL = _player.characterVisual.footL.Find(CrewBoomSupport.SKATE_OFFSET_L);
            var inlineOffsetR = _player.characterVisual.footR.Find(CrewBoomSupport.SKATE_OFFSET_R);

            if (inlineOffsetL != null && inlineOffsetR != null)
            {
                _player.characterVisual.moveStyleProps.skateL.transform.SetLocalPositionAndRotation(inlineOffsetL.localPosition, inlineOffsetL.localRotation);
                _player.characterVisual.moveStyleProps.skateL.transform.localScale = inlineOffsetL.localScale;
                _player.characterVisual.moveStyleProps.skateR.transform.SetLocalPositionAndRotation(inlineOffsetR.localPosition, inlineOffsetR.localRotation);
                _player.characterVisual.moveStyleProps.skateR.transform.localScale = inlineOffsetR.localScale;
            }
        }

        private void OnSkinLoadFinished()
        {
            UnloadShownStreamedCharacter();
            StreamedCharacter = new StreamedCharacterInstance(StreamedCharacterHandle);
            StreamedCharacter.Outfit = StreamedOutfit;
            if (!StreamedCharacter.Handle.Failed && StreamedCharacter.Handle.CharacterPrefab != null)
                ApplyStreamedCharacter();
            SkinLoaded?.Invoke();
        }

        private static int MainTexId = Shader.PropertyToID("_MainTex");
        private static int ColorId = Shader.PropertyToID("_Color");

        public void MakeLOD()
        {
            var lodMat = MPAssets.Instance.LODMaterial;
            var renderers = _player.characterVisual.GetComponentsInChildren<Renderer>(true);
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
                            if (renderer.sharedMaterials[i].renderQueue < 3000)
                                transp = false;
                            var propBlock = new MaterialPropertyBlock();
                            if (renderer.sharedMaterials[i].HasProperty(MainTexId))
                            {
                                var mainTex = renderer.sharedMaterials[i].GetTexture(MainTexId);
                                if (mainTex != null)
                                {
                                    propBlock.SetTexture(MainTexId, mainTex);
                                }
                            }
                            if (renderer.sharedMaterials[i].HasProperty(ColorId))
                            {
                                var color = renderer.sharedMaterials[i].GetColor(ColorId);
                                if (color != null)
                                {
                                    propBlock.SetColor(ColorId, color);
                                }
                            }
                            renderer.SetPropertyBlock(propBlock, i);
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
