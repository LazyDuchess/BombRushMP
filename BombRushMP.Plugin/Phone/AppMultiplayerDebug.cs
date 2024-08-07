using BombRushMP.Plugin;
using CommonAPI.Phone;

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

        var button = PhoneUIUtility.CreateSimpleButton("Create Lobby");
        button.OnConfirm += () =>
        {
            ClientController.Instance.ClientLobbyManager.CreateLobby();
        };
        ScrollView.AddButton(button);

        button = PhoneUIUtility.CreateSimpleButton("Join Lobby");
        button.OnConfirm += () =>
        {
            MyPhone.OpenApp(typeof(AppJoinLobbyDebug));
        };
        ScrollView.AddButton(button);
    }
}
