using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BombRushMP.Common.Packets;
using HarmonyLib;
using Reptile;
using Riptide;

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
    }
}
