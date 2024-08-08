using BombRushMP.Plugin;
using BombRushMP.Plugin.Phone;
using CommonAPI.Phone;

public class AppMultiplayer : CustomApp
{
    private PhoneButton _createLobbyButton;
    private PhoneButton _lobbyButton;
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

    public override void OnAppUpdate()
    {
        base.OnAppUpdate();
        var lobbyManager = ClientController.Instance.ClientLobbyManager;
        if (lobbyManager.CurrentLobby != null)
        {
            if (_lobbyButton == null)
                MakeLobbyButton();
            if (_createLobbyButton != null)
            {
                ScrollView.RemoveButton(_createLobbyButton);
                _createLobbyButton = null;
            }
        }
        else
        {
            if (_createLobbyButton == null)
                MakeCreateLobbyButton();
            if (_lobbyButton != null)
            {
                ScrollView.RemoveButton(_lobbyButton);
                _lobbyButton = null;
            }
        }
    }

    private void MakeCreateLobbyButton()
    {
        var lobbyManager = ClientController.Instance.ClientLobbyManager;
        _createLobbyButton = PhoneUIUtility.CreateSimpleButton("Create Lobby");
        _createLobbyButton.OnConfirm += () =>
        {
            lobbyManager.CreateLobby();
        };
        ScrollView.InsertButton(0, _createLobbyButton);
    }

    private void MakeLobbyButton()
    {
        _lobbyButton = PhoneUIUtility.CreateSimpleButton("Lobby");
        _lobbyButton.OnConfirm += () =>
        {
            MyPhone.OpenApp(typeof(AppMultiplayerCurrentLobby));
        };
        ScrollView.InsertButton(0, _lobbyButton);
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
