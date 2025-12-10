using BombRushMP.Common.Networking;
using BombRushMP.Common.Packets;
using BombRushMP.Plugin.Gamemodes;
using Reptile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

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
        private bool _canShoot = false;

        private float _turnSpeed = 1000f;
        private Vector3 _targetFacing = Vector3.zero;

        private float _backwardsThreshold = -0.1f;
        private float _turnThreshold = 0.2f;

        private float _targetDistance = 20f;

        private int _layerMask = (1 << Layers.Default) | (1 << Layers.Junk) | (1 << Layers.CameraIgnore) | (1 << Layers.NonStableSurface) | (1 << Layers.Player) | (1 << Layers.StreetLife);

        private GameObject _target = null;

        private bool _frozen = false;

        private float _propFireRate = 1.0f;
        private float _hunterFireRate = 0.4f;
        private float _fireTimer = 0f;

        public bool Frozen => _frozen;
        public bool Aiming => _aiming;

        private float _timeToLockOnHit = 3f;
        private float _lockedTimer = 0f;

        public void HitLock()
        {
            _lockedTimer = _timeToLockOnHit;
        }

        private void FixedUpdate_Prop()
        {
            var propDisguiseController = PropDisguiseController.Instance;

            if (!propDisguiseController.InSetupPhase)
            {
                var playerMotor = _player.motor;
                var playerSpeed = _player.motor.velocity;

                var hSpeed = playerSpeed;
                hSpeed.y = 0f;

                var vSpeed = playerSpeed;
                vSpeed.x = 0f;
                vSpeed.z = 0f;

                if (hSpeed.magnitude >= propDisguiseController.PropHorizontalSpeed)
                    hSpeed = propDisguiseController.PropHorizontalSpeed * hSpeed.normalized;

                if (vSpeed.magnitude >= propDisguiseController.PropVerticalSpeed)
                    vSpeed = propDisguiseController.PropVerticalSpeed * vSpeed.normalized;

                _player.motor.velocity = hSpeed + vSpeed;
            }
        }

        private void Awake()
        {
            _player = GetComponent<Player>();
            _cam = GameplayCamera.instance;
        }

        public static PropHuntPlayer GetLocal()
        {
            return WorldHandler.instance.GetCurrentPlayer().GetComponent<PropHuntPlayer>();
        }

        public void Freeze()
        {
            if (_frozen) return;
            _frozen = true;
            _player.motor._rigidbody.isKinematic = true;
            _player.motor._rigidbody.velocity = Vector3.zero;
            _player.motor._rigidbody.rotation = Quaternion.identity;
        }

        public void Unfreeze()
        {
            if (!_frozen) return;
            _frozen = false;
            _player.motor._rigidbody.isKinematic = false;
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

        private GameObject CalculateTarget()
        {
            var propDisguiseController = PropDisguiseController.Instance;

            var targetDistance = _targetDistance;
            if (propDisguiseController.LocalPropHuntTeam == PropHuntTeams.Hunters)
                targetDistance = Mathf.Infinity;

            var ray = new Ray(_cam.transform.position, _cam.transform.forward);
            var hits = Physics.RaycastAll(ray, targetDistance, _layerMask, QueryTriggerInteraction.Collide);
            
            GameObject closestHit = null;
            var closestHitDistance = Mathf.Infinity;
            foreach(var hit in hits)
            {
                var playa = hit.collider.gameObject.GetComponentInParent<PlayerComponent>();
                if (playa != null)
                {
                    if (playa.Player == _player) continue;
                    if (playa.HasPropDisguise && hit.collider.gameObject.layer == Layers.Player)
                    {
                        if (propDisguiseController.LocalPropHuntTeam == PropHuntTeams.Props) continue;
                        var disguiseStreetLife = playa.DisguiseGameObject.GetComponentInChildren<StreetLife>();
                        if (disguiseStreetLife == null) continue;
                    }
                }

                if (propDisguiseController.LocalPropHuntTeam == PropHuntTeams.Hunters && hit.collider.gameObject.layer == Layers.StreetLife && playa == null) continue;

                var dist = Vector3.Distance(_cam.transform.position, hit.point);
                if (closestHit == null)
                {
                    closestHit = hit.collider.gameObject;
                    closestHitDistance = dist;
                }
                else
                {
                    if (dist < closestHitDistance)
                    {
                        closestHit = hit.collider.gameObject;
                        closestHitDistance = dist;
                    }
                }
            }

            return closestHit;
        }

        private void FixedUpdate()
        {
            var propDisguiseController = PropDisguiseController.Instance;
            var playerComp = PlayerComponent.GetLocal();
            
            if (_aiming && CanTurnToAim() && !playerComp.HasPropDisguise)
            {
                var flatCurrentForward = _player.transform.forward;
                flatCurrentForward.y = 0f;
                flatCurrentForward = flatCurrentForward.normalized;

                var yawDelta = Vector3.SignedAngle(flatCurrentForward, _targetFacing, Vector3.up);
                _player.SetRotation(_player.transform.rotation * Quaternion.Euler(0f, Mathf.Clamp(yawDelta, -_turnSpeed * Time.deltaTime, _turnSpeed * Time.deltaTime), 0f));
            }

            if (propDisguiseController.LocalPropHuntTeam == PropHuntTeams.Props)
                FixedUpdate_Prop();
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
            var propDisguiseController = PropDisguiseController.Instance;
            propDisguiseController.OutlineGameObject(null);
            _player.switchToEquippedMovestyleLocked = false;
            Unfreeze();
        }

        private bool CanPlayerAim()
        {
            if (_player.ability == null) return true;
            if (_player.ability is BoostAbility) return true;
            if (_player.ability is AirDashAbility) return true;
            if (_player.ability is SlideAbility && _player.usingEquippedMovestyle) return true;
            return false;
        }
        
        private void LateUpdate()
        {
            _target = null;
            AnimUpdate();
            var aimingBackwards = IsAimingBackwards(_player.transform.forward, _backwardsThreshold);
            var canTurnToAIm = CanTurnToAim();
            _aiming = _player.sprayButtonHeld;
            _canShoot = false;
            var freezeButton = _player.boostButtonNew;

            if (_lockedTimer > 0f)
            {
                _aiming = false;
                freezeButton = false;
            }

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
                _canShoot = true;
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

            var propDisguiseController = PropDisguiseController.Instance;
            if (_canShoot)
                _target = CalculateTarget();
            if (propDisguiseController.LocalPropHuntTeam == PropHuntTeams.Props)
            {
                if (_canShoot)
                {
                    if (_target != null && propDisguiseController.IndexByProp.TryGetValue(_target, out var propIndex))
                        propDisguiseController.OutlineGameObject(_target);
                    else
                        propDisguiseController.OutlineGameObject(null);
                }
                else
                    propDisguiseController.OutlineGameObject(null);

                if (freezeButton)
                {
                    if (!_frozen)
                        Freeze();
                    else
                        Unfreeze();
                }
            }
            else
                propDisguiseController.OutlineGameObject(null);

            var ui = XHairUI.Instance;
            ui.CurrentMode = XHairUI.Modes.Off;
            if (_aiming)
            {
                ui.CurrentMode = XHairUI.Modes.On;
                if (!_canShoot)
                    ui.CurrentMode = XHairUI.Modes.Cross;
            }

            if (_canShoot && _player.switchStyleButtonNew && _fireTimer <= 0f)
            {
                if (propDisguiseController.LocalPropHuntTeam == PropHuntTeams.Props)
                {
                    if (_target != null && propDisguiseController.IndexByProp.TryGetValue(_target, out var propIndex))
                    {
                        PlayerComponent.GetLocal().ApplyPropDisguise(propIndex);
                    }
                }
                if (propDisguiseController.LocalPropHuntTeam == PropHuntTeams.Hunters && !propDisguiseController.InSetupPhase)
                {
                    _player.AudioManager.PlaySfxGameplay(SfxCollectionID.CombatSfx, AudioClipID.PistolShot, 0.05f);
                    var dmg = 1;
                    if (_target != null)
                    {
                        var playerTarget = _target.GetComponentInParent<Player>();
                        if (playerTarget != null)
                        {
                            var mpPlayer = MPUtility.GetMuliplayerPlayer(playerTarget);
                            if (mpPlayer != null)
                            {
                                var currentLobby = ClientController.Instance.ClientLobbyManager.CurrentLobby;
                                if (currentLobby.LobbyState.Players.TryGetValue(mpPlayer.ClientId, out var lobbyPlayer))
                                {
                                    if (lobbyPlayer.Team == (byte)PropHuntTeams.Props)
                                    {
                                        ui.HitMarkerTime = 0.5f;
                                        var dmgPacket = new ClientPropHuntShoot(mpPlayer.ClientId);
                                        ClientController.Instance.SendPacket(dmgPacket, IMessage.SendModes.Reliable, NetChannels.Gamemodes);
                                    }
                                    else
                                    {
                                        _player.ChangeHP(dmg);
                                    }
                                }
                                else
                                {
                                    _player.ChangeHP(dmg);
                                }
                            }
                            else
                            {
                                _player.ChangeHP(dmg);
                            }
                        }
                        else
                        {
                            _player.ChangeHP(dmg);
                        }
                    }
                    else
                    {
                        _player.ChangeHP(dmg);
                    }
                }
                _fireTimer = propDisguiseController.LocalPropHuntTeam == PropHuntTeams.Hunters ? _hunterFireRate : _propFireRate;
            }

            if (propDisguiseController.LocalPropHuntTeam == PropHuntTeams.Hunters)
            {
                _player.boostCharge = _player.maxBoostCharge;
            }
            else
            {
                _player.boostCharge = 0f;
                _player.recoverDamageTimer = 0f;
            }

            if (propDisguiseController.InSetupPhase)
            {
                _player.ResetHP();
            }
            _fireTimer -= Time.deltaTime;
            if (_fireTimer <= 0f)
                _fireTimer = 0f;
            _lockedTimer -= Time.deltaTime;

            if (_lockedTimer <= 0f)
                _lockedTimer = 0f;
        }
    }
}
