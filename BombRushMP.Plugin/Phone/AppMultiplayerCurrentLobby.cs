﻿using CommonAPI.Phone;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Plugin.Phone
{
    public class AppMultiplayerCurrentLobby : CustomApp
    {
        public override bool Available => false;

        public static void Initialize()
        {
            PhoneAPI.RegisterApp<AppMultiplayerCurrentLobby>("lobby");
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

        public override void OnAppUpdate()
        {
            base.OnAppUpdate();
            var lobbyManager = ClientController.Instance.ClientLobbyManager;
            if (lobbyManager.CurrentLobby == null)
            {
                MyPhone.CloseCurrentApp();
            }
        }

        private void RefreshApp()
        {
            ScrollView.RemoveAllButtons();
            var clientController = ClientController.Instance;
            var lobbyManager = clientController.ClientLobbyManager;
            var currentLobby = lobbyManager.CurrentLobby;
            PhoneButton button = null;
            if (currentLobby.LobbyState.HostId == clientController.LocalID)
            {
                if (currentLobby.CurrentGamemode == null)
                {
                    button = PhoneUIUtility.CreateSimpleButton("Start Game");
                    button.OnConfirm += () =>
                    {
                        if (currentLobby.CurrentGamemode == null)
                        {
                            MyPhone.CloseCurrentApp();
                            MyPhone.TurnOff();
                            lobbyManager.StartGame();
                        }
                    };
                    ScrollView.AddButton(button);

                    button = PhoneUIUtility.CreateSimpleButton("Change Gamemode");
                    button.OnConfirm += () =>
                    {

                    };
                    ScrollView.AddButton(button);

                    button = PhoneUIUtility.CreateSimpleButton("Invite Players");
                    button.OnConfirm += () =>
                    {

                    };
                    ScrollView.AddButton(button);
                }
                else
                {
                    button = PhoneUIUtility.CreateSimpleButton("End Game");
                    button.OnConfirm += () =>
                    {
                        if (currentLobby.CurrentGamemode != null)
                        {
                            MyPhone.CloseCurrentApp();
                            lobbyManager.EndGame();
                        }
                    };
                    ScrollView.AddButton(button);
                }

                button = PhoneUIUtility.CreateSimpleButton("Kick Players");
                button.OnConfirm += () =>
                {

                };
                ScrollView.AddButton(button);
            }

            button = PhoneUIUtility.CreateSimpleButton("Leave Lobby");
            button.OnConfirm += () =>
            {
                lobbyManager.LeaveLobby();
                MyPhone.CloseCurrentApp();
            };
            ScrollView.AddButton(button);
        }
    }
}
