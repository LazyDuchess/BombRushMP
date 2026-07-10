using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reptile;
using UnityEngine;
using HarmonyLib;

namespace BombRushMP.Plugin.Patches
{
    [HarmonyPatch(typeof(MoveAlongHandler))]
    internal static class MoveAlongHandlerPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(MoveAlongHandler.HitboxHitPlayer))]
        private static bool HitboxHitPlayer_Prefix(MoveAlongHandler __instance, Player player, MoveAlongPoints hitMover)
        {
            if (!MPSettings.Instance.FreeroamTraffic) return true;
            var clientController = ClientController.Instance;
            if (clientController == null) return true;
            if (clientController.ClientLobbyManager.CurrentLobby != null) return true;
            int num = __instance.moversCold.FindIndexOf(hitMover, null);
            if (num == -1)
            {
                return false;
            }
            MoveAlongPoints_Hot moveAlongPoints_Hot = __instance.moversHot[num];
            if (moveAlongPoints_Hot.activelyMoving || moveAlongPoints_Hot.timeSpeed >= 0.3f)
            {
                if (MPUtility.GetRagdollAllowed() && MPSettings.Instance.RagdollOnHit)
                {
                    if (!moveAlongPoints_Hot.activelyMoving && moveAlongPoints_Hot.timeSpeed <= 0.7f)
                    {
                        player.GetHit(1, (moveAlongPoints_Hot.tf.position - player.tf.position).normalized, KnockbackAbility.KnockbackType.FAR);
                    }
                    else
                    {
                        player.GetHit(1, (moveAlongPoints_Hot.tf.position - player.tf.position).normalized, KnockbackAbility.KnockbackType.BIG);
                    }
                }
                else
                {
                    player.GetHit(1, (moveAlongPoints_Hot.tf.position - player.tf.position).normalized, KnockbackAbility.KnockbackType.FAR);
                }
            }
            return false;
        }
    }
}
