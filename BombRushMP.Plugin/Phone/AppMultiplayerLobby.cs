using CommonAPI.Phone;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Plugin.Phone
{
    public class AppMultiplayerLobby : CustomApp
    {
        public override bool Available => false;

        public static void Initialize()
        {
            PhoneAPI.RegisterApp<AppMultiplayerLobby>("lobby");
        }

        public override void OnAppInit()
        {
            base.OnAppInit();
            CreateIconlessTitleBar("Lobby");
            ScrollView = PhoneScrollView.Create(this);
        }

        public override void OnAppEnable()
        {
            base.OnAppEnable();
            RefreshApp();
        }

        private void RefreshApp()
        {
            ScrollView.RemoveAllButtons();
            var lobbyManager = ClientController.Instance.ClientLobbyManager;
            if (lobbyManager.CurrentLobby != null)
            {
                var button = PhoneUIUtility.CreateSimpleButton("Leave Lobby");
                button.OnConfirm += () =>
                {
                    lobbyManager.LeaveLobby();
                    MyPhone.CloseCurrentApp();
                };
                ScrollView.AddButton(button);
            }
            else
            {
                var button = PhoneUIUtility.CreateSimpleButton("Create Lobby");
                button.OnConfirm += () =>
                {
                    lobbyManager.CreateLobby();
                    MyPhone.CloseCurrentApp();
                };
                ScrollView.AddButton(button);
            }
        }
    }
}
