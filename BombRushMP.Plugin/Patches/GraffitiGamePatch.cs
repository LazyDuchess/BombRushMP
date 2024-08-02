using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BombRushMP.Common.Packets;
using HarmonyLib;
using Reptile;
using UnityEngine;

namespace BombRushMP.Plugin.Patches
{
    [HarmonyPatch(typeof(GraffitiGame))]
    internal static class GraffitiGamePatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(GraffitiGame.Play))]
        private static void Play_Postfix(string anim)
        {
            var clientController = ClientController.Instance;
            if (clientController == null) return;
            if (!clientController.Connected) return;
            clientController.SendPacket(new PlayerAnimation(Animator.StringToHash(anim)), Riptide.MessageSendMode.Reliable);
            if (anim == "grafSlashFinisher")
                clientController.SendPacket(new PlayerGraffitiFinisher(), Riptide.MessageSendMode.Reliable);
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(GraffitiGame.PlayPuppetSlashAnim))]
        private static void PlayPuppetSlashAnim_Postfix(Side side, bool outAnim)
        {
            var clientController = ClientController.Instance;
            if (clientController == null) return;
            if (!clientController.Connected) return;
            clientController.SendPacket(new PlayerAnimation(Animator.StringToHash("grafSlash" + side.ToString() + (outAnim ? "_Out" : ""))), Riptide.MessageSendMode.Reliable);
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(GraffitiGame.DoGraffitiEffect))]
        private static void DoGraffitiEffect_Postfix(GraffitiGame __instance)
        {
            var clientController = ClientController.Instance;
            if (clientController == null) return;
            if (!clientController.Connected) return;
            clientController.SendPacket(new PlayerGraffitiSlash(), Riptide.MessageSendMode.Reliable);
        }
    }
}
