using Reptile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using BombRushMP.Common;

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
