using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Reptile;
using HarmonyLib;

namespace BombRushMP.Plugin.Patches
{
    [HarmonyPatch(typeof(GameplayCamera))]
    internal static class GameplayCameraPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(GameplayCamera.OnDestroy))]
        private static bool OnDestroy_Prefix(GameplayCamera __instance)
        {
            if (GameplayCamera.instance != __instance) return false;
            return true;
        }
    }
}
