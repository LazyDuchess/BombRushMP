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

namespace BombRushMP.Plugin
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("CommonAPI", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("CrewBoom", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("MapStation.Plugin", BepInDependency.DependencyFlags.SoftDependency)]
    public class MPPlugin : BaseUnityPlugin
    {
        private string _localAdminKey = Guid.NewGuid().ToString();
        private bool _selfHosting = false;
        private bool _offline = false;
        private ServerController _localServerController;
        private void Awake()
        {
            new MPSettings(Config, Path.GetDirectoryName(Info.Location), Path.Combine(Paths.ConfigPath, PluginInfo.PLUGIN_GUID));
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
            new MPAssets(Path.Combine(Path.GetDirectoryName(Info.Location), "assets"));
            // Plugin startup logic
            if (Chainloader.PluginInfos.ContainsKey("CrewBoom"))
            {
                CrewBoomSupport.Initialize();
            }
            if (Chainloader.PluginInfos.ContainsKey("MapStation.Plugin"))
            {
                MapStationSupport.Initialize();
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
            StageManager.OnStagePostInitialization += StageManager_OnStagePostInitialization;
            ReflectionController.Initialize();
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
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
            if (MPSettings.Instance.DebugInfo)
                DebugUI.Create();
        }
    }
}
