using BombRushMP.Plugin;
using BombRushMP.Plugin.Gamemodes;
using CommonAPI.Phone;
using System.ComponentModel;
using System.Linq;

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
        var lobbies = lobbyManager.Lobbies.OrderBy(p => p.Value.InGame ? 1 : 0);
        foreach (var lobby in lobbies)
        {
            if (lobbyManager.CurrentLobby != null && lobbyManager.CurrentLobby.LobbyState.Id == lobby.Key) continue;
            if (!lobby.Value.LobbyState.Challenge) continue;
            var buttonName = $"({lobby.Value.LobbyState.Players.Count}) {lobbyManager.GetLobbyName(lobby.Key)} - {MPUtility.GetPlayerDisplayName(clientController.Players[lobby.Value.LobbyState.HostId].ClientState)}";
            if (lobby.Value.LobbyState.InGame)
            {
                buttonName += " (In Progress)";
            }
            var host = clientController.Players[lobby.Value.LobbyState.HostId];
            var button = PhoneUIUtility.CreateSimpleButton(buttonName);
            button.Label.spriteAsset = MPAssets.Instance.Sprites;
            button.OnConfirm += () =>
            {
                if (!lobby.Value.LobbyState.Challenge) return;
                if (lobby.Value.LobbyState.InGame)
                {
                    clientController.ClientLobbyManager.QueueJoinLobby(lobby.Key);
                    ChatUI.Instance.AddMessage($"<color=yellow>Lobby is currently in-game. You will join when the game ends.</color>");
                    return;
                }
                clientController.ClientLobbyManager.JoinLobby(lobby.Key);
            };
            ScrollView.AddButton(button);

        }
    }
}
