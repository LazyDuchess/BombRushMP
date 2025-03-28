﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BombRushMP.Common.Packets;
using HarmonyLib;
using Reptile;
using UnityEngine;
using BombRushMP.Common;
using BombRushMP.Plugin.Gamemodes;
using BombRushMP.Common.Networking;

namespace BombRushMP.Plugin.Patches
{
    [HarmonyPatch(typeof(GraffitiGame))]
    internal static class GraffitiGamePatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(GraffitiGame.Play))]
        private static void Play_Postfix(GraffitiGame __instance, string anim)
        {
            var clientController = ClientController.Instance;
            if (clientController == null) return;
            if (!clientController.Connected) return;
            clientController.SendPacket(new PlayerAnimation(Animator.StringToHash(anim)), PlayerAnimation.ClientSendMode, NetChannels.Animation);
            if (anim == "grafSlashFinisher")
                clientController.SendPacket(new PlayerGraffitiFinisher((byte)__instance.gSpot.size), IMessage.SendModes.ReliableUnordered, NetChannels.VisualUpdates);
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(GraffitiGame.PlayPuppetSlashAnim))]
        private static void PlayPuppetSlashAnim_Postfix(Side side, bool outAnim)
        {
            var clientController = ClientController.Instance;
            if (clientController == null) return;
            if (!clientController.Connected) return;
            clientController.SendPacket(new PlayerAnimation(Animator.StringToHash("grafSlash" + side.ToString() + (outAnim ? "_Out" : ""))), PlayerAnimation.ClientSendMode, NetChannels.Animation);
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(GraffitiGame.DoGraffitiEffect))]
        private static void DoGraffitiEffect_Postfix(GraffitiGame __instance)
        {
            var clientController = ClientController.Instance;
            if (clientController == null) return;
            if (!clientController.Connected) return;
            clientController.SendPacket(new PlayerGraffitiSlash(__instance.slashCardinal.ToSystemVector3()), IMessage.SendModes.ReliableUnordered, NetChannels.VisualUpdates);
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(GraffitiGame.SetState))]
        private static void SetState_Prefix(GraffitiGame __instance, GraffitiGame.GraffitiGameState setState)
        {
            if (setState != GraffitiGame.GraffitiGameState.SHOW_PIECE) return;
            var clientController = ClientController.Instance;
            if (clientController == null) return;
            var lobby = clientController.ClientLobbyManager.CurrentLobby;
            if (lobby == null) return;
            if (!lobby.InGame) return;
            if (lobby.CurrentGamemode is not GraffitiRace) return;
            var grafRace = lobby.CurrentGamemode as GraffitiRace;
            if (!grafRace.IsRaceGraffitiSpot(__instance.gSpot)) return;
            grafRace.AddScore(__instance.gSpot);
            grafRace.MarkGraffitiSpotDone(__instance.gSpot);
        }
    }
}
