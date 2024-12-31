using Reptile;
using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BombRushMP.Common;
using System.Collections.ObjectModel;
using UnityEngine;
using System.Collections.Generic;

namespace BombRushMP.MapStation
{
    public static class MapStationSupport
    {
        public static bool Installed { get; private set; } = false;
        public static List<MPStage> Stages = new();
        private static Type _apiManagerType;
        public static void Initialize()
        {
            Installed = true;

            _apiManagerType = ReflectionUtility.GetTypeByName("MapStation.API.APIManager");
            var onInitializedEvent = _apiManagerType.GetEvent("OnInitialized", BindingFlags.Static | BindingFlags.Public);
            onInitializedEvent.AddEventHandler(null, OnMapStationAPIInitialized);
        }

        private static void OnMapStationAPIInitialized()
        {
            var customStageType = ReflectionUtility.GetTypeByName("MapStation.API.ICustomStage");
            var apiType = ReflectionUtility.GetTypeByName("MapStation.API.IMapStationAPI");

            var api = _apiManagerType.GetField("API", BindingFlags.Static | BindingFlags.Public).GetValue(null);
            var customStages = apiType.GetProperty("CustomStages", BindingFlags.Public | BindingFlags.Instance).GetValue(api) as IList;

            var stageType = ReflectionUtility.GetTypeByName("MapStation.API.ICustomStage");
            var stageDisplayNameField = stageType.GetProperty("DisplayName");
            var stageIdField = stageType.GetProperty("StageID");

            foreach(var stage in customStages)
            {
                var displayName = (string)stageDisplayNameField.GetValue(stage);
                var stageId = (int)stageIdField.GetValue(stage);

                var mpStage = new MPStage(displayName, stageId);

                Stages.Add(mpStage);
            }
        }

        public static bool IsMapOptionToggleable(GameObject go)
        {
            var comps = go.GetComponentsInParent<MonoBehaviour>(true);
            foreach(var comp in comps)
            {
                if (comp.GetType().Name == "ActiveOnMapOption")
                    return true;
            }
            return false;
        }
    }
}
