using BombRushMP.Plugin;
using CommonAPI.Phone;

public class AppMultiplayer : CustomApp
{
    public static void Initialize()
    {
        PhoneAPI.RegisterApp<AppMultiplayer>("multiplayer");
    }

    public override void OnAppInit()
    {
        base.OnAppInit();
        CreateIconlessTitleBar("Multiplayer", 70f);
        ScrollView = PhoneScrollView.Create(this);
        PopulateButtons();
    }

    private void PopulateButtons()
    {
        ScrollView.RemoveAllButtons();

        var button = PhoneUIUtility.CreateSimpleButton("Spectate");
        button.OnConfirm += () =>
        {
            SpectatorController.StartSpectating();
        };
        ScrollView.AddButton(button);

        button = PhoneUIUtility.CreateSimpleButton("Debug");
        button.OnConfirm += () =>
        {
            MyPhone.OpenApp(typeof(AppMultiplayerDebug));
        };
        ScrollView.AddButton(button);
    }
}
