﻿using BombRushMP.Common;
using BombRushMP.Common.Networking;
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
    public class AppMultiplayerCurrentLobby : CustomApp
    {
        public override bool Available => false;
        private SimplePhoneButton _allowTeamSwitchingButton;
        private SimplePhoneButton _challengeModeButton;

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
            RefreshLabels();
        }

        private void RefreshLabels()
        {
            var clientController = ClientController.Instance;
            var lobbyManager = clientController.ClientLobbyManager;
            var currentLobby = lobbyManager.CurrentLobby;
            if (currentLobby?.LobbyState == null) return;
            if (_allowTeamSwitchingButton != null)
            {
                _allowTeamSwitchingButton.Label.text = "Allow Team Switching = " + (currentLobby.LobbyState.AllowTeamSwitching ? "ON" : "OFF");
            }
            if (_challengeModeButton != null)
            {
                _challengeModeButton.Label.text = "NPC Mode = " + (currentLobby.LobbyState.Challenge ? "ON" : "OFF");
            }
        }

        private void RefreshApp()
        {
            ScrollView.RemoveAllButtons();
            var clientController = ClientController.Instance;
            var lobbyManager = clientController.ClientLobbyManager;
            var currentLobby = lobbyManager.CurrentLobby;
            if (currentLobby == null) return;
            var gamemode = currentLobby.GetOrCreateGamemode();
            PhoneButton button = null;
            if (currentLobby.LobbyState.HostId == clientController.LocalID)
            {
                if (!currentLobby.InGame)
                {
                    button = PhoneUIUtility.CreateSimpleButton("Start Game");
                    button.OnConfirm += () =>
                    {
                        if (!currentLobby.InGame)
                        {
                            lobbyManager.StartGame();
                        }
                    };
                    ScrollView.AddButton(button);

                    button = PhoneUIUtility.CreateSimpleButton("Change Gamemode");
                    button.OnConfirm += () =>
                    {
                        var gamemodeValues = Enum.GetValues(typeof(GamemodeIDs));
                        var gamemode = (int)currentLobby.LobbyState.Gamemode;
                        gamemode++;
                        if (gamemode >= gamemodeValues.Length)
                            gamemode = 0;
                        var settings = GamemodeFactory.GetGamemodeSettings((GamemodeIDs)gamemode);
                        var savedSettings = MPSaveData.Instance.GetSavedSettings((GamemodeIDs)gamemode);
                        if (savedSettings != null)
                            settings.ApplySaved(savedSettings);
                        lobbyManager.SetGamemode((GamemodeIDs)gamemode, settings);

                    };
                    ScrollView.AddButton(button);

                    button = PhoneUIUtility.CreateSimpleButton("Gamemode Settings");
                    button.OnConfirm += () =>
                    {
                        MyPhone.OpenApp(typeof(AppGamemodeSettings));

                    };
                    ScrollView.AddButton(button);

                    button = PhoneUIUtility.CreateSimpleButton("Invite Players");
                    button.OnConfirm += () =>
                    {
                        MyPhone.OpenApp(typeof(AppMultiplayerLobbyInvite));
                    };
                    ScrollView.AddButton(button);

                    button = PhoneUIUtility.CreateSimpleButton("Switch Teams");
                    button.OnConfirm += () =>
                    {
                        MyPhone.OpenApp(typeof(AppMultiplayerSwitchPlayerTeams));
                    };
                    ScrollView.AddButton(button);

                    _allowTeamSwitchingButton = PhoneUIUtility.CreateSimpleButton("Allow Team Switching");
                    _allowTeamSwitchingButton.OnConfirm += () =>
                    {
                        clientController.ClientLobbyManager.SetAllowTeamSwitching(!currentLobby.LobbyState.AllowTeamSwitching);
                    };
                    ScrollView.AddButton(_allowTeamSwitchingButton);

                    _challengeModeButton = PhoneUIUtility.CreateSimpleButton("NPC Mode");
                    _challengeModeButton.OnConfirm += () =>
                    {
                        clientController.ClientLobbyManager.SetChallenge(!currentLobby.LobbyState.Challenge);
                    };
                    ScrollView.AddButton(_challengeModeButton);
                }
                else
                {
                    button = PhoneUIUtility.CreateSimpleButton("End Game");
                    button.OnConfirm += () =>
                    {
                        if (currentLobby.InGame)
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
                    MyPhone.OpenApp(typeof(AppMultiplayerLobbyKick));
                };
                ScrollView.AddButton(button);
            }
            else
            {
                if (!currentLobby.InGame)
                {
                    button = PhoneUIUtility.CreateSimpleButton("Switch Team");
                    button.OnConfirm += () =>
                    {
                        var team = lobbyManager.CurrentLobby.LobbyState.Players[clientController.LocalID].Team;
                        team++;
                        if (team >= TeamManager.Teams.Length)
                            team = 0;
                        clientController.SendPacket(new ClientSetTeam(team), IMessage.SendModes.ReliableUnordered, NetChannels.ClientAndLobbyUpdates);
                    };
                    ScrollView.AddButton(button);

                    button = PhoneUIUtility.CreateSimpleButton("Toggle Ready");
                    button.OnConfirm += () =>
                    {
                        clientController.SendPacket(new ClientLobbySetReady(!currentLobby.LobbyState.Players[clientController.LocalID].Ready), IMessage.SendModes.ReliableUnordered, NetChannels.ClientAndLobbyUpdates);
                    };
                    ScrollView.AddButton(button);
                }
            }
            button = PhoneUIUtility.CreateSimpleButton("Leave Lobby");
            button.OnConfirm += () =>
            {
                lobbyManager.LeaveLobby();
                MyPhone.CloseCurrentApp();
            };
            ScrollView.AddButton(button);
            RefreshLabels();
        }
    }
}
