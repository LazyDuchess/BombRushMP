using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BombRushMP.Common.Packets;
using HarmonyLib;
using Reptile;
using Riptide;
using UnityEngine;

namespace BombRushMP.Plugin.Patches
{
    [HarmonyPatch(typeof(Player))]
    internal static class PlayerPatch
    {
        internal static bool PlayAnimPatchEnabled = true;

        [HarmonyPrefix]
        [HarmonyPatch(nameof(Player.PlayAnim))]
        private static bool PlayAnim_Prefix(Player __instance, int newAnim, bool forceOverwrite, bool instant, float atTime)
        {
            var clientController = ClientController.Instance;
            if (clientController == null) return true;
            if (clientController.Connected && !__instance.isAI)
            {
                var packet = new PlayerAnimation();
                packet.NewAnim = newAnim;
                packet.ForceOverwrite = forceOverwrite;
                packet.Instant = instant;
                packet.AtTime = atTime;
                // Doesn't really need to be reliable no?
                clientController.SendPacket(packet, MessageSendMode.Reliable);
            }
            if (!PlayAnimPatchEnabled) return true;
            if (MPUtility.IsMultiplayerPlayer(__instance)) return false;
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(Player.FixedUpdateAbilities))]
        private static bool FixedUpdateAbilities_Prefix(Player __instance)
        {
            if (MPUtility.IsMultiplayerPlayer(__instance)) return false;
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(Player.PlayVoice))]
        private static void PlayVoice_Prefix(Player __instance, AudioClipID audioClipID, VoicePriority voicePriority)
        {
            if (__instance.isAI) return;
            var clientController = ClientController.Instance;
            if (clientController == null) return;
            if (!clientController.Connected) return;
            var packet = new PlayerVoice();
            packet.AudioClipId = (int)audioClipID;
            packet.VoicePriority = (int)voicePriority;
            clientController.SendPacket(packet, MessageSendMode.Reliable);
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(Player.Jump))]
        private static void Jump_Prefix(Player __instance)
        {
            if (__instance.isAI) return;
            var clientController = ClientController.Instance;
            if (clientController == null) return;
            if (!clientController.Connected) return;
            var packet = new PlayerVoice();
            packet.AudioClipId = (int)AudioClipID.VoiceJump;
            packet.VoicePriority = (int)VoicePriority.MOVEMENT;
            clientController.SendPacket(packet, MessageSendMode.Reliable);
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(Player.OnTriggerStay))]
        private static bool OnTriggerStay_Prefix(Player __instance)
        {
            if (MPUtility.IsMultiplayerPlayer(__instance)) return false;
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(Player.CheckWallrun))]
        private static bool CheckWallrun_Prefix(Player __instance)
        {
            if (MPUtility.IsMultiplayerPlayer(__instance)) return false;
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(Player.CheckVert))]
        private static bool CheckVert_Prefix(Player __instance, ref bool __result)
        {
            if (MPUtility.IsMultiplayerPlayer(__instance))
            {
                __result = false;
                return false;
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(Player.StartHitpause))]
        private static bool StartHitpause_Prefix(Player __instance)
        {
            if (MPUtility.IsMultiplayerPlayer(__instance)) return false;
            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(Player.SetOutfit))]
        private static void SetOutfit_Postfix(Player __instance)
        {
            var clientController = ClientController.Instance;
            if (clientController == null) return;
            if (!clientController.Connected) return;
            if (__instance.isAI) return;
            clientController.SendClientState();
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(Player.SetCharacter))]
        private static void SetCharacter_Postfix(Player __instance)
        {
            var clientController = ClientController.Instance;
            if (clientController == null) return;
            if (!clientController.Connected) return;
            if (__instance.isAI) return;
            clientController.SendClientState();
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(Player.OrientVisualToSurface))]
        private static bool OrientVisualToSurface_Prefix(Player __instance)
        {
            if (MPUtility.IsMultiplayerPlayer(__instance)) return false;
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(Player.UpdateVisualRot))]
        private static bool UpdateVisualRot_Prefix(Player __instance)
        {
            if (MPUtility.IsMultiplayerPlayer(__instance)) return false;
            return true;
        }
    }
}
