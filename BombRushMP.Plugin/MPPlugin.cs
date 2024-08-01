using BepInEx;
using BombRushMP.Common.Packets;
using HarmonyLib;
using Reptile;
using UnityEngine.SceneManagement;

namespace BombRushMP.Plugin
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class MPPlugin : BaseUnityPlugin
    {
        private void Awake()
        {
            // Plugin startup logic
            var harmony = new Harmony(PluginInfo.PLUGIN_GUID);
            harmony.PatchAll();
            StageManager.OnStagePostInitialization += StageManager_OnStagePostInitialization;
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }

        private void StageManager_OnStagePostInitialization()
        {
            var stage = Reptile.Utility.SceneNameToStage(SceneManager.GetActiveScene().name);
            ClientController.Create("127.0.0.1:41585");
        }
    }
}
