using HarmonyLib;
using Reptile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Plugin.Patches
{
    [HarmonyPatch(typeof(GameplayUI))]
    internal static class GameplayUIPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(GameplayUI.Init))]
        private static void Init_Postfix(GameplayUI __instance)
        {
            SpectatorUI.InitializeUI(__instance);
            PropHuntUI.InitializeUI(__instance);
        }
    }
}
