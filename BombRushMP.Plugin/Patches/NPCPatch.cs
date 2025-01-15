using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Reptile;
namespace BombRushMP.Plugin.Patches
{
    [HarmonyPatch(typeof(NPC))]
    internal static class NPCPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(NPC.SetAvailable))]
        private static bool SetAvailable_Prefix(bool set, NPC __instance)
        {
            var taxi = __instance.GetComponent<Taxi>();
            if (taxi == null) return true;
            var currentEncounter = WorldHandler.instance.currentEncounter;
            if (!set && currentEncounter != null && currentEncounter is ProxyEncounter && __instance.available)
            {
                __instance.available = false;
                return false;
            }
            return true;
        }
    }
}
