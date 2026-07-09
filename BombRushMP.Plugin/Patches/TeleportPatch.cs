using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reptile;
using HarmonyLib;

namespace BombRushMP.Plugin.Patches
{
    [HarmonyPatch(typeof(Teleport))]
    internal static class TeleportPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(Teleport.DoTeleport))]
        private static void DoTeleport_Prefix(Teleport __instance, Player player)
        {
            if (__instance.teleportRoutine == null && __instance.doDamage && !player.isAI && MPUtility.GetRagdollAllowed() && MPSettings.Instance.RagdollOnHit)
            {
                PlayerComponent.GetLocal().Ragdoll.BecomeRagdoll(new PlayerRagdoll.Parameters(PlayerRagdoll.Modes.Hit));
            }
        }
    }
}
