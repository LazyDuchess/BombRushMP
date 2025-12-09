using BombRushMP.Common.Packets;
using HarmonyLib;
using Reptile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BombRushMP.Common;
using BombRushMP.Common.Networking;
using BombRushMP.Plugin.Gamemodes;

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
            var playerComp = PlayerComponent.Get(__instance.p);
            if (playerComp != null && playerComp.HasPropDisguise) return;
            var clientController = ClientController.Instance;
            if (clientController == null) return;
            if (!clientController.Connected) return;
            var voicePacket = new PlayerVoice();
            voicePacket.AudioClipId = (int)AudioClipID.VoiceJump;
            voicePacket.VoicePriority = (int)VoicePriority.MOVEMENT;
            clientController.SendPacket(voicePacket, IMessage.SendModes.ReliableUnordered, NetChannels.VisualUpdates);
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(GrindAbility.SetToLine))]
        private static bool SetToLine_Postfix(GrindAbility __instance)
        {
            if (__instance.p.isAI) return true;
            var propDisguiseController = PropDisguiseController.Instance;
            if (propDisguiseController.LocalPropHuntTeam == PropHuntTeams.Props && propDisguiseController.InPropHunt) return false;
            var clientController = ClientController.Instance;
            if (clientController == null) return true;
            if (!clientController.Connected) return true;
            clientController.SendGenericEvent(GenericEvents.Teleport, IMessage.SendModes.ReliableUnordered);
            return true;
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
            clientController.SendGenericEvent(GenericEvents.Teleport, IMessage.SendModes.ReliableUnordered);
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
