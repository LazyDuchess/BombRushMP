using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reptile;
using UnityEngine;

namespace BombRushMP.Plugin
{
    public class PseudoDieAbility : Ability
    {
        private int dieHash = Animator.StringToHash("die");

        public PseudoDieAbility(Player player) : base(player)
        {
        }

        public override void Init()
        {
            normalMovement = true;
            normalRotation = false;
            canStartGrind = false;
            canStartWallrun = false;
            targetSpeed = 0f;
        }

        public override void OnStartAbility()
        {
            p.SwitchToEquippedMovestyle(false, false, true, true);
            p.StartScreenShake(ScreenShakeType.MEDIUM, 0.6f, false);
            try
            {
                p.PlayVoice(AudioClipID.VoiceDie, VoicePriority.DIE, true);
            }
            catch(Exception) { }
            p.PlayAnim(dieHash, false, false, -1f);
            p.RemoveAllCuffs(null);
        }
    }
}
