using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Reptile;

namespace BombRushMP.Plugin.Patches
{
    [HarmonyPatch(typeof(HandplantAbility))]
    internal class HandplantAbilityPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(HandplantAbility.OnStopAbility))]
        private static void OnStopAbility_Postfix(HandplantAbility __instance)
        {
            if (__instance.p.isAI) return;
            var proSkater = ProSkaterPlayer.Get(__instance.p);
            if (proSkater != null)
            {
                if (__instance.p.abilityTimer <= ProSkaterPlayer.GrindPenaltyTime)
                    proSkater.PenalizeGrinding();
                else
                    proSkater.SoftResetGrinding();
            }
        }
    }
}
