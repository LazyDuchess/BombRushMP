using HarmonyLib;
using Reptile;
using BombRushMP.Common;
using BombRushMP.Common.Networking;

namespace BombRushMP.Plugin.Patches
{
    [HarmonyPatch(typeof(DieAbility))]
    internal static class DieAbilityPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(DieAbility.OnStartAbility))]
        private static void OnStartAbility_Postfix(DieAbility __instance)
        {
            if (__instance.p.isAI) return;
            ClientController.Instance.SendGenericEvent(GenericEvents.Death, IMessage.SendModes.ReliableUnordered);
        }
    }
}
