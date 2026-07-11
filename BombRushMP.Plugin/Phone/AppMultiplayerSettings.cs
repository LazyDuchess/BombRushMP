using BombRushMP.Plugin;
using CommonAPI.Phone;
using Reptile;
using UnityEngine;
using BombRushMP.Common;
using System.Linq.Expressions;
using BombRushMP.Plugin.Gamemodes;
using System.Collections.Generic;
using BombRushMP.Mono.Runtime;

public class AppMultiplayerSettings : CustomApp
{
    public override bool Available => false;
    private SimplePhoneButton _pvpButton;
    private SimplePhoneButton _trafficButton;
    private SimplePhoneButton _nameplatesButton;
    private SimplePhoneButton _minimapButton;
    private SimplePhoneButton _chatButton;
    private SimplePhoneButton _voicesButton;
    private SimplePhoneButton _ragdollButton;
    private SimplePhoneButton _themeButton;
    private SimplePhoneButton _nameButton;
    private SimplePhoneButton _crewNameButton;
    private const int DescriptionStatusPriority = 0;
    private const float DescriptionStatusTime = 0.5f;
    private Dictionary<PhoneButton, string> _buttonDescriptions = new();

    public static void Initialize()
    {
        PhoneAPI.RegisterApp<AppMultiplayerSettings>("settings");
    }

    public override void OnAppInit()
    {
        base.OnAppInit();
        CreateIconlessTitleBar("Settings");
        ScrollView = PhoneScrollView.Create(this);
        PopulateButtons();
    }

    private void ShowDescription(string text)
    {
        var statusUI = StatusTextUI.Instance;
        statusUI.TryShowStatus(DescriptionStatusPriority, text, DescriptionStatusTime);
    }

    private void SetButtonDescription(PhoneButton button, string desc)
    {
        _buttonDescriptions[button] = desc;
    }

    public override void OnAppUpdate()
    {
        var currentButton = ScrollView.Buttons[ScrollView.SelectedIndex];
        if (currentButton != null)
        {
            if (currentButton == _themeButton)
            {
                UpdateDescriptions();
            }
            if (_buttonDescriptions.TryGetValue(currentButton, out var desc))
            {
                ShowDescription(desc);
            }
            else
            {
                ShowDescription("");
            }
        }
    }

    private void UpdateLabels()
    {
        _pvpButton.Label.text = $"PvP = {(MPSettings.Instance.PvP ? "ON" : "OFF")}";
        _trafficButton.Label.text = $"New Traffic = {(MPSettings.Instance.FreeroamTraffic ? "ON" : "OFF")}";
        _nameplatesButton.Label.text = $"Nameplates = {(MPSettings.Instance.ShowNamePlates ? "ON" : "OFF")}";
        _voicesButton.Label.text = $"Voices = {(MPSettings.Instance.PlayerAudioEnabled ? "ON" : "OFF")}";
        _minimapButton.Label.text = $"Minimap = {(MPSettings.Instance.ShowMinimap ? "ON" : "OFF")}";
        _chatButton.Label.text = $"Chat = {(MPSettings.Instance.ShowChat ? "ON" : "OFF")}";
        _ragdollButton.Label.text = $"Ragdoll = {(MPSettings.Instance.RagdollOnHit ? "ON" : "OFF")}";
    }

    private void UpdateDescriptions()
    {
        SetButtonDescription(_themeButton, $"Pick a theme for the UI. Changes apply on map load! Current Theme: {MPSettings.Instance.Theme}");
        SetButtonDescription(_nameButton, $"Change your player name. You might have to reload the map to apply. Current Name: {MPSettings.Instance.PlayerName}");
        SetButtonDescription(_crewNameButton, $"Change your crew name. Used for team gamemodes, share it with your crew! Current Crew Name: {MPSettings.Instance.CrewName}");
    }

