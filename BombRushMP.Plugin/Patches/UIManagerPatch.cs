using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reptile;
using HarmonyLib;

namespace BombRushMP.Plugin.Patches
{
    [HarmonyPatch(typeof(UIManager))]
    internal static class UIManagerPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(UIManager.ShowPauseMenu))]
        private static bool ShowPauseMenu_Prefix()
        {
            if (MPUtility.AnyMenusOpen())
                return false;
            var currentLobby = ClientController.Instance.ClientLobbyManager.CurrentLobby;
            if (currentLobby != null && currentLobby.InGame)
                return false;
            return true;
        }
    }
}
