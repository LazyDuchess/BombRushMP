using CommonAPI.Phone;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Plugin.Phone
{
    public class AppMultiplayerLobbyKick : PlayerPickerApp
    {
        public static void Initialize()
        {
            PhoneAPI.RegisterApp<AppMultiplayerLobbyKick>("invite players");
        }

        public override void OnAppInit()
        {
            base.OnAppInit();
            CreateIconlessTitleBar("Kick Players", 70f);
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
            base.PlayerChosen(playerIds);
            var clientController = ClientController.Instance;
            foreach (var player in playerIds)
            {
                clientController.ClientLobbyManager.KickPlayer(player);
            }
        }
    }
}
