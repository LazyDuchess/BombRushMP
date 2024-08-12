using BepInEx.Configuration;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Plugin.Patches
{
    [HarmonyPatch(typeof(KeyboardShortcut))]
    internal class KeyboardShortcutPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(KeyboardShortcut.IsDown))]
        private static bool IsDown_Prefix(ref bool __result)
        {
            if (InputUtils.InputBlocked)
            {
                __result = false;
                return false;
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(KeyboardShortcut.IsPressed))]
        private static bool IsPressed_Prefix(ref bool __result)
        {
            if (InputUtils.InputBlocked)
            {
                __result = false;
                return false;
            }
            return true;
        }
    }
}
