using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BombRushMP.Common;
using HarmonyLib;
using Reptile;
using Riptide;

namespace BombRushMP.Plugin.Patches
{
    [HarmonyPatch(typeof(SlideAbility))]
    internal static class SlideAbilityPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(SlideAbility.OnStopAbility))]
        private static void OnStopAbility_Postfix(SlideAbility __instance)
        {
            if (__instance.p.isAI) return;
            var proSkater = ProSkaterPlayer.Get(__instance.p);
            if (proSkater != null)
            {
                if (__instance.p.abilityTimer <= ProSkaterPlayer.ManualPenaltyTime)
                    proSkater.PenalizeManual();
                else
                    proSkater.SoftResetManual();
                proSkater.ManualBalance.CurrentSensitivity += ProSkaterPlayer.LeaveManualSensitivity;
            }
        }
    }
}
