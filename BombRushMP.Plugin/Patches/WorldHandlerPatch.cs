using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reptile;
using HarmonyLib;
using UnityEngine;

namespace BombRushMP.Plugin.Patches
{
    [HarmonyPatch(typeof(WorldHandler))]
    internal static class WorldHandlerPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(WorldHandler.PlacePlayerAt), [typeof(Player), typeof(Vector3), typeof(Quaternion), typeof(bool)])]
        private static void PlacePlayerAt_Prefix(Player player, bool stopAbility)
        {
            if (!stopAbility && !player.isAI && !player.IsDead())
            {
                var playerComp = PlayerComponent.GetLocal();
                if (playerComp.Ragdoll.Active)
                    playerComp.Ragdoll.StopRagdoll();
            }
        }
    }
}
