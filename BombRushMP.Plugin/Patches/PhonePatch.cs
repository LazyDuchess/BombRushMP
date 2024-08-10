using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Reptile.Phone;
using System.Reflection.Emit;

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

        [HarmonyTranspiler]
        [HarmonyPatch(nameof(Reptile.Phone.Phone.OpenCloseHandeling))]
        private static IEnumerable<CodeInstruction> OpenCloseHandeling_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var pastCloseHandling = false;
            foreach (var instruction in instructions)
            {
                if (instruction.LoadsConstant(57) && !pastCloseHandling)
                {
                    pastCloseHandling = true;
                    yield return instruction;
                    continue;
                }
                if (instruction.LoadsConstant(21) || instruction.LoadsConstant(29) || instruction.LoadsConstant(57))
                {
                    yield return new CodeInstruction(OpCodes.Ldc_I4_S, 21);
                }
                else
                    yield return instruction;
            }
        }
    }
}
