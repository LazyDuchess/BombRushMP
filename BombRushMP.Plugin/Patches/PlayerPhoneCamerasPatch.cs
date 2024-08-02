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
    [HarmonyPatch(typeof(PlayerPhoneCameras))]
    internal static class PlayerPhoneCamerasPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(PlayerPhoneCameras.Awake))]
        private static bool Awake_Prefix(PlayerPhoneCameras __instance)
        {
            var player = __instance.GetComponentInParent<Player>();
            if (player != null && MPUtility.IsMultiplayerPlayer(player))
            {
                __instance.transform.Find("rearCamera").GetComponent<Camera>().enabled = false;
                __instance.transform.Find("frontCamera").GetComponent<Camera>().enabled = false;
                return false;
            }
            return true;
        }
    }
}
