using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using BepInEx.Configuration;
using UnityEngine;
using BombRushMP.Common;
using BombRushMP.NetworkInterfaceProvider;
using BombRushMP.Common.Networking;
using Reptile;

namespace BombRushMP.Plugin
{
    public class MPSettings
    {
        public static MPSettings Instance { get; private set; }
        private const byte Version = 0;

        public string Directory { get; private set; }

        public TMPFilter.Criteria ChatCriteria = new TMPFilter.Criteria(
            [
                "b",
                "color",
                "i",
                "mark",
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
        public bool PlayerDopplerEnabled
        {
            get
            {
                return _playerDopplerEnabled.Value;
            }

            set
            {
                _playerDopplerEnabled.Value = value;
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
                var nam = _playerName.Value;
                if (nam.Length > Constants.MaxNameLength)
                    nam = nam.Substring(0, Constants.MaxNameLength);
                return nam;
            }
            set
            {
                _playerName.Value = value;
            }
        }

        public string CrewName
        {
            get
            {
                var nam = _crewName.Value;
                if (nam.Length > Constants.MaxCrewNameLength)
                    nam = nam.Substring(0, Constants.MaxCrewNameLength);
                return nam;
            }
            set
            {
                _crewName.Value = value;
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

#if DEBUG
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
#else
        public bool FilterProfanity => true;
        public bool DebugLocalPlayer => false;
        public bool DebugInfo => false;
#endif

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

        public NetworkInterfaces NetworkInterface
        {
            get
            {
                return _networkInterface.Value;
            }
			set
			{
				_networkInterface.Value = value;
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

        public bool UseNativeSockets
        {
            get
            {
                return _useNativeSockets.Value;
            }

            set
            {
                _useNativeSockets.Value = value;
            }
        }

        public bool HostServer
        {
            get
            {
                return _hostServer.Value;
            }

            set
            {
                _hostServer.Value = value;
            }
        }

        public float TicksPerSecond
        {
            get
            {
                return _ticksPerSecond.Value;
            }

            set
            {
                _ticksPerSecond.Value = value;
            }
        }

        public ushort MaxPlayers
        {
            get
            {
                return _maxPlayers.Value;
            }

            set
            {
                _maxPlayers.Value = value;
            }
        }

        public bool Offline
        {
            get
            {
                return _offline.Value;
            }

            set
            {
                _offline.Value = value;
            }
        }

        public string AuthKey
        {
            get
            {
                return _authKey;
            }

            set
            {
                _authKey = value;
            }
        }

        public CharacterNames FallbackCharacter
        {
            get
            {
                return _fallbackCharacter.Value;
            }
            set
            {
                _fallbackCharacter.Value = value;
            }
        }

        public int FallbackOutfit
        {
            get
            {
                return _fallbackOutfit.Value;
            }

            set
            {
                _fallbackOutfit.Value = value;
            }
        }

#if DEBUG
        public bool Invisible
        {
            get
            {
                return _invisible.Value;
            }
            set
            {
                _invisible.Value = value;
            }
        }
#else
        public bool Invisible => false;
#endif
        public KeyCode PlayerListKey
        {
            get
            {
                return _playerListKey.Value;
            }
            set
            {
                _playerListKey.Value = value;
            }
        }

        public bool DontAutoScrollChatIfFocused
        {
            get
            {
                return _dontAutoScrollChatIfFocused.Value;
            }

            set
            {
                _dontAutoScrollChatIfFocused.Value = value;
            }
        }

        public bool DeathMessages
        {
            get
            {
                return _deathMessages.Value;
            }

            set
            {
                _deathMessages.Value = value;
            }
        }

        public bool HidePlayersOutOfView
        {
            get
            {
                return _hidePlayersOutOfView.Value;
            }

            set
            {
                _hidePlayersOutOfView.Value = value;
            }
        }

        public float PlayerDrawDistance
        {
            get
            {
                return _playerDrawDistance.Value;
            }

            set
            {
                _playerDrawDistance.Value = value;
            }
        }

        private ConfigEntry<ReflectionQualities> _reflectionQuality;
        private ConfigEntry<bool> _playerAudioEnabled;
        private ConfigEntry<bool> _playerDopplerEnabled;
        private ConfigEntry<string> _serverAddress;
        private ConfigEntry<string> _playerName;
        private ConfigEntry<string> _crewName;
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
        private ConfigEntry<KeyCode> _playerListKey;
        private ConfigEntry<bool> _filterProfanity;
        private ConfigEntry<bool> _showChat;
        private ConfigEntry<BalanceUI.Types> _balanceUIType;
        private ConfigEntry<NetworkInterfaces> _networkInterface;
        private ConfigEntry<bool> _useNativeSockets;
        private ConfigEntry<bool> _hostServer;
        private ConfigEntry<float> _ticksPerSecond;
        private ConfigEntry<ushort> _maxPlayers;
        private ConfigEntry<bool> _offline;
        private string _authKey = "";
        private ConfigEntry<CharacterNames> _fallbackCharacter;
        private ConfigEntry<int> _fallbackOutfit;
        private ConfigEntry<bool> _invisible;
        private ConfigEntry<bool> _dontAutoScrollChatIfFocused;
        private ConfigEntry<bool> _deathMessages;
        private ConfigEntry<bool> _hidePlayersOutOfView;
        private ConfigEntry<float> _playerDrawDistance;
#if DEBUG
        public bool UpdatePlayers => _updatePlayers.Value;
        public bool UpdateLobbyUI => _updateLobbyUI.Value;
        public bool UpdateClientController => _updateClientController.Value;
        public bool UpdateLobbyController => _updateLobbyController.Value;
        public bool UpdateNetworkClient => _updateNetworkClient.Value;
        private ConfigEntry<bool> _updatePlayers;
        private ConfigEntry<bool> _updateLobbyUI;
        private ConfigEntry<bool> _updateClientController;
        private ConfigEntry<bool> _updateLobbyController;
        private ConfigEntry<bool> _updateNetworkClient;
#endif
        private string _savePath;
        private ConfigFile _configFile;

        private const string General = "1. General";
        private const string Settings = "2. Settings";
        private const string ChatSettings = "3. Chat Settings";
        private const string Input = "4. Input";
        private const string Debug = "5. Debug";
        private const string Visuals = "6. Visuals";
        private const string Advanced = "7. Advanced";
        private const string Server = "8. Server";
        private const string Optimization = "9. Optimization";
        private const string MainServerAddress = "free.soulisall.city";
        public const string DefaultName = "Goofiest Gooner";

        public MPSettings(ConfigFile configFile, string dir, string configPath)
        {
            Directory = dir;
            Instance = this;
            _configFile = configFile;
            var authFilePath = Path.Combine(configPath, "auth.txt");
            System.IO.Directory.CreateDirectory(configPath);
            if (!File.Exists(authFilePath))
                File.Create(authFilePath);
            else
                _authKey = TMPFilter.Sanitize(File.ReadAllText(authFilePath));
            _reflectionQuality = configFile.Bind(Settings, "Reflection Quality", ReflectionQualities.High, "Quality of reflections on reflective surfaces.");
            _playerName = configFile.Bind(General, "Player Name", DefaultName, "Your player name.");
            _playerName.SettingChanged += (sender, args) =>
            {
                var clientController = ClientController.Instance;
                clientController.InfrequentClientStateUpdateQueued = true;
            };
            _crewName = configFile.Bind(General, "Crew Name", "", "Name of your crew.");
            _crewName.SettingChanged += (sender, args) =>
            {
                var clientController = ClientController.Instance;
                clientController.InfrequentClientStateUpdateQueued = true;
            };
            _serverAddress = configFile.Bind(General, "Server Address", MainServerAddress, "Address of the server to connect to.");
            _serverPort = configFile.Bind(General, "Server Port", 41585, "Port of the server to connect to.");
            _playerAudioEnabled = configFile.Bind(Settings, "Player Voices Enabled", true, "Whether to enable voices for other players' actions.");
            _playerDopplerEnabled = configFile.Bind(Settings, "Player Audio Doppler", false, "Whether to enable the doppler effect on other players' sound effects.");
            _showNamePlates = configFile.Bind(Settings, "Show Nameplates", true, "Whether to show nameplates above players.");
            _showMinimap = configFile.Bind(Settings, "Show Minimap", true, "Whether to always show the minimap in-game.");
            _showNotifications = configFile.Bind(Settings, "Show Notifications", true, "Whether to show notifications when you're invited to a lobby.");
            _showAFKEffects = configFile.Bind(Settings, "Show AFK Effects on Players", true, "Whether to render sleeping Z's on AFK players.");
            _leaveJoinMessages = configFile.Bind(ChatSettings, "Show Player Join/Leave Messages", true, "Whether to show player join/leave messages in chat.");
            _afkMessages = configFile.Bind(ChatSettings, "Show Player AFK Messages", true, "Whether to show a message in chat when a player goes AFK.");
            _inviteMessages = configFile.Bind(ChatSettings, "Show Lobby Invite Messages", true, "Whether to show a message in chat when you're invited to a lobby.");
            _talkKey = configFile.Bind(Input, "Talk Key", KeyCode.H, "Press this key to make your character talk.");
            _chatKey = configFile.Bind(Input, "Chat Key", KeyCode.Tab, "Press this key to open the chat.");
            _playerListKey = configFile.Bind(Input, "Player List Key", KeyCode.J, "Press this key to toggle the player list.");
            _showChat = configFile.Bind(ChatSettings, "Show Chat", true, "Whether to display the chat.");
            _dontAutoScrollChatIfFocused = configFile.Bind(ChatSettings, "No auto scroll chat if focused", false, "If true, will not scroll to the bottom when messages are received if the chat window is open for typing/scrolling.");
            _balanceUIType = configFile.Bind(Visuals, "Balance UI", BalanceUI.Types.TypeC, "Balance UI theme.");
#if DEBUG
            _filterProfanity = configFile.Bind(ChatSettings, "Filter Profanity", true, "Whether to filter offensive words in the chat.");
            _debugLocalPlayer = configFile.Bind(Debug, "Debug Local Player", false, "Render the networked local player in the game.");
            _debugInfo = configFile.Bind(Debug, "Debug Info", false, "Shows debug stuff.");
            _invisible = configFile.Bind(Debug, "Invisible", false);
            _updatePlayers = configFile.Bind(Debug, "Update Players", true, "FOR STRESS TEST: update mpplayers");
            _updateLobbyUI = configFile.Bind(Debug, "Update Lobby UI", true, "FOR STRESS TEST: update lobby ui");
            _updateClientController = configFile.Bind(Debug, "Update Client Controller", true, "FOR STRESS TEST: update client controller");
            _updateLobbyController = configFile.Bind(Debug, "Update Lobby Controller", true, "FOR STRESS TEST: update client lobby controller");
            _updateNetworkClient = configFile.Bind(Debug, "Update Network Client", true, "FOR STRESS TEST: update net interface client");
#endif
            _networkInterface = configFile.Bind(Advanced, "Network Interface", NetworkInterfaces.LiteNetLib, "Networking library to use. Should match the server.");
            _useNativeSockets = configFile.Bind(Advanced, "Use Native Sockets", true, "Whether the networking library should use native sockets if available. Potentially better performance. Currently only supported via LiteNetLib networking.");
            _useNativeSockets.SettingChanged += (sender, args) =>
            {
                NetworkingEnvironment.UseNativeSocketsIfAvailable = UseNativeSockets;
            };
            _hostServer = configFile.Bind(Server, "Host Server", false, "Host a local server.");
            _ticksPerSecond = configFile.Bind(Server, "Ticks per second", 1f / Constants.DefaultNetworkingTickRate, "Networking updates per second for local server.");
            _maxPlayers = configFile.Bind(Server, "Max players", (ushort)64, "Max players for local server.");
            _offline = configFile.Bind(General, "Offline", false, "Run All City Network in singleplayer, offline mode.");
            _fallbackCharacter = configFile.Bind(General, "Fallback Character", CharacterNames.Red, "Character to display to other players when you're using a CrewBoom character they don't have.");
            _fallbackCharacter.SettingChanged += (sender, args) =>
            {
                var clientController = ClientController.Instance;
                clientController.InfrequentClientStateUpdateQueued = true;
            };
            _fallbackOutfit = configFile.Bind(General, "Fallback Character Outfit", 0, "Outfit number to use for your fallback character.");
            _fallbackOutfit.SettingChanged += (sender, args) =>
            {
                var clientController = ClientController.Instance;
                clientController.InfrequentClientStateUpdateQueued = true;
            };
            _deathMessages = configFile.Bind(ChatSettings, "Show Player Death Messages", true, "Whether to send messages in chat when players die.");
            _hidePlayersOutOfView = configFile.Bind(Optimization, "Cull Players out of view", true, "Whether to cull players that are out of view, to save on resources.");
            _playerDrawDistance = configFile.Bind(Optimization, "Player draw distance", 20000f, "Distance at which players will be hidden");
        }

        public void Save()
        {
            _configFile.Save();
        }
    }
}
