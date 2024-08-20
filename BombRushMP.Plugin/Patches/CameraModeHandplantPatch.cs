using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reptile;
using HarmonyLib;
using UnityEngine;

namespace BombRushMP.Plugin.Patches
{
    [HarmonyPatch(typeof(CameraModeHandplant))]
    internal class CameraModeHandplantPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(CameraModeHandplant.UpdatePos))]
        private static bool UpdatePos_Prefix(int fixedFrameCounter, CameraModeHandplant __instance)
        {
            var player = __instance.player;
            var proSkater = ProSkaterPlayer.Get(player);
            if (proSkater == null) return true;
            __instance.HandleLookAt(fixedFrameCounter, null);
            bool toDefault = __instance.cam.orbitInput != Vector2.zero;
            if (toDefault)
            {
                __instance.goToDefault = false;
            }
            __instance.position = __instance.HandleCamInput(__instance.position);
            __instance.position = __instance.LerpTo(__instance.position, __instance.lookAtPos + (__instance.position - __instance.lookAtPos).normalized * __instance.cam.handplantCamDist, 2f);
            if (__instance.handPlantAbility.screwSpinTimer > 0f)
            {
                __instance.manualOrbitFactor = Mathf.Clamp01(__instance.lastOrbitTimer);
                __instance.position.y = __instance.LerpTo(__instance.position.y, __instance.lookAtPos.y + __instance.cam.handplantHeightOffsetDefault, 6f * __instance.manualOrbitFactor);
            }
            else if (__instance.goToDefault)
            {
                __instance.position.y = __instance.LerpTo(__instance.position.y, __instance.lookAtPos.y + __instance.cam.handplantHeightOffsetDefault, 6f * __instance.manualOrbitFactor);
            }
            __instance.positionFinal = __instance.HandleObstructions(__instance.position);
            return false;
        }
    }
}
