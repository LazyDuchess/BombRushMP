using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Reptile.Phone;

namespace BombRushMP.Plugin.Patches
{
    [HarmonyPatch(typeof(Reptile.Phone.Phone))]
    internal static class PhonePatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(Reptile.Phone.Phone.OpenCloseHandeling))]
        private static bool OpenCloseHandeling_Prefix()
        {
            if (SpectatorController.Instance != null)
                return false;
            return true;
        }
    }
}
