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
    [HarmonyPatch(typeof(AnimationEventRelay))]
    internal static class AnimationEventRelayPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(AnimationEventRelay.PlaySound))]
        private static bool PlaySound_Prefix(AnimationEvent animationEvent)
        {
            try
            {
                Enum.Parse(typeof(AudioClipID), animationEvent.stringParameter, true);
            }
            catch(ArgumentException)
            {
                return false;
            }
            return true;
        }
    }
}
