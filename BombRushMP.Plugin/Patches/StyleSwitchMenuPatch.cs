using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reptile;
using HarmonyLib;
using UnityEngine;
using ch.sycoforge.Decal;

namespace BombRushMP.Plugin.Patches
{
    [HarmonyPatch(typeof(StyleSwitchMenu))]
    internal static class StyleSwitchMenuPatch
    {
        private static List<MenuTimelineButton> ExtraButtons = new();
        private static List<TMProLocalizationAddOn> ExtraTexts = new();
        private static List<MPMoveStyleSkin> ExtraButtonSkins = new();
        private static MeshFilter[] _skateboardPreviewMeshes = null;
        private static TMProLocalizationAddOn _descriptionText = null;

        [HarmonyPostfix]
        [HarmonyPatch(nameof(StyleSwitchMenu.Activate))]
        private static void Activate_Postfix(StyleSwitchMenu __instance)
        {
            _descriptionText = __instance.transform.Find("DescriptionText").GetComponent<TMProLocalizationAddOn>();
            var resetTextures = GameObject.FindObjectsOfType<StyleSwitchResetTexture>();
            foreach(var resetTexture in resetTextures)
            {
                if (resetTexture.moveStyle == MoveStyle.SKATEBOARD)
                {
                    _skateboardPreviewMeshes = resetTexture.GetComponentsInChildren<MeshFilter>();
                    break;
                }
            }
            foreach(var button in ExtraButtons)
            {
                GameObject.Destroy(button.gameObject);
            }
            ExtraButtons.Clear();
            ExtraTexts.Clear();
            ExtraButtonSkins.Clear();
            var unlockableManager = MPUnlockManager.Instance;
            foreach(var unlock in unlockableManager.UnlockByID.Values)
            {
                var skin = unlock as MPMoveStyleSkin;
                if (skin == null) continue;
                if (skin.MoveStyle != __instance.moveStyleType) continue;
                ExtraButtonSkins.Add(skin);
            }
            var parent = __instance.firstButton.transform.parent;
            foreach (var skin in ExtraButtonSkins)
            {
                var gameObject = UnityEngine.Object.Instantiate<GameObject>(__instance.firstButton, parent);
                MenuTimelineButton component = gameObject.GetComponent<MenuTimelineButton>();
                TMProLocalizationAddOn componentInChildren = gameObject.GetComponentInChildren<TMProLocalizationAddOn>();
                ExtraButtons.Add(component);
                ExtraTexts.Add(componentInChildren);
            }
            if (ExtraButtons.Count <= 0) return;

            var lastNavi = __instance.buttons[__instance.buttons.Length - 1].navigation;
            lastNavi.selectOnDown = ExtraButtons[0];
            __instance.buttons[__instance.buttons.Length - 1].navigation = lastNavi;

            for (var i = 0; i < ExtraButtons.Count; i++)
            {
                var button = ExtraButtons[i];
                var text = ExtraTexts[i];
                var skin = ExtraButtonSkins[i];
                text.AssignAndUpdateText(skin.Title, GroupOptions.Text);
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
                button.SetButtonVariables(() =>
                {
                    CustomSkinButtonSelected(__instance, button, skin);
                }, skin.UnlockedByDefault, buttonUp, buttonDown, __instance.normalGameFontType, __instance.selectedGameFontType, __instance.nonSelectableAlphaValue);
            }
            var cancelNavi = __instance.cancelButton.navigation;
            cancelNavi.selectOnUp = ExtraButtons[ExtraButtons.Count - 1];
            __instance.cancelButton.navigation = cancelNavi;
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(StyleSwitchMenu.SkinButtonSelected))]
        private static void SkinButtonSelected_Postfix(MenuTimelineButton clickedButton, int skinIndex)
        {
            var styleSwitchMenu = clickedButton.GetComponentInParent<StyleSwitchMenu>();
            _descriptionText.AssignAndUpdateText("MOVESTYLE_DESCRIPTION", GroupOptions.Text);
            if (styleSwitchMenu == null) return;
            if (styleSwitchMenu.moveStyleType != MoveStyle.SKATEBOARD) return;
            var mesh = WorldHandler.instance.GetCurrentPlayer().MoveStylePropsPrefabs.skateboard.GetComponent<MeshFilter>().sharedMesh;
            foreach (var previewMesh in _skateboardPreviewMeshes)
                previewMesh.sharedMesh = mesh;
        }

        private static void CustomSkinButtonSelected(StyleSwitchMenu menu, MenuTimelineButton button, MPMoveStyleSkin skin)
        {
            if (menu.IsTransitioning || menu.buttonClicked != null)
            {
                return;
            }

            if (!string.IsNullOrEmpty(skin.HowToUnlock))
            {
                _descriptionText.AssignAndUpdateText($"To unlock: {skin.HowToUnlock}", GroupOptions.Text);
            }
            else
                _descriptionText.AssignAndUpdateText("MOVESTYLE_DESCRIPTION", GroupOptions.Text);

            if (menu.previewMaterials != null)
            {
                foreach (var previewMaterial in menu.previewMaterials)
                {
                    for (var i = 0; i < menu.moveStyleMaterials.Length; i++)
                    {
                        menu.previewMaterials[i].mainTexture = skin.Texture;
                    }
                }
            }
            var deckSkin = skin as MPSkateboardSkin;
            if (deckSkin != null)
            {
                var mesh = deckSkin.SkateboardMesh;
                if (mesh == null)
                    mesh = WorldHandler.instance.GetCurrentPlayer().MoveStylePropsPrefabs.skateboard.GetComponent<MeshFilter>().sharedMesh;
                foreach(var previewMesh in _skateboardPreviewMeshes)
                    previewMesh.sharedMesh = mesh;
            }
        }
    }
}
