using BombRushMP.Plugin;
using CommonAPI.Phone;
using Reptile;
using UnityEngine;
using BombRushMP.Common;

public class AppMultiplayerDebug : CustomApp
{
    public override bool Available => false;
    public static void Initialize()
    {
        PhoneAPI.RegisterApp<AppMultiplayerDebug>("multiplayer debug");
    }

    public override void OnAppInit()
    {
        base.OnAppInit();
        CreateIconlessTitleBar("Debug");
        ScrollView = PhoneScrollView.Create(this);
        PopulateButtons();
    }

    private void PopulateButtons()
    {
        ScrollView.RemoveAllButtons();

        var button = PhoneUIUtility.CreateSimpleButton("Join Lobby");
        button.OnConfirm += () =>
        {
            MyPhone.OpenApp(typeof(AppJoinLobbyDebug));
        };
        ScrollView.AddButton(button);

        button = PhoneUIUtility.CreateSimpleButton("Cop Skin Test");
        button.OnConfirm += () =>
        {
            var player = WorldHandler.instance.GetCurrentPlayer();
            var gender = UnityEngine.Random.Range(0, 2);

            if (gender == 0)
                SpecialSkinManager.Instance.ApplySpecialSkinToPlayer(player, SpecialSkins.FemaleCop);
            else
                SpecialSkinManager.Instance.ApplySpecialSkinToPlayer(player, SpecialSkins.MaleCop);

            SpecialSkinManager.Instance.ApplyRandomVariantToPlayer(player);
        };
        ScrollView.AddButton(button);

        button = PhoneUIUtility.CreateSimpleButton("Remove Skin");
        button.OnConfirm += () =>
        {
            var player = WorldHandler.instance.GetCurrentPlayer();
            SpecialSkinManager.Instance.RemoveSpecialSkinFromPlayer(player);
        };
        ScrollView.AddButton(button);

        button = PhoneUIUtility.CreateSimpleButton("Enable ProSkater Mode");
        button.OnConfirm += () =>
        {
            var player = WorldHandler.instance.GetCurrentPlayer();
            ProSkaterPlayer.Set(player, true);
        };
        ScrollView.AddButton(button);

        button = PhoneUIUtility.CreateSimpleButton("Disable ProSkater Mode");
        button.OnConfirm += () =>
        {
            var player = WorldHandler.instance.GetCurrentPlayer();
            ProSkaterPlayer.Set(player, false);
        };
        ScrollView.AddButton(button);

        button = PhoneUIUtility.CreateSimpleButton("Enter AFK State");
        button.OnConfirm += () =>
        {
            var player = WorldHandler.instance.GetCurrentPlayer();
            PlayerComponent.Get(player).ForceAFK();
        };
        ScrollView.AddButton(button);

        button = PhoneUIUtility.CreateSimpleButton("Exit AFK State");
        button.OnConfirm += () =>
        {
            var player = WorldHandler.instance.GetCurrentPlayer();
            PlayerComponent.Get(player).StopAFK();
        };
        ScrollView.AddButton(button);
    }
}
