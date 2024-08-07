using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reptile;
using HarmonyLib;

namespace BombRushMP.Plugin.Patches
{
    [HarmonyPatch(typeof(StageManager))]
    internal static class StageManagerPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(StageManager.IsPauseMenuAllowed))]
        private static bool IsPauseMenuAllowed_Prefix(ref bool __result)
        {
            if (SpectatorController.Instance != null)
            {
                __result = false;
                return false;
            }
            return true;
        }
    }
}
