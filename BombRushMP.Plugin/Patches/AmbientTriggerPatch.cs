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
    [HarmonyPatch(typeof(AmbientTrigger))]
    internal static class AmbientTriggerPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(AmbientTrigger.OnTriggerEnter))]
        private static bool OnTriggerEnter_Prefix(Collider trigger)
        {
            var player = trigger.GetComponentInParent<Player>();
            if (player == null) return true;
            if (player.isAI) return false;
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(AmbientTrigger.OnTriggerExit))]
        private static bool OnTriggerExit_Prefix(Collider trigger)
        {
            var player = trigger.GetComponentInParent<Player>();
            if (player == null) return true;
            if (player.isAI) return false;
            return true;
        }
    }
}
