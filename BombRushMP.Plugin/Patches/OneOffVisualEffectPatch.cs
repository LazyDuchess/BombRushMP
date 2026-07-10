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
    [HarmonyPatch(typeof(OneOffVisualEffect))]
    internal static class OneOffVisualEffectPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(OneOffVisualEffect.Awake))]
        private static void Awake_Postfix(OneOffVisualEffect __instance)
        {
            if (__instance.anim != null)
            {
                __instance.anim.cullingType = AnimationCullingType.AlwaysAnimate;
            }
        }
    }
}
