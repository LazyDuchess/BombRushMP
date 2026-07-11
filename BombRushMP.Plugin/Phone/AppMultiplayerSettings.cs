using BombRushMP.Plugin;
using CommonAPI.Phone;
using Reptile;
using UnityEngine;
using BombRushMP.Common;
using System.Linq.Expressions;
using BombRushMP.Plugin.Gamemodes;

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

    public static void Initialize()
    {
        PhoneAPI.RegisterApp<AppMultiplayerDebug>("settings");
    }

    public override void OnAppInit()
    {
        base.OnAppInit();
        CreateIconlessTitleBar("Settings");
        ScrollView = PhoneScrollView.Create(this);
        PopulateButtons();
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

    private void PopulateButtons()
    {
        ScrollView.RemoveAllButtons();
        SimplePhoneButton button;
        button = PhoneUIUtility.CreateSimpleButton("PvP");
        button.OnConfirm += () =>
        {
            MPSettings.Instance.PvP = !MPSettings.Instance.PvP;
            UpdateLabels();
        };
        ScrollView.AddButton(button);
        _pvpButton = button;
        button = PhoneUIUtility.CreateSimpleButton("New Traffic");
        button.OnConfirm += () =>
        {
            MPSettings.Instance.FreeroamTraffic = !MPSettings.Instance.FreeroamTraffic;
            UpdateLabels();
        };
        ScrollView.AddButton(button);
        _trafficButton = button;
        button = PhoneUIUtility.CreateSimpleButton("Nameplates");
        button.OnConfirm += () =>
        {
            MPSettings.Instance.ShowNamePlates = !MPSettings.Instance.ShowNamePlates;
            UpdateLabels();
        };
        ScrollView.AddButton(button);
        _nameplatesButton = button;
        button = PhoneUIUtility.CreateSimpleButton("Minimap");
        button.OnConfirm += () =>
        {
            MPSettings.Instance.ShowMinimap = !MPSettings.Instance.ShowMinimap;
            UpdateLabels();
        };
        ScrollView.AddButton(button);
        _minimapButton = button;
        button = PhoneUIUtility.CreateSimpleButton("Chat");
        button.OnConfirm += () =>
        {
            MPSettings.Instance.ShowChat = !MPSettings.Instance.ShowChat;
            UpdateLabels();
        };
        ScrollView.AddButton(button);
        _chatButton = button;
        button = PhoneUIUtility.CreateSimpleButton("Voices");
        button.OnConfirm += () =>
        {
            MPSettings.Instance.PlayerAudioEnabled = !MPSettings.Instance.PlayerAudioEnabled;
            UpdateLabels();
        };
        ScrollView.AddButton(button);
        _voicesButton = button;
        button = PhoneUIUtility.CreateSimpleButton("Ragdoll");
        button.OnConfirm += () =>
        {
            MPSettings.Instance.RagdollOnHit = !MPSettings.Instance.RagdollOnHit;
            UpdateLabels();
        };
        ScrollView.AddButton(button);
        _ragdollButton = button;
        UpdateLabels();
    }

    public override void OnAppEnable()
    {
        UpdateLabels();
    }
}
