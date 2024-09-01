using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reptile;
using HarmonyLib;
using UnityEngine;

namespace BombRushMP.Plugin.Patches
{
    [HarmonyPatch(typeof(StyleSwitchMenu))]
    internal static class StyleSwitchMenuPatch
    {
        private static List<MenuTimelineButton> ExtraButtons = new();
        private static List<TMProLocalizationAddOn> ExtraTexts = new();

        [HarmonyPostfix]
        [HarmonyPatch(nameof(StyleSwitchMenu.Init))]
        private static void Init_Postfix(StyleSwitchMenu __instance)
        {
            ExtraButtons = new();
            ExtraTexts = new();
            var parent = __instance.firstButton.transform.parent;
            var gameObject = UnityEngine.Object.Instantiate<GameObject>(__instance.firstButton, parent);
            MenuTimelineButton component = gameObject.GetComponent<MenuTimelineButton>();
            TMProLocalizationAddOn componentInChildren = gameObject.GetComponentInChildren<TMProLocalizationAddOn>();
            ExtraButtons.Add(component);
            ExtraTexts.Add(componentInChildren);
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(StyleSwitchMenu.Activate))]
        private static void Activate_Postfix(StyleSwitchMenu __instance)
        {
            if (ExtraButtons.Count <= 0) return;

            var lastNavi = __instance.buttons[__instance.buttons.Length - 1].navigation;
            lastNavi.selectOnDown = ExtraButtons[0];
            __instance.buttons[__instance.buttons.Length - 1].navigation = lastNavi;

            for (var i = 0; i < ExtraButtons.Count; i++)
            {
                var unlocked = false;
                var button = ExtraButtons[i];
                var text = ExtraTexts[i];
                text.AssignAndUpdateText("Freesoul", GroupOptions.Text, Array.Empty<string>());
                var buttonUp = __instance.buttons[__instance.buttons.Length - 1];
                if (i > 0)
                {
                    buttonUp = ExtraButtons[i - 1];
                }
                var buttonDown = __instance.cancelButton;
                if (i < ExtraButtons.Count - 1)
                {
                    buttonDown = ExtraButtons[i + 1];
                }
                button.SetButtonVariables(null, unlocked, buttonUp, buttonDown, __instance.normalGameFontType, __instance.selectedGameFontType, __instance.nonSelectableAlphaValue);
            }
            var cancelNavi = __instance.cancelButton.navigation;
            cancelNavi.selectOnUp = ExtraButtons[ExtraButtons.Count - 1];
            __instance.cancelButton.navigation = cancelNavi;
        }
    }
}
