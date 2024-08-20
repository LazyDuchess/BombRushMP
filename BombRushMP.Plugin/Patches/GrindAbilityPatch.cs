using BombRushMP.Common.Packets;
using HarmonyLib;
using Reptile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Riptide;
using BombRushMP.Common;

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

        [HarmonyPostfix]
        [HarmonyPatch(nameof(GrindAbility.SetToLine))]
        private static void SetToLine_Postfix(GrindAbility __instance)
        {
            if (__instance.p.isAI) return;
            var clientController = ClientController.Instance;
            if (clientController == null) return;
            if (!clientController.Connected) return;
            clientController.SendGenericEvent(GenericEvents.Teleport, MessageSendMode.Reliable);
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(GrindAbility.OnStopAbility))]
        private static void OnStopAbility_Postfix(GrindAbility __instance)
        {
            if (__instance.p.isAI) return;
            var proSkater = ProSkaterPlayer.Get(__instance.p);
            if (proSkater != null)
            {
                if (__instance.p.abilityTimer <= ProSkaterPlayer.GrindPenaltyTime)
                    proSkater.PenalizeGrinding();
                else
                    proSkater.SoftResetGrinding();
                proSkater.GrindBalance.CurrentSensitivity += ProSkaterPlayer.LeaveGrindSensitivity;
            }
            var clientController = ClientController.Instance;
            if (clientController == null) return;
            if (!clientController.Connected) return;
            clientController.SendGenericEvent(GenericEvents.Teleport, MessageSendMode.Reliable);
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(GrindAbility.UpdateTilting))]
        private static bool UpdateTilting_Prefix(GrindAbility __instance)
        {
            if (__instance.p.isAI) return true;
            var proSkater = ProSkaterPlayer.Get(__instance.p);
            if (proSkater == null) return true;
            proSkater.GrindUpdateTilting(__instance);
            return false;
        }
    }
}
