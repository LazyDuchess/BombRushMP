using BepInEx;
using BombRushMP.Common.Packets;
using HarmonyLib;
using Reptile;
using System.Net;
using System.IO;

namespace BombRushMP.Plugin
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class MPPlugin : BaseUnityPlugin
    {
        private void Awake()
        {
            // Plugin startup logic
            var configFile = Path.Combine(Paths.ConfigPath, PluginInfo.PLUGIN_GUID, "Settings.mps");

            new MPSettings(configFile);

            PacketFactory.Initialize();
            var harmony = new Harmony(PluginInfo.PLUGIN_GUID);
            harmony.PatchAll();
            StageManager.OnStagePostInitialization += StageManager_OnStagePostInitialization;
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }

        private void StageManager_OnStagePostInitialization()
        {
            ClientController.Create(Dns.GetHostAddresses("brcmp.lazyduchess.me")[0].ToString() + ":41585");
        }
    }
}
