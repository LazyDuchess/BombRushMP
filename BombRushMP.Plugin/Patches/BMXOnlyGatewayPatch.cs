using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Reptile;
using UnityEngine;

namespace BombRushMP.Plugin.Patches
{
    [HarmonyPatch(typeof(BMXOnlyGateway))]
    internal static class BMXOnlyGatewayPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(BMXOnlyGateway.OnTriggerStay))]
        private static bool OnTriggerStay_Prefix(Collider other)
        {
            if (other.gameObject.layer != Layers.PlayerInteract) return true;
            var player = other.GetComponentInParent<Player>();
            if (player == null) return true;
            if (player.isAI) return false;
            return true;
        }
    }
}
