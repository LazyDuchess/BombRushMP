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
    [HarmonyPatch(typeof(MapPin))]
    internal static class MapPinPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(MapPin.SetupMapPin))]
        private static void SetupMapPin_Postfix(MapPin __instance)
        {
            var clientController = ClientController.Instance;
            if (clientController == null) return;
            var lobby = clientController.ClientLobbyManager.CurrentLobby;
            if (lobby == null) return;
            if (!lobby.InGame) return;
            if (lobby.CurrentGamemode is GraffitiRace)
            {
                if (__instance.m_pinType == MapPin.PinType.GraffitiPin)
                {
                    __instance.DisableMapPinGameObject();
                }
            }
        }
    }
}
