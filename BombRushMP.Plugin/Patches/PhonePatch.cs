using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Reptile.Phone;
using System.Reflection.Emit;
using BombRushMP.Plugin.Phone;

namespace BombRushMP.Plugin.Patches
{
    [HarmonyPatch(typeof(Reptile.Phone.Phone))]
    internal static class PhonePatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(Reptile.Phone.Phone.OpenCloseHandeling))]
        private static bool OpenCloseHandeling_Prefix(Reptile.Phone.Phone __instance, ref bool __state)
        {
            __state = __instance.state == Reptile.Phone.Phone.PhoneState.BOOTINGUP;
            if (SpectatorController.Instance != null)
                return false;
            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(Reptile.Phone.Phone.OpenCloseHandeling))]
        private static void OpenCloseHandeling_Postfix(Reptile.Phone.Phone __instance, ref bool __state)
        {
            if (!__state && __instance.state == Reptile.Phone.Phone.PhoneState.BOOTINGUP && __instance.gameInput.GetButtonNew(21, 0))
            {
                PhoneUtility.BackToHomescreen(__instance);
                __instance.OpenApp(typeof(AppMultiplayer));
            }
        }
    }
}
