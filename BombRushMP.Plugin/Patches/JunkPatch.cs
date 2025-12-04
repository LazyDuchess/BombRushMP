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
    [HarmonyPatch(typeof(Junk))]
    internal static class JunkPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(Junk.OnTriggerStay))]
        private static bool OnTriggerStay_Prefix(Collider other)
        {
            var propDisguiseController = PropDisguiseController.Instance;
            if (propDisguiseController == null) return true;
            if (propDisguiseController.FrozenProps) return false;
            return true;
        }
    }
}
