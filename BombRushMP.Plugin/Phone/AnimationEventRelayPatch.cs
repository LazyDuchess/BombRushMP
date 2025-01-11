using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reptile;
using HarmonyLib;

namespace BombRushMP.Plugin.Phone
{
    [HarmonyPatch(typeof(AnimationEventRelay))]
    internal static class AnimationEventRelayPatch
    {
        [HarmonyPatch(nameof(AnimationEventRelay.ActivateLeftLegCollider))]
        [HarmonyPatch(nameof(AnimationEventRelay.ActivateRightLegCollider))]
        [HarmonyPatch(nameof(AnimationEventRelay.ActivateUpperBodyCollider))]
        [HarmonyPatch(nameof(AnimationEventRelay.DeactivateLeftLegCollider))]
        [HarmonyPatch(nameof(AnimationEventRelay.DeactivateRightLegCollider))]
        [HarmonyPatch(nameof(AnimationEventRelay.DeactivateUpperBodyCollider))]
        private static bool ActivateCollider(AnimationEventRelay __instance)
        {
            if (MPUtility.IsMultiplayerPlayer(__instance.player)) return false;
            return true;
        }
    }
}
