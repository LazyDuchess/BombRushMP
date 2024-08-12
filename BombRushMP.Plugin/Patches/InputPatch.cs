using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BombRushMP.Plugin.Patches
{
    internal static class InputPatch
    {
        private static MethodInfo GetKeyDown_Base_KeyCodeOverload = AccessTools.Method(typeof(Input), "GetKeyDown", [typeof(KeyCode)]);
        private static MethodInfo GetKeyDown_Base_StringOverload = AccessTools.Method(typeof(Input), "GetKeyDown", [typeof(string)]);
        private static MethodInfo GetKeyDown_Prefix_Method = AccessTools.Method(typeof(InputPatch), "GetKeyDown_Prefix");
        internal static void Patch(Harmony harmony)
        {
            var getKeyDownPrefixMethod = new HarmonyMethod(GetKeyDown_Prefix_Method);
            harmony.Patch(GetKeyDown_Base_KeyCodeOverload, getKeyDownPrefixMethod, null, null, null, null);
            harmony.Patch(GetKeyDown_Base_StringOverload, getKeyDownPrefixMethod, null, null, null, null);
        }
        private static bool GetKeyDown_Prefix(ref bool __result)
        {
            if (InputUtils.InputBlocked)
            {
                __result = false;
                return false;
            }
            return true;
        }
    }
}
