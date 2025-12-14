using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reptile;
using UnityEngine;
using HarmonyLib;

namespace BombRushMP.Plugin.Patches
{
    [HarmonyPatch(typeof(AmbientManager))]
    internal class AmbientManagerPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(AmbientManager.Awake))]
        private static void Awake_Prefix(AmbientManager __instance)
        {
            if (!MPUtility.IsChristmas()) return;
            if (__instance.gameObject.scene.name != "hideout") return;
            __instance.transform.rotation = Quaternion.Euler(32f, 337f, 127f);
            __instance.AmbientEnvLight = new Color(0.236f, 0.406f, 0.609f);
            __instance.AmbientEnvShadow = new Color(0.038f, 0.112f, 0.295f);
        }
    }
}
