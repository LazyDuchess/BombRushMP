﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using BepInEx.Configuration;

namespace BombRushMP.Plugin
{
    public class MPSettings
    {
        public static MPSettings Instance { get; private set; }
        private const byte Version = 0;
        public bool PlayerAudioEnabled
        {
            get
            {
                return _playerAudioEnabled.Value;
            }

            set
            {
                _playerAudioEnabled.Value = value;
            }
        }
        public string ServerAddress
        {
            get
            {
                return _serverAddress.Value;
            }

            set
            {
                _serverAddress.Value = value;
            }
        }

        public int ServerPort
        {
            get
            {
                return _serverPort.Value;
            }
            set
            {
                _serverPort.Value = value;
            }
        }

        public string PlayerName
        {
            get
            {
                return _playerName.Value;
            }
            set
            {
                _playerName.Value = value;
            }
        }

        public bool DebugLocalPlayer
        {
            get
            {
                return _debugLocalPlayer.Value;
            }

            set
            {
                _debugLocalPlayer.Value = value;
            }
        }

        public bool ShowNamePlates
        {
            get
            {
                return _showNamePlates.Value;
            }

            set
            {
                _showNamePlates.Value = value;
            }
        }

        public bool ShowMinimap
        {
            get
            {
                return _showMinimap.Value;
            }

            set
            {
                _showMinimap.Value = value;
            }
        }

        public bool ShowNotifications
        {
            get
            {
                return _showNotifications.Value;
            }

            set
            {
                _showNotifications.Value = value;
            }
        }

        public bool LeaveJoinMessages
        {
            get
            {
                return _leaveJoinMessages.Value;
            }

            set
            {
                _leaveJoinMessages.Value = value;
            }
        }

        public bool ShowAFKEffects
        {
            get
            {
                return _showAFKEffects.Value;
            }

            set
            {
                _showAFKEffects.Value = value;
            }
        }

        private ConfigEntry<bool> _playerAudioEnabled;
        private ConfigEntry<string> _serverAddress;
        private ConfigEntry<string> _playerName;
        private ConfigEntry<int> _serverPort;
        private ConfigEntry<bool> _debugLocalPlayer;
        private ConfigEntry<bool> _showNamePlates;
        private ConfigEntry<bool> _showMinimap;
        private ConfigEntry<bool> _showNotifications;
        private ConfigEntry<bool> _leaveJoinMessages;
        private ConfigEntry<bool> _showAFKEffects;
        private string _savePath;
        private ConfigFile _configFile;
        public MPSettings(ConfigFile configFile)
        {
            Instance = this;
            _configFile = configFile;
            _playerName = configFile.Bind("General", "Player Name", "Goofiest Gooner", "Your player name.");
            _serverAddress = configFile.Bind("General", "Server Address", "brcmp.lazyduchess.me", "Address of the server to connect to.");
            _serverPort = configFile.Bind("General", "Server Port", 41585, "Port of the server to connect to.");
            _playerAudioEnabled = configFile.Bind("Settings", "Player Voices Enabled", true, "Whether to enable voices for other players' actions.");
            _showNamePlates = configFile.Bind("Settings", "Show Nameplates", true, "Whether to show nameplates above players.");
            _showMinimap = configFile.Bind("Settings", "Show Minimap", true, "Whether to always show the minimap in-game.");
            _debugLocalPlayer = configFile.Bind("Debug", "Debug Local Player", false, "Render the networked local player in the game.");
            _showNotifications = configFile.Bind("Settings", "Show Notifications", true, "Whether to show notifications when you're invited to a lobby.");
            _playerName.SettingChanged += (sender, args) =>
            {
                var clientController = ClientController.Instance;
                if (clientController.Connected)
                    clientController.SendClientState();
            };
            _leaveJoinMessages = configFile.Bind("Settings", "Show Player Join/Leave Messages", true, "Whether to show player join/leave messages in chat.");
            _showAFKEffects = configFile.Bind("Settings", "Show AFK Effects on Players", true, "Whether to render sleeping Z's on AFK players.");
        }

        private void _playerName_SettingChanged(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            _configFile.Save();
        }
    }
}
