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
    }

    private void PopulateButtons()
    {
        ScrollView.RemoveAllButtons();
        SimplePhoneButton button;
        button = PhoneUIUtility.CreateSimpleButton("PvP");
        button.OnConfirm += () =>
        {
            MPSettings.Instance.PvP = !MPSettings.Instance.PvP;
        };
        ScrollView.AddButton(button);
        _pvpButton = button;
        button = PhoneUIUtility.CreateSimpleButton("New Traffic");
        button.OnConfirm += () =>
        {
            MPSettings.Instance.FreeroamTraffic = !MPSettings.Instance.FreeroamTraffic;
        };
        ScrollView.AddButton(button);
        _trafficButton = button;
        UpdateLabels();
    }

    public override void OnAppEnable()
    {
        UpdateLabels();
    }
}
