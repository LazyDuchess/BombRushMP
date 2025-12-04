using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reptile;
using HarmonyLib;
using UnityEngine;
using BombRushMP.Plugin.Gamemodes;

namespace BombRushMP.Plugin.Patches
{
    [HarmonyPatch(typeof(StreetLife))]
    internal static class StreetLifePatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(StreetLife.OnTriggerStay))]
        private static bool OnTriggerStay(Collider other)
        {
            var propDisguiseController = PropDisguiseController.Instance;
            if (propDisguiseController != null && propDisguiseController.FrozenProps) return false;
            return true;
        }
    }
}
