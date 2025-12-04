using BombRushMP.Common.Packets;
using BombRushMP.Plugin.Gamemodes;
using CommonAPI.Phone;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Plugin.Phone
{
    public class AppMultiplayerSwitchPlayerTeams : PlayerPickerApp
    {
        public static void Initialize()
        {
            PhoneAPI.RegisterApp<AppMultiplayerSwitchPlayerTeams>("set teams");
        }

        public override void OnAppInit()
        {
            base.OnAppInit();
            CreateIconlessTitleBar("Set Teams", 70f);
            AllowEveryone = false;
            AllowMyself = true;
        }

        public override bool PlayerFilter(ushort playerId)
        {
            var lobbyManager = ClientController.Instance.ClientLobbyManager;
            var currentLobby = lobbyManager.CurrentLobby;
            if (currentLobby == null) return false;
            return (currentLobby.LobbyState.Players.ContainsKey(playerId));
        }

        public override void PlayerChosen(ushort[] playerIds)
        {
            var lobbyManager = ClientController.Instance.ClientLobbyManager;
            var currentLobby = lobbyManager.CurrentLobby;
            var clientController = ClientController.Instance;
            foreach (var player in playerIds)
            {
                var team = lobbyManager.CurrentLobby.LobbyState.Players[player].Team;
                team++;
                var teams = GamemodeFactory.GetTeams(lobbyManager.CurrentLobby.LobbyState.Gamemode);
                if (team >= teams.Length)
                    team = 0;
                clientController.SendPacket(new ClientLobbySetPlayerTeam(player, team), Common.Networking.IMessage.SendModes.Reliable, Common.Networking.NetChannels.ClientAndLobbyUpdates);
            }
        }
    }
}
