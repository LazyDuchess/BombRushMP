using BombRushMP.Plugin;
using CommonAPI.Phone;
using Reptile;
using UnityEngine;
using BombRushMP.Common;
using System.Linq.Expressions;
using BombRushMP.Plugin.Gamemodes;

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
        SimplePhoneButton button;
        button = PhoneUIUtility.CreateSimpleButton("Join Lobby");
        button.OnConfirm += () =>
        {
            MyPhone.OpenApp(typeof(AppJoinLobbyDebug));
        };
        ScrollView.AddButton(button);
        button = PhoneUIUtility.CreateSimpleButton("Forklift Skin");
        button.OnConfirm += () =>
        {
            var player = WorldHandler.instance.GetCurrentPlayer();
            SpecialSkinManager.Instance.ApplySpecialSkinToPlayer(player, SpecialSkins.Forklift);
        };
        ScrollView.AddButton(button);
        button = PhoneUIUtility.CreateSimpleButton("Sean Kingston Skin");
        button.OnConfirm += () =>
        {
            var player = WorldHandler.instance.GetCurrentPlayer();
            SpecialSkinManager.Instance.ApplySpecialSkinToPlayer(player, SpecialSkins.SeanKingston);
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

        button = PhoneUIUtility.CreateSimpleButton("Enable Prop Hunt Mode");
        button.OnConfirm += () =>
        {
            PropDisguiseController.Instance.FreezeProps();
        };
        ScrollView.AddButton(button);

        button = PhoneUIUtility.CreateSimpleButton("Disable Prop Hunt Mode");
        button.OnConfirm += () =>
        {
            PropDisguiseController.Instance.UnfreezeProps();
        };
        ScrollView.AddButton(button);

        button = PhoneUIUtility.CreateSimpleButton("Apply Nearest Disguise");
        button.OnConfirm += () =>
        {
            var player = WorldHandler.instance.GetCurrentPlayer();
            var propController = PropDisguiseController.Instance;
            var nearestDist = Mathf.Infinity;
            var nearest = 0;
            var nearestSet = false;
            foreach(var prop in propController.Props)
            {
                var dist = Vector3.Distance(player.transform.position, prop.Value.transform.position);
                if (!nearestSet)
                {
                    nearestDist = dist;
                    nearest = prop.Key;
                    nearestSet = true;
                }
                else
                {
                    if (dist < nearestDist)
                    {
                        nearestDist = dist;
                        nearest = prop.Key;
                    }
                }
            }
            if (!nearestSet) return;
            PlayerComponent.Get(player).ApplyPropDisguise(nearest);
        };
        ScrollView.AddButton(button);

        button = PhoneUIUtility.CreateSimpleButton("Remove Disguise");
        button.OnConfirm += () =>
        {
            PlayerComponent.GetLocal().RemovePropDisguise();
        };
        ScrollView.AddButton(button);

        button = PhoneUIUtility.CreateSimpleButton("Kill Yourself");
        button.OnConfirm += () =>
        {
            var player = WorldHandler.instance.GetCurrentPlayer();
            player.ChangeHP(99999);
        };
        ScrollView.AddButton(button);
    }
}
