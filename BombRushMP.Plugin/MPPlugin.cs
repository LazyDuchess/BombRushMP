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

namespace BombRushMP.Plugin
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("CommonAPI", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("CrewBoom", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("MapStation.Plugin", BepInDependency.DependencyFlags.SoftDependency)]
    public class MPPlugin : BaseUnityPlugin
    {
        private void Awake()
        {
            new MPSettings(Config);
            NetworkInterfaceFactory.InitializeNetworkInterface(MPSettings.Instance.NetworkInterface);
            NetworkingEnvironment.NetworkingInterface.MaxPayloadSize = Constants.MaxPayloadSize;
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
        }

        private void StageManager_OnStagePostInitialization()
        {
            ClientController.Create(GetServerAddress(MPSettings.Instance.ServerAddress, MPSettings.Instance.ServerPort));
            LobbyUI.Create();
            TimerUI.Create();
            MPMapController.Create();
            NotificationController.Create();
            ChatUI.Create();
            BalanceUI.Create();
            if (MPSettings.Instance.DebugInfo)
                DebugUI.Create();
        }

        private string GetServerAddress(string address, int port)
        {
            var addresses = Dns.GetHostAddresses(address);
            if (addresses.Length > 0)
                address = addresses[0].ToString();
            address += $":{port}";
            return address;
        }
    }
}
