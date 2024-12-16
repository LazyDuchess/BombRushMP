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

namespace BombRushMP.Plugin
{
    public class PlayerComponent : MonoBehaviour
    {
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
        public Material SkateboardMaterial = null;
        public Material InlineMaterial = null;
        public Material BMXMaterial = null;
        public Material BMXSpokesMaterial = null;

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

        public void ApplyMoveStyleMaterial(Material customMaterial)
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
                renderer.sharedMaterials = [customMaterial, customMaterial];
            }

            allRenderers = moveStyleProps.skateL.GetComponentsInChildren<MeshRenderer>().ToList();
            allRenderers.AddRange(moveStyleProps.skateR.GetComponentsInChildren<MeshRenderer>());

            foreach (var renderer in allRenderers)
            {
                renderer.sharedMaterials = [customMaterial];
            }

            allRenderers = moveStyleProps.skateboard.GetComponentsInChildren<MeshRenderer>().ToList();

            foreach (var renderer in allRenderers)
            {
                renderer.sharedMaterials = [customMaterial];
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
        }

        public void ForceAFK()
        {
            _afkTimer = ClientConstants.AFKTime;
            AFK = true;
        }

        private void LateUpdate()
        {
            var mpSettings = MPSettings.Instance;
            if (Local)
            {
                var gameInput = Core.Instance.GameInput;
                var axisArray = new float[] { gameInput.GetAxis(13), gameInput.GetAxis(14), gameInput.GetAxis(8), gameInput.GetAxis(18)};
                foreach(var axis in axisArray)
                {
                    if (axis != 0f)
                        _afkTimer = 0f;
                }
                if (gameInput.GetAnyButtonNew())
                {
                    _afkTimer = 0f;
                }
                _afkTimer += Core.dt;

                if (_afkTimer >= ClientConstants.AFKTime)
                {
                    AFK = true;
                    _afkTimer = ClientConstants.AFKTime;
                }
                else
                {
                    AFK = false;
                }
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
            return player.GetComponent<PlayerComponent>();
        }
    }
}
