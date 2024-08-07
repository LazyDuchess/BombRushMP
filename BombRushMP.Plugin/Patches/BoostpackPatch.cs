using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reptile;
using HarmonyLib;

namespace BombRushMP.Plugin.Patches
{
    [HarmonyPatch(typeof(Boostpack))]
    internal static class BoostpackPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(Boostpack.LateUpdate))]
        private static void LateUpdate_Prefix(Boostpack __instance)
        {
            var spectatorController = SpectatorController.Instance;
            if (spectatorController != null)
            {
                __instance.player.cam = GameplayCamera.instance;
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(Boostpack.LateUpdate))]
        private static void LateUpdate_Postfix(Boostpack __instance)
        {
            var spectatorController = SpectatorController.Instance;
            if (spectatorController != null)
            {
                __instance.player.cam = null;
            }
        }
    }
}
