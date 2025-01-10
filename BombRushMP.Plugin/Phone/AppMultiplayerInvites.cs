using CommonAPI.Phone;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Plugin.Phone
{
    public class AppMultiplayerInvites : CustomApp
    {
        public override bool Available => false;

        public static void Initialize()
        {
            PhoneAPI.RegisterApp<AppMultiplayerInvites>("invites");
        }

        public override void OnAppInit()
        {
            base.OnAppInit();
            CreateIconlessTitleBar("Invites");
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
            var now = DateTime.UtcNow;

            var invites = lobbyManager.LobbiesInvited.OrderBy(x =>
            {
                var lobby = lobbyManager.Lobbies[x];
                var inviteTime = lobby.LobbyState.InvitedPlayers[clientController.LocalID];
                return (now - inviteTime).TotalSeconds;
            });

            var button = PhoneUIUtility.CreateSimpleButton("Ignore all");
            button.OnConfirm += () =>
            {
                clientController.ClientLobbyManager.DeclineAllInvites();
                MyPhone.CloseCurrentApp();
            };
            ScrollView.AddButton(button);

            foreach(var invite in invites)
            {
                var lobby = lobbyManager.Lobbies[invite];
                button = PhoneUIUtility.CreateSimpleButton($"({lobby.LobbyState.Players.Count}) {lobbyManager.GetLobbyName(invite)} - {MPUtility.GetPlayerDisplayName(clientController.Players[lobby.LobbyState.HostId].ClientState)}");
                button.Label.spriteAsset = MPAssets.Instance.Sprites;
                ScrollView.AddButton(button);
                button.OnConfirm += () =>
                {
                    AppMultiplayerAcceptInvite.Show(invite, MyPhone,
                        onAccept: () =>
                        {
                            PhoneUtility.BackToHomescreen(MyPhone);
                            MyPhone.OpenApp(typeof(AppMultiplayer));
                        },
                        onDecline: () =>
                        {
                            PhoneUtility.BackToHomescreen(MyPhone);
                            MyPhone.OpenApp(typeof(AppMultiplayer));
                        });
                };
            }
        }
    }
}
