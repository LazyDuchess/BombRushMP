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

        public override void SetEncounterState(EncounterState setState)
        {
            stateTimer = 0f;
            state = setState;
            switch (state)
            {
                case EncounterState.CLOSED:
                    EnterEncounterState(EncounterState.CLOSED);
                    if (WorldHandler.instance.currentEncounter == this)
                    {
                        WorldHandler.instance.MakeStuffAvailableAgainCauseEncounterOver();
                    }
                    break;
                case EncounterState.OPEN_STARTUP:
                    EnterEncounterState(EncounterState.OPEN_STARTUP);
                    return;
                case EncounterState.OPEN:
                    if (openOn == OpenOn.REQUIRED_REP && requirement != 0)
                    {
                        WorldHandler.instance.ResetRequiredREP();
                    }
                    requirement = 0;
                    if (openingSequence != null && !shownOpening)
                    {
                        handler.StartEnteringSequence(openingSequence, this, turnOffPlayerDuringSequences, true, false, false, true, null, true, lowerVolumeDuringSequence, null, false);
                        return;
                    }
                    EnterEncounterState(Encounter.EncounterState.OPEN);
                    shownOpening = true;
                    return;
                case Encounter.EncounterState.INTRO:
                    WorldHandler.instance.StartEncounter(this);
                    currentlyActive = true;
                    ChangeSkybox(true);
                    if (introSequence != null && currentCheckpoint == -1)
                    {
                        handler.StartEnteringSequence(introSequence, this, turnOffPlayerDuringSequences, true, instantIntro, false, allowPhone, null, true, lowerVolumeDuringSequence, null, false);
                    }
                    else
                    {
                        EnterEncounterState(EncounterState.INTRO);
                        OnIntro.Invoke();
                        SetEncounterState(EncounterState.MAIN_EVENT);
                        SequenceHandler.instance.LetPlayerExitSequence();
                    }
                    WriteToData();
                    return;
                case EncounterState.MAIN_EVENT:
                    EnterEncounterState(Encounter.EncounterState.MAIN_EVENT);
                    OnStart.Invoke();
                    StartMainEvent();
                    return;
                case EncounterState.MAIN_EVENT_SUCCES_DECAY:
                    win = true;
                    EnterEncounterState(EncounterState.MAIN_EVENT_SUCCES_DECAY);
                    return;
                case EncounterState.MAIN_EVENT_FAILED_DECAY:
                    EnterEncounterState(EncounterState.MAIN_EVENT_FAILED_DECAY);
                    return;
                case EncounterState.OUTRO_SUCCES:
                    if (outroSuccesSequence != null)
                    {
                        handler.StartEnteringSequence(outroSuccesSequence, this, turnOffPlayerDuringSequences, true, false, false, true, null, true, lowerVolumeDuringSequence, null, false);
                        return;
                    }
                    EnterEncounterState(EncounterState.OUTRO_SUCCES);
                    OnOutro.Invoke();
                    if (WorldHandler.instance.currentEncounter == this)
                    {
                        WorldHandler.instance.MakeStuffAvailableAgainCauseEncounterOver();
                    }
                    SetEncounterState(EncounterState.CLOSED);
                    Complete(null);
                    return;
                case EncounterState.OUTRO_FAIL:
                    if (outroFailSequence != null)
                    {
                        handler.StartEnteringSequence(outroFailSequence, this, turnOffPlayerDuringSequences, true, false, false, true, null, true, lowerVolumeDuringSequence, null, false);
                        return;
                    }
                    EnterEncounterState(Encounter.EncounterState.OUTRO_FAIL);
                    OnOutro.Invoke();
                    Fail(null);
                    if (restartImmediatelyOnFail)
                    {
                        ActivateEncounterInstantIntro();
                        return;
                    }
                    if (WorldHandler.instance.currentEncounter == this)
                    {
                        WorldHandler.instance.MakeStuffAvailableAgainCauseEncounterOver();
                    }
                    SetEncounterState(EncounterState.CLOSED);
                    break;
                default:
                    return;
            }
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
