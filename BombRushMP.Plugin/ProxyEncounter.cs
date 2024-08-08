using Reptile;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;
using UnityEngine;
using CommonAPI;

namespace BombRushMP.Plugin
{
    public class ProxyEncounter : Encounter
    {
        public static ProxyEncounter Instance;
        private const string UID = "3c2af407-bdcc-45af-82e0-ab20104d41a4";

        public static void Initialize()
        {
            StageAPI.OnStagePreInitialization += OnStagePreInitialization;
        }

        private static void OnStagePreInitialization(Stage newStage, Stage previousStage)
        {
            var encounterGameObject = new GameObject("BRCMP Proxy Encounter");
            Instance = encounterGameObject.AddComponent<ProxyEncounter>();
            Instance.uid = UID;
        }

        public override void InitSceneObject()
        {
            makeUnavailableDuringEncounter = new GameObject[0];
            introSequence = null;
            currentCheckpoint = -1;
            OnIntro = new UnityEvent();
            OnStart = new UnityEvent();
            OnOutro = new UnityEvent();
            OnCompleted = new UnityEvent();
            OnFailed = new UnityEvent();

            base.InitSceneObject();
        }

        // Don't load.
        public override void ReadFromData()
        {

        }

        // Don't save.
        public override void WriteToData()
        {

        }
    }
}
