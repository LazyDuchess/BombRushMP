using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Reptile;

namespace BombRushMP.Plugin
{
    public class PropHuntPlayer : MonoBehaviour
    {
        private Player _player;
        private GameplayCamera _cam;

        private float _cameraAimAmount = 0f;
        private float _spineAimAmount = 0f;

        private float _cameraAimSpeed = 20f;
        private float _spineAimSpeed = 10f;

        private bool _aiming = false;

        private void Awake()
        {
            _player = GetComponent<Player>();
            _cam = GameplayCamera.instance;
        }

        public static PropHuntPlayer GetLocal()
        {
            return WorldHandler.instance.GetCurrentPlayer().GetComponent<PropHuntPlayer>();
        }

        private bool IsAimingBackwards()
        {
            var camForwardFlat = _cam.transform.forward;
            camForwardFlat.y = 0f;
            camForwardFlat = camForwardFlat.normalized;

            var playerForwardFlat = _player.transform.forward;
            playerForwardFlat.y = 0f;
            playerForwardFlat = playerForwardFlat.normalized;

            if (Vector3.Dot(camForwardFlat, playerForwardFlat) < -0.1) return true;

            return false;
        }

        private bool CanTurnToAim()
        {
            return (!_player.usingEquippedMovestyle && _player.ability == null && _player.GetTotalSpeed() <= 0.5f);
        }

        private void LateUpdate()
        {
            var canTurnToAIm = CanTurnToAim();
            _aiming = false;
            if (_player.sprayButtonHeld)
            {
                _cameraAimAmount = Mathf.Lerp(_cameraAimAmount, 1f, _cameraAimSpeed * Time.deltaTime);
                _aiming = true;
            }
            else
            {
                _cameraAimAmount = Mathf.Lerp(_cameraAimAmount, 0f, _cameraAimSpeed * Time.deltaTime);
            }

            if ((_player.sprayButtonHeld && _player.ability == null && !IsAimingBackwards()) || canTurnToAIm)
            {
                _spineAimAmount = Mathf.Lerp(_spineAimAmount, 1f, _spineAimSpeed * Time.deltaTime);
                _player.anim.Play(_player.canSprayHash, 1, 0.5f);
            }
            else
            {
                _spineAimAmount = Mathf.Lerp(_spineAimAmount, 0f, _spineAimSpeed * Time.deltaTime);
            }

            if (_aiming && IsAimingBackwards() && canTurnToAIm)
            {
                var camForwardFlat = _cam.transform.forward;
                camForwardFlat.y = 0f;
                camForwardFlat = camForwardFlat.normalized;
                var targetRot = Quaternion.LookRotation(camForwardFlat, Vector2.up);
                _player.SetRotation(targetRot);
            }

            _player.spraycanLayerWeight = _spineAimAmount;
            _player.anim.SetLayerWeight(1, _spineAimAmount);
            _player.anim.SetLayerWeight(2, _spineAimAmount);

            var camPos = _player.transform.position;
            camPos += Vector3.up * 1.5f;
            camPos += _cam.transform.right * 0.5f;
            camPos -= _cam.transform.forward * 1f;

            var spine = _player.characterVisual.head.parent.parent;
            var localDir = spine.InverseTransformDirection(_cam.transform.forward);
            var delta = Quaternion.FromToRotation(Vector3.forward, localDir);
            var finalRot = spine.localRotation * delta;

            spine.localRotation = Quaternion.Slerp(spine.localRotation, finalRot, _spineAimAmount * 0.7f);
            _cam.transform.position = Vector3.Lerp(_cam.transform.position, camPos, _cameraAimAmount);
        }
    }
}
