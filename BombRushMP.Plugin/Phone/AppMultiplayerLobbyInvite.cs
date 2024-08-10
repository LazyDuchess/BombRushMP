using CommonAPI.Phone;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Plugin.Phone
{
    public class AppMultiplayerLobbyInvite : PlayerPickerApp
    {
        public static void Initialize()
        {
            PhoneAPI.RegisterApp<AppMultiplayerLobbyInvite>("invite players");
        }

        public override void OnAppInit()
        {
            base.OnAppInit();
            CreateIconlessTitleBar("Invite Players", 70f);
        }

        public override void PlayerChosen(ushort[] playerIds)
        {
            base.PlayerChosen(playerIds);
            var clientController = ClientController.Instance;
            foreach(var player in playerIds)
            {
                clientController.ClientLobbyManager.InvitePlayer(player);
            }
        }
    }
}
