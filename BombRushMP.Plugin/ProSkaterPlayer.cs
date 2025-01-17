using BombRushMP.Plugin.Gamemodes;
using Reptile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BombRushMP.Plugin
{
    public class BalanceMeter
    {
        public float Current = 0f;
        public float CurrentSpeed = 0f;
        public float CurrentSensitivity = 0f;
        public float BeginSensitivity = 0.25f;
        public float PassiveSensitivityIncrease = 0.1f;
        public float MinimumDirection = 0.1f;
        public float NudgeForce = 6f;

        public BalanceMeter()
        {
            Reset();
        }

        public void Reset()
        {
            Current = 0f;
            CurrentSpeed = 0f;
            CurrentSensitivity = BeginSensitivity;
        }

        public void DoPenalty(float strength, float sensitivityStrength, float minimumBalance)
        {
            if (Mathf.Abs(Current) < minimumBalance)
            {
                if (Current == 0f)
                {
                    var sign = UnityEngine.Random.Range(0, 2) == 0f ? 1f : -1f;
                    Current = minimumBalance * sign;
                }
                else
                    Current += minimumBalance * Mathf.Sign(Current);
            }
            Current *= strength;
            CurrentSpeed *= strength;
            CurrentSensitivity *= sensitivityStrength;
        }

        public void DoSoftReset(float strength)
        {
            Current *= strength;
            CurrentSpeed *= strength;
        }

        public void Nudge(float strength)
        {
            CurrentSpeed += NudgeForce * strength * CurrentSensitivity * Core.dt;
        }

        public void TickActive()
        {
            var direction = Current;
            if (Current == 0f)
            {
                if (CurrentSpeed != 0f)
                    direction = MinimumDirection * Mathf.Sign(CurrentSpeed);
                else
                {
                    var mul = -1f;
                    var dir = UnityEngine.Random.Range(0, 2);
                    if (dir == 0)
                        mul = 1f;
                    direction = MinimumDirection * mul;
                }
            }
            else
            {
                if (Mathf.Abs(Current) < MinimumDirection)
                    direction = MinimumDirection * Mathf.Sign(Current);
            }
            direction *= CurrentSensitivity;
            CurrentSpeed += direction * Core.dt;
            Current += CurrentSpeed * Core.dt;
            Current = Mathf.Clamp(Current, -1f, 1f);
            if (Current >= 1f && CurrentSpeed > 0f)
                CurrentSpeed = 0f;
            if (Current <= -1f && CurrentSpeed < 0f)
                CurrentSpeed = 0f;
            CurrentSensitivity += PassiveSensitivityIncrease * Core.dt;
        }
    }
    public class ProSkaterPlayer : MonoBehaviour
    {
        public const float GrindPenaltyTime = 0.1f;
        private const float GrindSoftReset = 0.5f;
        private const float GrindPenalty = 1.25f;
        private const float GrindSensitivityPenalty = 1.1f;
        private const float GrindPenaltyMinimumBalance = 0.2f;
        public const float LeaveGrindSensitivity = 0.0025f;

        public const float ManualPenaltyTime = 0.1f;
        private const float ManualSoftReset = 0.5f;
        private const float ManualPenalty = 1.25f;
        private const float ManualSensitivityPenalty = 1.1f;
        private const float ManualPenaltyMinimumBalance = 0.2f;
        public const float LeaveManualSensitivity = 0.0025f;

        public bool DidGrind = false;
        public bool DidManual = false;
        public BalanceMeter GrindBalance = new();
        public BalanceMeter ManualBalance = new();
        private Player _player;
        
        private void Awake()
        {
            _player = GetComponent<Player>();
        }

        public void OnEndCombo()
        {
            DidGrind = false;
            DidManual = false;
            GrindBalance.Reset();
            ManualBalance.Reset();
        }

        public void PenalizeGrinding()
        {
            GrindBalance.DoPenalty(GrindPenalty, GrindSensitivityPenalty, GrindPenaltyMinimumBalance);
        }

        public void SoftResetGrinding()
        {
            GrindBalance.DoSoftReset(GrindSoftReset);
        }

        public void PenalizeManual()
        {
            ManualBalance.DoPenalty(ManualPenalty, ManualSensitivityPenalty, ManualPenaltyMinimumBalance);
        }

        public void SoftResetManual()
        {
            ManualBalance.DoSoftReset(ManualSoftReset);
        }

        private void HandplantUpdate(HandplantAbility handplantAbility)
        {
            DidGrind = true;
            GrindBalance.TickActive();
            GrindBalance.Nudge(handplantAbility.p.moveInputPlain.x);
        }

        public void GrindUpdateTilting(GrindAbility grindAbility)
        {
            DidGrind = true;
            GrindBalance.TickActive();
            GrindBalance.Nudge(grindAbility.p.moveInputPlain.x);
            grindAbility.p.anim.SetFloat(grindAbility.grindDirectionHash, GrindBalance.Current);
            grindAbility.grindTilt.x = GrindBalance.Current;

            var clientController = ClientController.Instance;
            if (clientController != null && clientController.ClientLobbyManager != null && clientController.ClientLobbyManager.CurrentLobby != null)
            {
                var lobby = clientController.ClientLobbyManager.CurrentLobby;
                var gamemode = lobby.CurrentGamemode as ProSkaterScoreBattle;
                if (gamemode != null && lobby.InGame && gamemode.RewardTilting)
                {
                    if (Mathf.Abs(grindAbility.grindTilt.x) > 0.25f)
                    {
                        grindAbility.grindTiltBuffer.x = grindAbility.grindTilt.x;
                        grindAbility.grindTiltBufferTimer = 0.1f;
                    }
                    if (grindAbility.grindTiltBufferTimer > 0f)
                    {
                        grindAbility.grindTiltBufferTimer -= Core.dt;
                        if (grindAbility.grindTiltBufferTimer <= 0f)
                        {
                            grindAbility.grindTiltBuffer.x = 0f;
                        }
                    }
                }
            }
        }

        private void Update()
        {
            if (_player.ability != null && (_player.ability is GrindAbility || _player.ability is HandplantAbility))
            {
                if (GrindBalance.Current >= 1f || GrindBalance.Current <= -1f)
                {
                    var balance = GrindBalance.Current;
                    _player.DropCombo();
                    _player.GetHit(0, -transform.right * Mathf.Sign(balance), KnockbackAbility.KnockbackType.FAR);
                }
            }
        }

        private void FixedUpdate()
        {
            if (Core.Instance.IsCorePaused) return;
            if (_player.ability == null) return;
            if (_player.ability is HandplantAbility)
                HandplantUpdate(_player.ability as HandplantAbility);
            if (IsOnManual())
                ManualUpdate();
        }

        public bool IsOnManual()
        {
            if (_player.ability == null) return false;
            if (_player.ability is SlideAbility) return true;
            if (_player.ability is GroundTrickAbility && _player.IsGrounded()) return true;
            return false;
        }

        private void ManualUpdate()
        {
            DidManual = true;
            ManualBalance.TickActive();
            ManualBalance.Nudge(Core.Instance.GameInput.GetAxis(6));
            if (ManualBalance.Current >= 1f || ManualBalance.Current <= -1f)
            {
                var balance = ManualBalance.Current;
                _player.DropCombo();
                _player.GetHit(0, -transform.forward * Mathf.Sign(balance), KnockbackAbility.KnockbackType.FAR);     
            }
            _player.comboTimeOutTimer = 1f;
        }

        public static void Set(Player player, bool set)
        {
            var existing = Get(player);
            if (set && existing != null)
                return;
            if (!set && existing == null)
                return;
            if (set)
                player.gameObject.AddComponent<ProSkaterPlayer>();
            else
                Destroy(existing);
        }

        public static ProSkaterPlayer Get(Player player)
        {
            return player.GetComponent<ProSkaterPlayer>();
        }
    }
}
