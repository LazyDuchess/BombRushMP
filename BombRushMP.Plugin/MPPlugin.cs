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

namespace BombRushMP.Plugin
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInDependency("CrewBoom", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("MapStation.Plugin", BepInDependency.DependencyFlags.SoftDependency)]
    public class MPPlugin : BaseUnityPlugin
    {
        private void Awake()
        {
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
            new MPSettings(Config);
            PacketFactory.Initialize();
            var harmony = new Harmony(PluginInfo.PLUGIN_GUID);
            harmony.PatchAll();
            StageManager.OnStagePostInitialization += StageManager_OnStagePostInitialization;
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
        }

        private void StageManager_OnStagePostInitialization()
        {
            ClientController.Create(GetServerAddress(MPSettings.Instance.ServerAddress, MPSettings.Instance.ServerPort));
            LobbyUI.Create();
            TimerUI.Create();
            MPMapController.Create();
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
