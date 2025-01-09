using BombRushMP.Common;
using BombRushMP.Common.Networking;
using BombRushMP.Common.Packets;
using BombRushMP.MapStation;
using Reptile;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BombRushMP.Plugin.Gamemodes
{
    public class GraffitiRace : Gamemode
    {
        public bool QuickGraffitiEnabled => (Settings.SettingByID[SettingQuickGraffitiID] as ToggleGamemodeSetting).IsOn;
        public enum SpawnMode
        {
            Automatic,
            At_Host
        }

        private static int SettingSpawnModeID = Animator.StringToHash("SpawnMode");
        private static int SettingQuickGraffitiID = Animator.StringToHash("QuickGraffiti");
        private static int SettingGraffitiAmountID = Animator.StringToHash("GraffitiAmount");
        private const int DefaultGraffiti = 10;
        private const int MinGraffiti = 5;
        private const int MaxGraffiti = 70;
        private const int GraffitiSteps = 5;
        private const float DistanceOffWall = 3f;

        public enum States
        {
            Countdown,
            Main,
            Finished
        }
        private enum FetchingStates
        {
            None,
            Started,
            Finished
        }
        public int Score = 0;
        private States _state = States.Countdown;
        private FetchingStates _fetchingState = FetchingStates.None;
        private float _countdownTimer = 0f;
        private WorldHandler _worldHandler;
        private Dictionary<GraffitiSpot, GraffitiSpotProgress> _originalProgress = new();
        private HashSet<GraffitiSpot> _otherSpots = new();
        private UIScreenIndicators _indicators;
        private Dictionary<GraffitiSpot, MapPin> _mapPins = new();

        public GraffitiRace() : base()
        {
            _worldHandler = WorldHandler.instance;
            MinimapOverrideMode = MinimapOverrideModes.ForceOn;
        }

        public override void OnPacketReceived_InGame(Packets packetId, Packet packet)
        {
            var player = _worldHandler.GetCurrentPlayer();
            switch (packetId)
            {
                case Packets.ClientGraffitiRaceData:
                    {
                        var raceData = (ClientGraffitiRaceData)packet;
                        OnReceive_GraffitiRaceData(raceData);
                    }
                    break;

                case Packets.ServerGamemodeBegin:
                    {
                        _state = States.Main;
                        player.userInputEnabled = true;
                    }
                    break;
            }
        }

        public override void OnStart()
        {
            base.OnStart();
            _fetchingState = FetchingStates.None;
            _indicators = UIScreenIndicators.Create();
            var timerUI = TimerUI.Instance;
            timerUI.Activate();
            if (ClientController.LocalID == Lobby.LobbyState.HostId)
                OnStart_Host();
            var player = _worldHandler.GetCurrentPlayer();
            player.boostCharge = player.maxBoostCharge;
            player.userInputEnabled = false;
        }

        public override void OnUpdate_InGame()
        {
            var timerUI = TimerUI.Instance;
            switch (_state)
            {
                case States.Countdown:
                    timerUI.SetText("1");

                    if (_countdownTimer < 2f)
                        timerUI.SetText("2");

                    if (_countdownTimer < 1f)
                        timerUI.SetText("3");
                    break;

                case States.Main:
                    timerUI.SetText($"{Score}/{_originalProgress.Count}");
                    break;
            }
            _countdownTimer += Time.deltaTime;
        }

        public bool IsRaceGraffitiSpot(GraffitiSpot grafSpot)
        {
            return _originalProgress.ContainsKey(grafSpot) && grafSpot.topCrew == Crew.NONE;
        }

        public void MarkGraffitiSpotDone(GraffitiSpot grafSpot)
        {
            var mapController = Mapcontroller.Instance;
            _indicators.RemoveIndicator(grafSpot.transform);
            if (_mapPins.TryGetValue(grafSpot, out var pin))
            {
                mapController.m_MapPins.Remove(pin);
                pin.isMapPinValid = false;
                pin.DisableMapPinGameObject();
                GameObject.Destroy(pin.gameObject);
                _mapPins.Remove(grafSpot);
            }
        }

        public void AddScore()
        {
            Score++;
            var packet = new ClientGamemodeScore(Score);
            ClientController.SendPacket(packet, IMessage.SendModes.Reliable);
        }

        public override void OnEnd(bool cancelled)
        {
            base.OnEnd(cancelled);
            _fetchingState = FetchingStates.None;
            var mapController = Mapcontroller.Instance;
            foreach (var pin in _mapPins.Values)
            {
                if (pin == null) continue;
                if (mapController != null)
                    mapController.m_MapPins.Remove(pin);
                pin.isMapPinValid = false;
                pin.DisableMapPinGameObject();
                GameObject.Destroy(pin.gameObject);
            }
            if (_indicators != null)
            {
                GameObject.Destroy(_indicators.gameObject);
            }
            foreach(var spot in _otherSpots)
            {
                if (spot == null) continue;
                spot.gameObject.SetActive(true);
            }
            foreach(var og in _originalProgress)
            {
                if (og.Key == null) continue;
                og.Key.progressableData = og.Value;
                og.Key.ReadFromData();
            }
            if (_state == States.Countdown)
            {
                var player = _worldHandler.GetCurrentPlayer();
                if (player != null)
                    player.userInputEnabled = true;
            }
            var timerUI = TimerUI.Instance;
            if (timerUI != null)
                timerUI.DeactivateDelayed();
            if (!cancelled)
            {
                Core.Instance.AudioManager.PlaySfxUI(SfxCollectionID.EnvironmentSfx, AudioClipID.MascotUnlock);
            }
        }

        private GraffitiSpot GetGraffitiSpotByUID(string uid)
        {
            var spots = _worldHandler.SceneObjectsRegister.grafSpots;
            foreach(var spot in spots)
            {
                if (spot.Uid == uid)
                    return spot;
            }
            return null;
        }

        private void OnReceive_GraffitiRaceData(ClientGraffitiRaceData packet)
        {
            if (_fetchingState != FetchingStates.Started)
            {
                var player = WorldHandler.instance.GetCurrentPlayer();
                player.CompletelyStop();
                _worldHandler.PlaceCurrentPlayerAt(packet.SpawnPosition.ToUnityVector3(), packet.SpawnRotation.ToUnityQuaternion(), true);
                player.SetPosAndRotHard(packet.SpawnPosition.ToUnityVector3(), packet.SpawnRotation.ToUnityQuaternion());
                _otherSpots.Clear();
                _originalProgress.Clear();
                foreach (var spot in _worldHandler.SceneObjectsRegister.grafSpots)
                {
                    spot.gameObject.SetActive(false);
                    _otherSpots.Add(spot);
                }
            }
            _fetchingState = FetchingStates.Started;
            if (packet.FinalPacket)
                _fetchingState = FetchingStates.Finished;
            var mapController = Mapcontroller.Instance;
            
            foreach(var spot in _worldHandler.SceneObjectsRegister.grafSpots)
            {
                if (packet.GraffitiSpots.Contains(spot.Uid))
                {
                    spot.gameObject.SetActive(true);
                    _otherSpots.Remove(spot);
                }
            }
            foreach(var uid in packet.GraffitiSpots)
            {
                var grafSpot = GetGraffitiSpotByUID(uid);
                _originalProgress[grafSpot] = (GraffitiSpotProgress)grafSpot.progressableData;
                grafSpot.ClearPaint();
                grafSpot.bottomCrew = Crew.PLAYERS;
                _indicators.AddIndicator(grafSpot.transform);

                var pin = mapController.CreatePin(MapPin.PinType.StoryObjectivePin);
                pin.AssignGameplayEvent(grafSpot.gameObject);
                pin.InitMapPin(MapPin.PinType.StoryObjectivePin);
                pin.OnPinEnable();
                _mapPins[grafSpot] = pin;
            }
        }

        private List<GraffitiSpot> GetValidSpots()
        {
            return _worldHandler.SceneObjectsRegister.grafSpots.Where(
               x =>
               x.deactivateDuringEncounters == false &&
               x is not GraffitiSpotFinisher &&
               x.attachedTo == GraffitiSpot.AttachType.DEFAULT &&
               x.notAllowedToPaint != PlayerType.HUMAN &&
               x.GetComponentInParent<Rigidbody>(true) == null &&
               x.beTargetForObjective == Story.ObjectiveID.NONE &&
               x.GetComponentInParent<ActiveOnChapter>(true) == null &&
               !MapStationSupport.IsMapOptionToggleable(x.gameObject)
            ).ToList();
        }

        private void OnStart_Host()
        {
            var validSpots = GetValidSpots();

            var raceSpots = new List<string>();

            var spotAmount = Settings.SettingByID[SettingGraffitiAmountID].Value;
            var spawnMode = (SpawnMode)Settings.SettingByID[SettingSpawnModeID].Value;

            if (validSpots.Count <= 0)
            {
                ClientLobbyManager.EndGame();
                return;
            }

            if (validSpots.Count < spotAmount)
                spotAmount = Mathf.CeilToInt(validSpots.Count * 0.5f);

            for (var i = 0; i < spotAmount; i++)
            {
                var index = UnityEngine.Random.Range(0, validSpots.Count);
                var spot = validSpots[index];
                validSpots.RemoveAt(index);
                raceSpots.Add(spot.Uid);
            }

            var spawnSpot = GetGraffitiSpotByUID(raceSpots[0]);


            var spawnForward = -spawnSpot.transform.forward;
            var spawnPosition = spawnSpot.transform.position - (spawnForward * DistanceOffWall);
            spawnForward.y = 0f;
            spawnForward = spawnForward.normalized;
            var spawnRotation = Quaternion.LookRotation(spawnForward, Vector3.up);

            if (spawnMode == SpawnMode.Automatic)
            {
                foreach (var raceSpot in raceSpots)
                {
                    var grafSpot = GetGraffitiSpotByUID(raceSpot);
                    var spotFw = -grafSpot.transform.forward;
                    var spotPos = grafSpot.transform.position - (spotFw * DistanceOffWall);

                    var ray = new Ray(spotPos, Vector3.down);

                    if (Physics.Raycast(ray, out var hit, 1000f, 0x7FFFFFFF, QueryTriggerInteraction.Collide))
                    {
                        if (Vector3.Angle(hit.normal, Vector3.up) <= 20f && hit.collider.gameObject.layer == Layers.Default)
                        {
                            spawnPosition = hit.point;
                            spawnForward = -grafSpot.transform.forward;
                            spawnForward.y = 0f;
                            spawnForward = spawnForward.normalized;
                            spawnRotation = Quaternion.LookRotation(spawnForward, Vector3.up);
                            break;
                        }
                    }
                }
            }
            else
            {
                var player = WorldHandler.instance.GetCurrentPlayer();
                spawnPosition = player.transform.position;
                spawnForward = player.transform.forward;
                spawnRotation = player.transform.rotation;
            }

            var spotLists = new List<List<string>>();
            var curSpotAmount = 0;
            var currentList = new List<string>();
            foreach(var raceSpot in raceSpots)
            {
                if (curSpotAmount >= ClientGraffitiRaceData.MaxGraffitiSpotsPerPacket)
                {
                    spotLists.Add(currentList);
                    currentList = new List<string>();
                    curSpotAmount = 0;
                }
                currentList.Add(raceSpot);
                curSpotAmount++;
            }
            spotLists.Add(currentList);

            for (var i = 0; i < spotLists.Count; i++)
            {
                var spotList = spotLists[i];
                var final = i >= spotLists.Count - 1;
                var packet = new ClientGraffitiRaceData(spawnPosition.ToSystemVector3(), spawnRotation.ToSystemQuaternion(), spotList, final);
                ClientController.SendPacket(packet, IMessage.SendModes.Reliable);
            }
        }

        public override GamemodeSettings GetDefaultSettings()
        {
            var settings = new GamemodeSettings();
            settings.SettingByID[SettingSpawnModeID] = new GamemodeSetting("Spawn Mode", SpawnMode.Automatic);
            settings.SettingByID[SettingQuickGraffitiID] = new ToggleGamemodeSetting("Quick Graffiti", true);
            var maxSpots = GetValidSpots().Count;
            var minSpots = MinGraffiti;
            if (maxSpots < MinGraffiti)
                minSpots = 0;
            settings.SettingByID[SettingGraffitiAmountID] = new GamemodeSetting("Graffiti Amount", DefaultGraffiti, minSpots, maxSpots, 5);
            return settings;
        }
    }
}
