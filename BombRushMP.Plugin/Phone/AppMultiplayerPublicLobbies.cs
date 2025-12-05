using BombRushMP.Plugin;
using BombRushMP.Plugin.Gamemodes;
using CommonAPI.Phone;
using System.ComponentModel;

public class AppMultiplayerPublicLobbies : CustomApp
{
    public override bool Available => false;
    public static void Initialize()
    {
        PhoneAPI.RegisterApp<AppMultiplayerPublicLobbies>("public lobbies");
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
        var lobbyManager = clientController.ClientLobbyManager;
        var lobbies = lobbyManager.Lobbies;
        foreach (var lobby in lobbies)
        {
            if (!lobby.Value.LobbyState.Challenge) continue;
            if (lobby.Value.LobbyState.InGame) continue;
            var host = clientController.Players[lobby.Value.LobbyState.HostId];
            var button = PhoneUIUtility.CreateSimpleButton($"({lobby.Value.LobbyState.Players.Count}) {lobbyManager.GetLobbyName(lobby.Key)} - {MPUtility.GetPlayerDisplayName(clientController.Players[lobby.Value.LobbyState.HostId].ClientState)}");
            button.Label.spriteAsset = MPAssets.Instance.Sprites;
            button.OnConfirm += () =>
            {
                if (!lobby.Value.LobbyState.Challenge) return;
                if (lobby.Value.LobbyState.InGame) return;
                clientController.ClientLobbyManager.JoinLobby(lobby.Key);
            };
            ScrollView.AddButton(button);

        }
    }
}
