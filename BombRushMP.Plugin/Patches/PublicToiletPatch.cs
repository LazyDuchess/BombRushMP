using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Reptile;

namespace BombRushMP.Plugin.Patches
{
    [HarmonyPatch(typeof(PublicToilet))]
    internal static class PublicToiletPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(PublicToilet.DoSequence))]
        private static bool DoSequence_Prefix()
        {
            var playerComp = PlayerComponent.GetLocal();
            if (playerComp.SpecialSkin != Common.SpecialSkins.None)
                return false;
            return true;
        }
    }
}
