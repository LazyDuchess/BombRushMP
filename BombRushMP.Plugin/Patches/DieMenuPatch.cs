using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reptile;
using HarmonyLib;

namespace BombRushMP.Plugin.Patches
{
    [HarmonyPatch(typeof(DieMenu))]
    internal static class DieMenuPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(DieMenu.InitButtons))]
        private static void InitButtons_Postfix(DieMenu __instance)
        {
            __instance.resumeBtn.onClick.RemoveAllListeners();
            __instance.resumeBtn.onClick.AddListener(() =>
            {
                if (CanRevive())
                {
                    MPUtility.Revive();
                }
                else
                {
                    __instance.uIManager.HideDieMenu();
                    Core.Instance.BaseModule.StageManager.RestartStage();
                    __instance.baseModule.UnPauseGame(PauseType.GameOver);
                }
            });
        }

        private static bool CanRevive()
        {
            var worldHandler = WorldHandler.instance;
            var clientController = ClientController.Instance;
            if (!clientController.Connected) return false;
            var encounter = worldHandler.currentEncounter;
            if (encounter != null && encounter is not ProxyEncounter) return false;
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(DieMenu.Activate))]
        private static bool Activate_Prefix()
        {
            var player = WorldHandler.instance.GetCurrentPlayer();
            if (!player.IsDead())
                return false;
            return true;
        }
    }
}
