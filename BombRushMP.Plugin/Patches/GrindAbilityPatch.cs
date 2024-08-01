using BombRushMP.Common.Packets;
using HarmonyLib;
using Reptile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Riptide;

namespace BombRushMP.Plugin.Patches
{
    [HarmonyPatch(typeof(GrindAbility))]
    internal class GrindAbilityPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(GrindAbility.JumpOut))]
        private static void JumpOut_Prefix(GrindAbility __instance)
        {
            if (__instance.p.isAI) return;
            var clientController = ClientController.Instance;
            if (clientController == null) return;
            if (!clientController.Connected) return;
            var voicePacket = new PlayerVoice();
            voicePacket.AudioClipId = (int)AudioClipID.VoiceJump;
            voicePacket.VoicePriority = (int)VoicePriority.MOVEMENT;
            clientController.SendPacket(voicePacket, MessageSendMode.Reliable);
        }
    }
}