    private void PopulateButtons()
    {
        _buttonDescriptions.Clear();
        ScrollView.RemoveAllButtons();
        SimplePhoneButton button;
        button = PhoneUIUtility.CreateSimpleButton("Player Name");
        button.OnConfirm += () =>
        {
            TextInput.Instance.ShowOkCancel((text) =>
            {
                MPSettings.Instance.PlayerName = text;
                UpdateDescriptions();
            }, (text) =>
            {

            }, (text) =>
            {
                return true;
            }, "Enter your player name.", Constants.MaxNameLength, "Player Name...", MPSettings.Instance.PlayerName);
        };
        ScrollView.AddButton(button);
        _nameButton = button;
        button = PhoneUIUtility.CreateSimpleButton("Crew Name");
        button.OnConfirm += () =>
        {
            TextInput.Instance.ShowOkCancel((text) =>
            {
                MPSettings.Instance.CrewName = text;
                UpdateDescriptions();
            }, (text) =>
            {

            }, (text) =>
            {
                return true;
            }, "Enter your player name.", Constants.MaxCrewNameLength, "Crew Name...", MPSettings.Instance.CrewName);
        };
        ScrollView.AddButton(button);
        _crewNameButton = button;
        button = PhoneUIUtility.CreateSimpleButton("PvP");
        button.OnConfirm += () =>
        {
            MPSettings.Instance.PvP = !MPSettings.Instance.PvP;
            UpdateLabels();
        };
        ScrollView.AddButton(button);
        SetButtonDescription(button, "When enabled, you will be able to fight with players who also have this option enabled. Only works when not in a lobby.");
        _pvpButton = button;
        button = PhoneUIUtility.CreateSimpleButton("New Traffic");
        button.OnConfirm += () =>
        {
            MPSettings.Instance.FreeroamTraffic = !MPSettings.Instance.FreeroamTraffic;
            UpdateLabels();
        };
        ScrollView.AddButton(button);
        SetButtonDescription(button, "When enabled, traffic will be able to hit you more easily. Only works when not in a lobby.");
        _trafficButton = button;
        button = PhoneUIUtility.CreateSimpleButton("Nameplates");
        button.OnConfirm += () =>
        {
            MPSettings.Instance.ShowNamePlates = !MPSettings.Instance.ShowNamePlates;
            UpdateLabels();
        };
        ScrollView.AddButton(button);
        SetButtonDescription(button, "Display names above players.");
        _nameplatesButton = button;
        button = PhoneUIUtility.CreateSimpleButton("Minimap");
        button.OnConfirm += () =>
        {
            MPSettings.Instance.ShowMinimap = !MPSettings.Instance.ShowMinimap;
            UpdateLabels();
        };
        ScrollView.AddButton(button);
        SetButtonDescription(button, "Show the minimap in the bottom left corner.");
        _minimapButton = button;
        button = PhoneUIUtility.CreateSimpleButton("Chat");
        button.OnConfirm += () =>
        {
            MPSettings.Instance.ShowChat = !MPSettings.Instance.ShowChat;
            UpdateLabels();
        };
        ScrollView.AddButton(button);
        SetButtonDescription(button, "Show the chat window.");
        _chatButton = button;
        button = PhoneUIUtility.CreateSimpleButton("Voices");
        button.OnConfirm += () =>
        {
            MPSettings.Instance.PlayerAudioEnabled = !MPSettings.Instance.PlayerAudioEnabled;
            UpdateLabels();
        };
        ScrollView.AddButton(button);
        SetButtonDescription(button, "Hear other players' voices.");
        _voicesButton = button;
        button = PhoneUIUtility.CreateSimpleButton("Ragdoll");
        button.OnConfirm += () =>
        {
            MPSettings.Instance.RagdollOnHit = !MPSettings.Instance.RagdollOnHit;
            UpdateLabels();
        };
        ScrollView.AddButton(button);
        SetButtonDescription(button, "Activate ragdoll when hit in-game.");
        _ragdollButton = button;
        button = PhoneUIUtility.CreateSimpleButton("Theme");
        button.OnConfirm += () =>
        {
            MyPhone.OpenApp(typeof(AppMultiplayerThemes));
        };
        ScrollView.AddButton(button);
        _themeButton = button;
        UpdateLabels();
        UpdateDescriptions();
    }

    public override void OnAppEnable()
    {
        UpdateLabels();
        UpdateDescriptions();
    }
}
