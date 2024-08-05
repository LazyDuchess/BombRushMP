using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reptile;
using HarmonyLib;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace BombRushMP.Plugin.Patches
{
    [HarmonyPatch(typeof(OptionsMenu))]
    public static class OptionsMenuPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(OptionsMenu.Awake))]
        private static void Awake_Postfix(OptionsMenu __instance)
        {
            PatchGameTab(__instance);
            PatchAudioTab(__instance);   
        }

        private static void PatchGameTab(OptionsMenu optionsMenu)
        {
            var gameTab = optionsMenu.GetComponentInChildren<OptionsMenuGameTab>();
            var invertAxis = gameTab.settingsContainer.transform.Find("GameTabSettingsInvertAxisContainer");
            var showNamePlates = GameObject.Instantiate(invertAxis.gameObject);
            showNamePlates.gameObject.name = "GameTabSettingsShowNamePlatesContainer";
            showNamePlates.transform.Find("MultiSelectionButton").GetComponentInChildren<TextMeshProUGUI>().text = "Show nameplates";
            showNamePlates.transform.SetParent(invertAxis.transform.parent, false);
            showNamePlates.transform.localPosition = new Vector3(showNamePlates.transform.localPosition.x, -10f, showNamePlates.transform.localPosition.z);
            GameObject.Destroy(showNamePlates.GetComponentInChildren<TMProLocalizationAddOn>());

            var multiOptionButton = showNamePlates.GetComponentInChildren<MultiOptionButton>();
            var mpSettings = MPSettings.Instance;
            multiOptionButton.SetLabelText(mpSettings.ShowNamePlates ? "On" : "Off");

            multiOptionButton.OnChanged += (index) => {
                var mpSettings = MPSettings.Instance;
                mpSettings.ShowNamePlates = !mpSettings.ShowNamePlates;
                mpSettings.Save();
                multiOptionButton.SetLabelText(mpSettings.ShowNamePlates ? "On" : "Off");
            };

            gameTab.textSelectables = gameTab.textSelectables.AddItem(multiOptionButton).ToArray();

            var languageButton = gameTab.settingsContainer.transform.Find("GameTabSettingsLanguageContainer").GetComponentInChildren<MultiOptionButton>();
            var applyLanguageButton = gameTab.settingsContainer.transform.Find("GameTabApplyLanguageButton").GetComponentInChildren<TextMeshProMenuButton>();

            var newNav = new Navigation();
            newNav.selectOnUp = languageButton;
            newNav.selectOnDown = applyLanguageButton;
            newNav.mode = Navigation.Mode.Explicit;
            multiOptionButton.navigation = newNav;

            newNav = languageButton.navigation;
            newNav.selectOnDown = multiOptionButton;
            languageButton.navigation = newNav;

            newNav = applyLanguageButton.navigation;
            newNav.selectOnUp = multiOptionButton;
            applyLanguageButton.navigation = newNav;
        }

        private static void PatchAudioTab(OptionsMenu optionsMenu)
        {
            var gameTab = optionsMenu.GetComponentInChildren<OptionsMenuGameTab>();
            var audioTab = optionsMenu.GetComponentInChildren<OptionsMenuAudioTab>();

            var invertAxis = gameTab.settingsContainer.transform.Find("GameTabSettingsInvertAxisContainer");
            var playerAudio = GameObject.Instantiate(invertAxis.gameObject);
            playerAudio.transform.SetParent(audioTab.settingsContainer.transform, false);
            playerAudio.RectTransform().anchorMin = new Vector2(0.0f, 0.69f);
            playerAudio.RectTransform().anchorMax = new Vector2(1f, 1f);
            playerAudio.transform.localPosition = new Vector3(playerAudio.transform.localPosition.x, -60f, playerAudio.transform.localPosition.z);
            playerAudio.transform.Find("MultiSelectionButton").GetComponentInChildren<TextMeshProUGUI>().text = "Player voices";
            playerAudio.gameObject.name = "AudioTabSettingsPlayerVoicesContainer";
            var optionsLabel = playerAudio.transform.Find("OptionsLabel").transform;
            optionsLabel.localPosition = new Vector3(117f, 0f, 0f);
            var multiOptionButton = playerAudio.GetComponentInChildren<MultiOptionButton>();
            GameObject.Destroy(playerAudio.GetComponentInChildren<TMProLocalizationAddOn>());

            var mpSettings = MPSettings.Instance;
            multiOptionButton.SetLabelText(mpSettings.PlayerAudioEnabled ? "On" : "Off");

            multiOptionButton.OnChanged += (index) => {
                var mpSettings = MPSettings.Instance;
                mpSettings.PlayerAudioEnabled = !mpSettings.PlayerAudioEnabled;
                mpSettings.Save();
                multiOptionButton.SetLabelText(mpSettings.PlayerAudioEnabled ? "On" : "Off");
            };

            var backButton = optionsMenu.transform.Find("OptionsMenuBottomButtonContainer").Find("BackButton").GetComponentInChildren<TextMeshProMenuButton>();
            var musicVolume = audioTab.settingsContainer.transform.Find("AudioTabMusicVolumeContainer").GetComponentInChildren<OptionsSliderButton>();

            var newNav = new Navigation();
            newNav.selectOnUp = musicVolume;
            newNav.selectOnDown = backButton;
            newNav.mode = Navigation.Mode.Explicit;
            multiOptionButton.navigation = newNav;

            newNav = musicVolume.navigation;
            newNav.selectOnDown = multiOptionButton;
            musicVolume.navigation = newNav;

            audioTab.lastSelectable = multiOptionButton.gameObject;
            audioTab.textSelectables = audioTab.textSelectables.AddItem(multiOptionButton).ToArray();
        }
    }
}
