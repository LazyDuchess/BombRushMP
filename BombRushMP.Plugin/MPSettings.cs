using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using BepInEx.Configuration;
using UnityEngine;
using BombRushMP.Common;

namespace BombRushMP.Plugin
{
    public class MPSettings
    {
        public static MPSettings Instance { get; private set; }
        private const byte Version = 0;

        public string Directory { get; private set; }

        public const int MaxMessageLength = 256;
        public const int MaxNameLength = 64;

        public TMPFilter.Criteria ChatCriteria = new TMPFilter.Criteria(
            [
                "b",
                "color",
                "i",
                "mark",
                "sprite",
                "s",
                "sub",
                "sup",
                "u",
                ]
            , TMPFilter.Criteria.Kinds.Whitelist
        );

        public ReflectionQualities ReflectionQuality
        {
            get
            {
                return _reflectionQuality.Value;
            }

            set
            {
                _reflectionQuality.Value = value;
            }
        }
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

        public bool AFKMessages
        {
            get
            {
                return _afkMessages.Value;
            }

            set
            {
                _afkMessages.Value = value;
            }
        }

        public bool InviteMessages
        {
            get
            {
                return _inviteMessages.Value;
            }

            set
            {
                _inviteMessages.Value = value;
            }
        }

        public KeyCode TalkKey
        {
            get
            {
                return _talkKey.Value;
            }

            set
            {
                _talkKey.Value = value;
            }
        }

        public KeyCode ChatKey
        {
            get
            {
                return _chatKey.Value;
            }

            set
            {
                _chatKey.Value = value;
            }
        }

        public bool FilterProfanity
        {
            get
            {
                return _filterProfanity.Value;
            }

            set
            {
                _filterProfanity.Value = value;
            }
        }

#if DEBUG
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

        public bool DebugInfo
        {
            get
            {
                return _debugInfo.Value;
            }

            set
            {
                _debugInfo.Value = value;
            }
        }

        public bool ShowChat
        {
            get
            {
                return _showChat.Value;
            }

            set
            {
                _showChat.Value = value;
            }
        }

        public BalanceUI.Types BalanceUIType
        {
            get
            {
                return _balanceUIType.Value;
            }

            set
            {
                _balanceUIType.Value = value;
            }
        }

#else
        public bool DebugLocalPlayer => false;
        public bool DebugInfo => false;
#endif

        private ConfigEntry<ReflectionQualities> _reflectionQuality;
        private ConfigEntry<bool> _playerAudioEnabled;
        private ConfigEntry<string> _serverAddress;
        private ConfigEntry<string> _playerName;
        private ConfigEntry<int> _serverPort;
        private ConfigEntry<bool> _debugLocalPlayer;
        private ConfigEntry<bool> _showNamePlates;
        private ConfigEntry<bool> _showMinimap;
        private ConfigEntry<bool> _showNotifications;
        private ConfigEntry<bool> _leaveJoinMessages;
        private ConfigEntry<bool> _afkMessages;
        private ConfigEntry<bool> _inviteMessages;
        private ConfigEntry<bool> _showAFKEffects;
        private ConfigEntry<bool> _debugInfo;
        private ConfigEntry<KeyCode> _talkKey;
        private ConfigEntry<KeyCode> _chatKey;
        private ConfigEntry<bool> _filterProfanity;
        private ConfigEntry<bool> _showChat;
        private ConfigEntry<BalanceUI.Types> _balanceUIType;
        private string _savePath;
        private ConfigFile _configFile;

        private const string General = "1. General";
        private const string Settings = "2. Settings";
        private const string ChatSettings = "3. Chat Settings";
        private const string Input = "4. Input";
        private const string Debug = "5. Debug";
        private const string Visuals = "6. Visuals";
#if DEVELOPER_DEBUG
        private const string MainServerAddress = "ggdev.lazyduchess.me";
#else
        private const string MainServerAddress = "acn.lazyduchess.me";
#endif
        public MPSettings(ConfigFile configFile, string dir)
        {
            Directory = dir;
            Instance = this;
            _configFile = configFile;
            _reflectionQuality = configFile.Bind(Settings, "Reflection Quality", ReflectionQualities.High, "Quality of reflections on reflective surfaces.");
            _playerName = configFile.Bind(General, "Player Name", "Goofiest Gooner", "Your player name.");
            _serverAddress = configFile.Bind(General, "Server Address", MainServerAddress, "Address of the server to connect to.");
            _serverPort = configFile.Bind(General, "Server Port", 41585, "Port of the server to connect to.");
            _playerAudioEnabled = configFile.Bind(Settings, "Player Voices Enabled", true, "Whether to enable voices for other players' actions.");
            _showNamePlates = configFile.Bind(Settings, "Show Nameplates", true, "Whether to show nameplates above players.");
            _showMinimap = configFile.Bind(Settings, "Show Minimap", true, "Whether to always show the minimap in-game.");
            _showNotifications = configFile.Bind(Settings, "Show Notifications", true, "Whether to show notifications when you're invited to a lobby.");
            _playerName.SettingChanged += (sender, args) =>
            {
                var clientController = ClientController.Instance;
                if (clientController.Connected)
                    clientController.SendClientState();
            };
            _showAFKEffects = configFile.Bind(Settings, "Show AFK Effects on Players", true, "Whether to render sleeping Z's on AFK players.");
            _leaveJoinMessages = configFile.Bind(ChatSettings, "Show Player Join/Leave Messages", true, "Whether to show player join/leave messages in chat.");
            _afkMessages = configFile.Bind(ChatSettings, "Show Player AFK Messages", true, "Whether to show a message in chat when a player goes AFK.");
            _inviteMessages = configFile.Bind(ChatSettings, "Show Lobby Invite Messages", true, "Whether to show a message in chat when you're invited to a lobby.");
            _talkKey = configFile.Bind(Input, "Talk Key", KeyCode.H, "Press this key to make your character talk.");
            _chatKey = configFile.Bind(Input, "Chat Key", KeyCode.Tab, "Press this key to open the chat.");
            _filterProfanity = configFile.Bind(ChatSettings, "Filter Profanity", true, "Whether to filter offensive words in the chat.");
            _showChat = configFile.Bind(ChatSettings, "Show Chat", true, "Whether to display the chat.");
            _balanceUIType = configFile.Bind(Visuals, "Balance UI", BalanceUI.Types.TypeB, "Balance UI theme.");

#if DEBUG
            _debugLocalPlayer = configFile.Bind(Debug, "Debug Local Player", false, "Render the networked local player in the game.");
            _debugInfo = configFile.Bind(Debug, "Debug Info", false, "Shows debug stuff.");
#endif
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
