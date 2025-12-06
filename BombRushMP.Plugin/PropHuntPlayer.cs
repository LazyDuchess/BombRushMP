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

        private float _turnSpeed = 1000f;
        private Vector3 _targetFacing = Vector3.zero;

        private float _backwardsThreshold = -0.1f;
        private float _turnThreshold = 0.2f;

        private void Awake()
        {
            _player = GetComponent<Player>();
            _cam = GameplayCamera.instance;
        }

        public static PropHuntPlayer GetLocal()
        {
            return WorldHandler.instance.GetCurrentPlayer().GetComponent<PropHuntPlayer>();
        }

        private bool IsAimingBackwards(Vector3 forward, float thresh)
        {
            var camForwardFlat = _cam.transform.forward;
            camForwardFlat.y = 0f;
            camForwardFlat = camForwardFlat.normalized;

            var playerForwardFlat = forward;
            playerForwardFlat.y = 0f;
            playerForwardFlat = playerForwardFlat.normalized;

            if (Vector3.Dot(camForwardFlat, playerForwardFlat) < thresh) return true;

            return false;
        }

        private bool PressingMovementKeys()
        {
            return _player.moveInput.magnitude > 0f;
        }

        private bool CanTurnToAim()
        {
            return (_player.ability == null && _player.GetTotalSpeed() <= 0.5f && !PressingMovementKeys());
        }

        private void FixedUpdate()
        {
            if (_aiming && CanTurnToAim())
            {
                var flatCurrentForward = _player.transform.forward;
                flatCurrentForward.y = 0f;
                flatCurrentForward = flatCurrentForward.normalized;

                var yawDelta = Vector3.SignedAngle(flatCurrentForward, _targetFacing, Vector3.up);
                _player.SetRotation(_player.transform.rotation * Quaternion.Euler(0f, Mathf.Clamp(yawDelta, -_turnSpeed * Time.deltaTime, _turnSpeed * Time.deltaTime), 0f));
            }
        }

        private Vector3 GetMaxTurnRotation()
        {
            var flatCurrentForward = _player.transform.forward;
            flatCurrentForward.y = 0f;
            flatCurrentForward = flatCurrentForward.normalized;

            var flatCamForward = _cam.transform.forward;
            flatCamForward.y = 0f;
            flatCamForward = flatCamForward.normalized;


            var yawDelta = Vector3.SignedAngle(flatCurrentForward, _targetFacing, Vector3.up);

            var finalFw = Quaternion.Euler(0f, _player.transform.rotation.eulerAngles.y + Mathf.Sign(yawDelta)*90f, 0f) * Vector3.forward;
            finalFw.y = _cam.transform.forward.y;
            return finalFw.normalized;
        }

        private void AnimUpdate()
        {
            if (_spineAimAmount > 0f)
            {
                _player.anim.speed = 0f;
                _player.anim.SetFloat(_player.phoneDirectionXHash, -_player.phoneDirBone.localRotation.x * 2.5f + _player.customPhoneHandValue.x);
                _player.anim.SetFloat(_player.phoneDirectionYHash, -_player.phoneDirBone.localRotation.y * 2.5f + _player.customPhoneHandValue.y);
                _player.anim.Update(Time.deltaTime);
                _player.anim.speed = 1f;
            }
        }

        private void OnDestroy()
        {
            _player.switchToEquippedMovestyleLocked = false;
        }

        private bool CanPlayerAim()
        {
            if (_player.ability == null) return true;
            if (_player.ability is GrindAbility) return true;
            if (_player.ability is BoostAbility) return true;
            if (_player.ability is AirDashAbility) return true;
            if (_player.ability is SlideAbility && _player.usingEquippedMovestyle) return true;
            return false;
        }
        
        private void LateUpdate()
        {
            AnimUpdate();
            var aimingBackwards = IsAimingBackwards(_player.transform.forward, _backwardsThreshold);
            var canTurnToAIm = CanTurnToAim();
            _aiming = _player.sprayButtonHeld;
            if (_aiming)
            {
                _cameraAimAmount = Mathf.Lerp(_cameraAimAmount, 1f, _cameraAimSpeed * Time.deltaTime);
            }
            else
            {
                _cameraAimAmount = Mathf.Lerp(_cameraAimAmount, 0f, _cameraAimSpeed * Time.deltaTime);
            }

            if (_aiming && CanPlayerAim() && (!aimingBackwards || canTurnToAIm))
            {
                _spineAimAmount = Mathf.Lerp(_spineAimAmount, 1f, _spineAimSpeed * Time.deltaTime);
                _player.anim.Play(_player.canSprayHash, 1, 0.5f);
            }
            else
            {
                _spineAimAmount = Mathf.Lerp(_spineAimAmount, 0f, _spineAimSpeed * Time.deltaTime);
            }

            if (_aiming && IsAimingBackwards(_targetFacing, _turnThreshold) && canTurnToAIm)
            {
                var camForwardFlat = _cam.transform.forward;
                camForwardFlat.y = 0f;
                camForwardFlat = camForwardFlat.normalized;
                _targetFacing = camForwardFlat;
            }

            _player.spraycanLayerWeight = _spineAimAmount;
            _player.anim.SetLayerWeight(1, _spineAimAmount);
            _player.anim.SetLayerWeight(2, _spineAimAmount);

            var camPos = _player.transform.position;
            camPos += Vector3.up * 1.5f;
            camPos += _cam.transform.right * 0.5f;
            camPos -= _cam.transform.forward * 1f;

            var targetSpineDir = _cam.transform.forward;
            if (aimingBackwards)
            {
                targetSpineDir = GetMaxTurnRotation();
            }
            var spine = _player.characterVisual.head.parent.parent;
            var localDir = spine.InverseTransformDirection(targetSpineDir);
            var delta = Quaternion.FromToRotation(Vector3.forward, localDir);
            var finalRot = spine.localRotation * delta;

            spine.localRotation = Quaternion.Slerp(spine.localRotation, finalRot, _spineAimAmount * 0.7f);
            _cam.transform.position = Vector3.Lerp(_cam.transform.position, camPos, _cameraAimAmount);

            

            if (!_aiming || !canTurnToAIm || PressingMovementKeys())
            {
                _targetFacing = _player.transform.forward;
                _targetFacing.y = 0f;
                _targetFacing = _targetFacing.normalized;
            }

            _player.switchToEquippedMovestyleLocked = _aiming;
        }
    }
}
