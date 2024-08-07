using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reptile;
using HarmonyLib;

namespace BombRushMP.Plugin.Patches
{
    [HarmonyPatch(typeof(Mapcontroller))]
    internal static class MapcontrollerPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(Mapcontroller.UpdateCamera))]
        private static bool UpdateCamera_Prefix(Mapcontroller __instance)
        {
            if (__instance.m_Player == null) return false;
            if (__instance.m_Player.cam == null) return false;
            return true;
        }
    }
}
