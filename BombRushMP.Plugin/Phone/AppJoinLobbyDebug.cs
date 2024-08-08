using BombRushMP.Plugin;
using CommonAPI.Phone;

public class AppJoinLobbyDebug : CustomApp
{
    public override bool Available => false;
    public static void Initialize()
    {
        PhoneAPI.RegisterApp<AppJoinLobbyDebug>("debug join lobby");
    }

    public override void OnAppInit()
    {
        base.OnAppInit();
        CreateIconlessTitleBar("Lobbies");
        ScrollView = PhoneScrollView.Create(this);
    }

    public override void OnAppEnable()
    {
        base.OnAppEnable();
        PopulateButtons();
    }

    private void PopulateButtons()
    {
        ScrollView.RemoveAllButtons();
        var clientController = ClientController.Instance;
        var lobbies = clientController.ClientLobbyManager.Lobbies;
        foreach(var lobby in lobbies)
        {
            var host = clientController.Players[lobby.Value.LobbyState.HostId];
            var button = PhoneUIUtility.CreateSimpleButton($"{host.ClientState.Name} ({lobby.Value.LobbyState.Players.Count})");
            button.OnConfirm += () =>
            {
                clientController.ClientLobbyManager.JoinLobby(lobby.Key);
            };
            ScrollView.AddButton(button);

        }
    }
}
