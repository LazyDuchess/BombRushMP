using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reptile;
using HarmonyLib;
using UnityEngine;
using ch.sycoforge.Decal;
using UnityEngine.Events;
using BombRushMP.CrewBoom;
using MonoMod.Cil;

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
            var player = WorldHandler.instance.GetCurrentPlayer();
            var charData = MPSaveData.Instance.GetCharacterData(player.character);
            var user = Core.Instance.Platform.User;
            if (charData.MPMoveStyleSkin != -1)
            {
                for (var i=0;i<__instance.texts.Length; i++)
                {
                    var unlockableSaveDataFor = user.GetUnlockableSaveDataFor(__instance.currentUnlockables[i]);
                    if (!unlockableSaveDataFor.isUnlocked)
                        __instance.texts[i].AssignAndUpdateText("???", GroupOptions.Text);
                    else
                        __instance.texts[i].AssignAndUpdateTextWithTags(__instance.currentUnlockables[i].Title, GroupOptions.Skin, null, null);
                }
            }
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
                if (button != null && button.gameObject != null)
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
                string tag = null;
                if (skin.Identifier == charData.MPMoveStyleSkin)
                    tag = "<u>";
                text.AssignAndUpdateTextWithTags(skin.Title, GroupOptions.Text, tag, null);
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
                button.interactable = true;
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(delegate
                {
                    CustomSkinButtonClicked(__instance, button, skin);
                });
                button.gameObject.SetActive(true);
            }
            var cancelNavi = __instance.cancelButton.navigation;
            cancelNavi.selectOnUp = ExtraButtons[ExtraButtons.Count - 1];
            __instance.cancelButton.navigation = cancelNavi;
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(StyleSwitchMenu.SkinButtonClicked))]
        private static void SkinButtonClicked_Prefix(StyleSwitchMenu __instance, MenuTimelineButton clickedButton, int skinIndex)
        {
            if (__instance.IsTransitioning) return;
            if (__instance.buttonClicked != null) return;
            if (!clickedButton.canBeSelected) return;
            var currentPlayer = WorldHandler.instance.GetCurrentPlayer();
            PlayerComponent.Get(currentPlayer).ApplyMoveStyleSkin(0);
            MPSaveData.Instance.GetCharacterData(currentPlayer.character).MPMoveStyleSkin = -1;
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(StyleSwitchMenu.SkinButtonSelected))]
        private static void SkinButtonSelected_Postfix(StyleSwitchMenu __instance, MenuTimelineButton clickedButton, int skinIndex)
        {
            _descriptionText.AssignAndUpdateText("MOVESTYLE_DESCRIPTION", GroupOptions.Text);
            if (__instance == null) return;
            if (__instance.moveStyleType != MoveStyle.SKATEBOARD) return;
            var mesh = WorldHandler.instance.GetCurrentPlayer().MoveStylePropsPrefabs.skateboard.GetComponent<MeshFilter>().sharedMesh;
            foreach (var previewMesh in _skateboardPreviewMeshes)
                previewMesh.sharedMesh = mesh;
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(StyleSwitchMenu.SkinButtonClicked))]
        private static void SkinButtonClicked_Prefix(StyleSwitchMenu __instance)
        {
            var playerComp = PlayerComponent.Get(WorldHandler.instance.GetCurrentPlayer());
            playerComp.ApplyMoveStyleSkin(0);
            __instance.moveStyleMaterials = MoveStyleLoader.GetMoveStyleMaterials(WorldHandler.instance.GetCurrentPlayer(), __instance.moveStyleType);
        }

        private static void CustomSkinButtonClicked(StyleSwitchMenu menu, MenuTimelineButton clickedButton, MPMoveStyleSkin skin)
        {
            if (!menu.IsTransitioning && menu.buttonClicked == null)
            {
                if (!clickedButton.canBeSelected)
                {
                    if (menu.travelButtonCoroutine == null)
                    {
                        menu.travelButtonCoroutine = menu.StartCoroutine(menu.CanNotPerformButtonActionAnimation(clickedButton));
                        return;
                    }
                }
                else
                {
                    menu.buttonClicked = clickedButton;
                    Core instance = Core.Instance;
                    instance.AudioManager.PlaySfxUI(SfxCollectionID.MenuSfx, AudioClipID.confirm, 0f);
                    var currentPlayer = WorldHandler.instance.GetCurrentPlayer();
                    MPSaveData.Instance.GetCharacterData(currentPlayer.character).MPMoveStyleSkin = skin.Identifier;
                    Core.Instance.SaveManager.SaveCurrentSaveSlot();

                    skin.ApplyToPlayer(currentPlayer);
                    menu.StartCoroutine(menu.clipBehaviour.FlickerDelayedButtonPress(clickedButton, new UnityAction(menu.clipBehaviour.ExitMenu)));
                }
            }
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
