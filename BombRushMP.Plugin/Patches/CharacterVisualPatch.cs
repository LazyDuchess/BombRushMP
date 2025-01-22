using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BombRushMP.CrewBoom;
using HarmonyLib;
using Reptile;

namespace BombRushMP.Plugin.Patches
{
    [HarmonyPatch(typeof(CharacterVisual))]
    internal static class CharacterVisualPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(CharacterVisual.SetInlineSkatesPropsMode))]
        private static void SetInlineSkatesPropsMode_Postfix(CharacterVisual __instance)
        {
            var inlineOffsetL = __instance.footL.Find(CrewBoomSupport.SKATE_OFFSET_L);
            var inlineOffsetR = __instance.footR.Find(CrewBoomSupport.SKATE_OFFSET_R);

            if (inlineOffsetL != null && inlineOffsetR != null)
            {
                __instance.moveStyleProps.skateL.transform.SetLocalPositionAndRotation(inlineOffsetL.localPosition, inlineOffsetL.localRotation);
                __instance.moveStyleProps.skateL.transform.localScale = inlineOffsetL.localScale;
                __instance.moveStyleProps.skateR.transform.SetLocalPositionAndRotation(inlineOffsetR.localPosition, inlineOffsetR.localRotation);
                __instance.moveStyleProps.skateR.transform.localScale = inlineOffsetR.localScale;
            }
        }
    }
}
