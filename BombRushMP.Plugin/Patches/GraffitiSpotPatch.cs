using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reptile;
using HarmonyLib;
using BombRushMP.Plugin.Gamemodes;

namespace BombRushMP.Plugin.Patches
{
    [HarmonyPatch(typeof(GraffitiSpot))]   
    internal static class GraffitiSpotPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(GraffitiSpot.WriteToData))]
        private static bool WriteToData_Prefix(GraffitiSpot __instance)
        {
            var currentLobby = ClientController.Instance.ClientLobbyManager.CurrentLobby;

            if (currentLobby != null &&
                currentLobby.InGame &&
                currentLobby.CurrentGamemode is GraffitiRace)
                return false;

            return true;
        }
    }
}
