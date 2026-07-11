using BepInEx;
using BombRushMP.Common.Packets;
using HarmonyLib;
using Reptile;
using System.Net;
using System.IO;
using BepInEx.Bootstrap;
using BombRushMP.CrewBoom;
using BombRushMP.Plugin.Phone;
using BombRushMP.MapStation;
using BombRushMP.Common;
using BombRushMP.Plugin.Patches;
using BombRushMP.Common.Networking;
using BombRushMP.NetworkInterfaceProvider;
using BombRushMP.Plugin.LocalServer;
using BombRushMP.Plugin.OfflineInterface;
using System;
using BombRushMP.BunchOfEmotes;
using BombRushMP.NetRadio;
using BombRushMP.Plugin.Gamemodes;
using CommonAPI;
using BombRushMP.PluginCommon;
using System.Collections.Generic;
using BombRushMP.Mono.Runtime;
using Newtonsoft.Json;
using UnityEngine;
using System.Linq;

namespace BombRushMP.Plugin
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("CommonAPI", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("CrewBoom", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("MapStation.Plugin", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Dragsun.BunchOfEmotes", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("goatgirl.NetRadio", BepInDependency.DependencyFlags.SoftDependency)]
    public class MPPlugin : BaseUnityPlugin
    {
        private string _localAdminKey = Guid.NewGuid().ToString();
        private bool _selfHosting = false;
        private bool _offline = false;
        private ServerController _localServerController;

        public static Dictionary<string, string> ThemePaths = new();

        private void RegisterThemes(string path)
        {
            var themeDirs = Directory.GetDirectories(path, "*", SearchOption.TopDirectoryOnly);
            foreach (var dir in themeDirs)
            {
                var themeName = Path.GetFileName(dir);
                ThemePaths[themeName] = dir;
            }
        }

        private Theme LoadTheme(string themePath)
        {
            var theme = new Theme();
            var configPath = Path.Combine(themePath, "theme.json");
            var themeConfig = JsonConvert.DeserializeObject<ThemeConfig>(File.ReadAllText(configPath));
            try
            {
                var serializedTheme = JsonConvert.SerializeObject(themeConfig, Formatting.Indented);
                File.WriteAllText(configPath, serializedTheme);
            }
            catch (Exception e) 
            {
                Debug.LogError("Failed to write back serialized theme!");
                Debug.LogException(e);
            }
            theme.ParseConfig(themeConfig);
            var reticlePath = Path.Combine(themePath, "greticle.png");
            if (File.Exists(reticlePath))
            {
                var reticleTex = new Texture2D(2, 2);
                reticleTex.LoadImage(File.ReadAllBytes(reticlePath), true);
                theme.GraffitiReticle = reticleTex;
            }
            return theme;
        }

        private void Awake()
        {
            new MPSettings(Config, Path.GetDirectoryName(Info.Location), Path.Combine(Paths.ConfigPath, PluginInfo.PLUGIN_GUID));

            var defaultThemePath = Path.Combine(Path.GetDirectoryName(Info.Location), "themes");
            var customThemePath = Path.Combine(Paths.ConfigPath, PluginInfo.PLUGIN_GUID, "themes");

            Directory.CreateDirectory(defaultThemePath);
            Directory.CreateDirectory(customThemePath);

            RegisterThemes(defaultThemePath);
            RegisterThemes(customThemePath);

            var theme = MPSettings.Instance.Theme;

            if (!ThemePaths.ContainsKey(theme))
            {
                theme = "Default";
                MPSettings.Instance.Theme = theme;
            }

            Theme.CurrentTheme = LoadTheme(ThemePaths[theme]);

            NetworkingEnvironment.UseNativeSocketsIfAvailable = MPSettings.Instance.UseNativeSockets;
            NetworkingEnvironment.NetworkingInterface = NetworkInterfaceFactory.GetNetworkInterface(MPSettings.Instance.NetworkInterface);
            NetworkingEnvironment.LogEventHandler += (log) =>
            {
                ClientLogger.Log($"[{nameof(NetworkingEnvironment)}] {log}");
            };
            NetworkingEnvironment.NetworkingInterface.MaxPayloadSize = Constants.MaxPayloadSize;
            _offline = MPSettings.Instance.Offline;
            _selfHosting = MPSettings.Instance.HostServer;
            var maxPlayers = MPSettings.Instance.MaxPlayers;
            if (_offline)
            {
                _selfHosting = true;
                maxPlayers = 1;
                NetworkingEnvironment.NetworkingInterface = new OfflineInterface.OfflineInterface();
            }
            if (_selfHosting)
                _localServerController = new ServerController(MPSettings.Instance.ServerPort, 1f / MPSettings.Instance.TicksPerSecond, maxPlayers, _offline, new LocalServerDatabase(_localAdminKey));
            new MPAssets(Path.GetDirectoryName(Info.Location));
            // Plugin startup logic
            if (Chainloader.PluginInfos.ContainsKey("CrewBoom"))
            {
                CrewBoomSupport.Initialize();
                CrewBoomStreamer.Initialize();
                var cbFolder = Path.Combine(Paths.ConfigPath, "CrewBoom");
                var cbStreamFolder = Path.Combine(Paths.ConfigPath, PluginInfo.PLUGIN_GUID, "CrewBoom");
                Directory.CreateDirectory(cbStreamFolder);
                CrewBoomStreamer.AddDirectory(cbStreamFolder);
                CrewBoomStreamer.LoadCrewBoomConfigFolder(cbFolder);
            }
            if (Chainloader.PluginInfos.ContainsKey("MapStation.Plugin"))
            {
                MapStationSupport.Initialize();
            }
            if (Chainloader.PluginInfos.ContainsKey("goatgirl.NetRadio"))
            {
                NetRadioSupport.Initialize();
            }
            if (Chainloader.PluginInfos.ContainsKey("com.Dragsun.BunchOfEmotes"))
            {
                BunchOfEmotesSupport.Initialize();
            }
            ProxyEncounter.Initialize();
            InitializePhone();
            PacketFactory.Initialize();
            new SpecialSkinManager();
            new MPUnlockManager();
            new MPSaveData();
            var harmony = new Harmony(PluginInfo.PLUGIN_GUID);
            harmony.PatchAll();
            InputPatch.Patch(harmony);
            StageManager.OnStageInitialized += StageManager_OnStageInitialized;
            StageManager.OnStagePostInitialization += StageManager_OnStagePostInitialization;
            ReflectionController.Initialize();
            StageAPI.OnStagePreInitialization += OnStagePreInitialization;
            InitShared();
            InitBannedModsSystem();
            PlayerRagdoll.InitializeStatic();
            PreferencesPacket.InitializeStatic();
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }

        private void InitBannedModsSystem()
        {
            ClientController.RegisterCustomPacketHandler(CustomPacketBannedMods.JoinedLobbyPacketId, (id, data) =>
            {
                var clientController = ClientController.Instance;
                var lobbyManager = clientController.ClientLobbyManager;

                if (lobbyManager.CurrentLobby == null) return;
                if (lobbyManager.CurrentLobby.LobbyState.HostId != clientController.LocalID) return;

                var packet = new CustomPacketBannedMods(clientController.BannedMods);
                clientController.BroadcastCustomPacketToCurrentLobby(packet.Serialize(), CustomPacketBannedMods.PacketId, IMessage.SendModes.Reliable);
            });
            ClientController.RegisterCustomPacketHandler(CustomPacketBannedMods.PacketId, (id, data) =>
            {
                var clientController = ClientController.Instance;
                var lobbyManager = clientController.ClientLobbyManager;

                if (lobbyManager.CurrentLobby == null) return;
                if (lobbyManager.CurrentLobby.LobbyState.HostId != id) return;

                var bannedMods = new CustomPacketBannedMods();
                bannedMods.Deserialize(data);

                var flaggedMods = MPUtility.GetMyFlaggedMods(bannedMods.BannedMods);

                if (flaggedMods.Count > 0)
                {
                    lobbyManager.LeaveLobby();
                    var chat = ChatUI.Instance;
                    chat.AddMessage("<color=yellow>Hey you! You can't play in this lobby because the host has banned the following mods you have installed:");
                    foreach(var flaggedMod in flaggedMods)
                    {
                        chat.AddMessage($"<color=red>{flaggedMod}");
                    }
                    chat.AddMessage("<color=yellow>Disable these mods and try again!");
                }
            });
        }

        private void InitShared()
        {
            SharedUtils.SetLocalCharacter += (Characters ch) => {
                var save = Core.Instance.SaveManager.CurrentSaveSlot;
                var outfit = save.GetCharacterProgress(ch).outfit;
                var ply = WorldHandler.instance.GetCurrentPlayer();
                SpecialSkinManager.Instance.RemoveSpecialSkinFromPlayer(ply);
                ply.SetCharacter(ch, outfit);
                ply.InitVisual();
                Core.Instance.SaveManager.SaveCurrentSaveSlot();
            };

            SharedUtils.SetLocalSpecialSkin += (SpecialSkins skin) => {
                SpecialSkinManager.Instance.ApplySpecialSkinToPlayer(WorldHandler.instance.GetCurrentPlayer(), skin);
            };
        }


        private void OnStagePreInitialization(Stage newStage, Stage previousStage)
        {
            if (MPUtility.IsChristmas())
                XmasController.Create();
        }

        private void StageManager_OnStageInitialized()
        {
            if (CrewBoomSupport.Installed)
            {
                CrewBoomStreamer.ReloadResources();
                if (!MPSettings.Instance.ReloadCharactersInLoadingScreens && CrewBoomStreamer.AlreadyLoadedThisSession)
                {
                    //wao
                }
                else
                    CrewBoomStreamer.ReloadCharacters();
            }
            PropDisguiseController.Create();
        }

        private void Update()
        {
            CrewBoomStreamer.Tick();
        }

        private void InitializePhone()
        {
            AppMultiplayerBan.Initialize();
            AppGamemodeSettings.Initialize();
            AppMultiplayer.Initialize();
            AppMultiplayerDebug.Initialize();
            AppJoinLobbyDebug.Initialize();
            AppMultiplayerCurrentLobby.Initialize();
            AppMultiplayerStages.Initialize();
            AppMultiplayerBaseStages.Initialize();
            AppMultiplayerCustomStages.Initialize();
            AppMultiplayerLobbyInvite.Initialize();
            AppMultiplayerInvites.Initialize();
            AppMultiplayerAcceptInvite.Initialize();
            AppMultiplayerLobbyKick.Initialize();
            MoveStylePickerApp.Initialize();
            MoveStyleSkinPickerApp.Initialize();
            AppMultiplayerSwitchPlayerTeams.Initialize();
            AppMultiplayerPublicLobbies.Initialize();
            AppMultiplayerSettings.Initialize();
        }

        private void StageManager_OnStagePostInitialization()
        {
            var addr = MPSettings.Instance.ServerAddress;
            var authKey = MPSettings.Instance.AuthKey;
            if (_selfHosting)
            {
                addr = "localhost";
                authKey = _localAdminKey;
            }
            ClientController.Create(addr, MPSettings.Instance.ServerPort);
            ClientController.Instance.AuthKey = authKey;
            LobbyUI.Create();
            PlayerListUI.Create();
            TimerUI.Create();
            MPMapController.Create();
            NotificationController.Create();
            ChatUI.Create();
            BalanceUI.Create();
            StatsUI.Create();
            if (MPSettings.Instance.DebugInfo)
                DebugUI.Create();
        }
    }
}
