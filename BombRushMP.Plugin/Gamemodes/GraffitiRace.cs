using BombRushMP.Common;
using BombRushMP.Common.Networking;
using BombRushMP.Common.Packets;
using BombRushMP.MapStation;
using Reptile;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BombRushMP.Plugin.Gamemodes
{
    public class GraffitiRace : Gamemode
    {
        public bool QuickGraffitiEnabled => (Settings.SettingByID[SettingQuickGraffitiID] as ToggleGamemodeSetting).IsOn;
        public bool AutoGraffitiEnabled => (Settings.SettingByID[SettingAutoGraffitiID] as ToggleGamemodeSetting).IsOn;
        public enum SpawnMode
        {
            Automatic,
            At_Host,
            Current_Positions
        }

        public enum MoveStyleMode
        {
            Force_MoveStyle,
            Force_On_Foot,
            Current
        }

        private static int SettingMoveStyleID = Animator.StringToHash("MoveStyle");
        private static int SettingSpawnModeID = Animator.StringToHash("SpawnMode");
        private static int SettingQuickGraffitiID = Animator.StringToHash("QuickGraffiti");
        private static int SettingAutoGraffitiID = Animator.StringToHash("AutoGraffiti");
        private static int SettingGraffitiAmountID = Animator.StringToHash("GraffitiAmount");
        private static int SettingSingleGraffitiID = Animator.StringToHash("SingleGraffiti");
        private const int DefaultGraffiti = 10;
        private const int MinGraffiti = 5;
        private const int MaxGraffiti = 70;
        private const int GraffitiSteps = 5;
        private const float DistanceOffWall = 3f;
        private List<StageChunk> _offStageChunks = new();

        private void RestoreStageChunks()
        {
            foreach(var chunk in _offStageChunks)
            {
                chunk.gameObject.SetActive(false);
            }
            _offStageChunks.Clear();
        }

        private void TurnOnAllStageChunks()
        {
            _offStageChunks.Clear();
            var stageChunks = WorldHandler.instance.SceneObjectsRegister.stageChunks;
            foreach (var chunk in stageChunks)
            {
                if (!chunk.gameObject.activeSelf)
                {
                    _offStageChunks.Add(chunk);
                    chunk.gameObject.SetActive(true);
                }
            }
        }

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
                case Packets.ClientGraffitiRaceGSpots:
                    {
                        var raceData = (ClientGraffitiRaceGSpots)packet;
                        OnReceive_GraffitiRaceData(raceData);
                    }
                    break;

                case Packets.ClientGraffitiRaceStart:
                    {
                        var startData = (ClientGraffitiRaceStart)packet;
                        OnReceive_GraffitiStart(startData);
                    }
                    break;

                case Packets.ServerGamemodeBegin:
                    {
                        _state = States.Main;
                        player.userInputEnabled = true;
                    }
                    break;

                case Packets.ServerTeamGraffRaceScore:
                    {
                        OnReceive_TeamGraffitiRaceScore((ServerTeamGraffRaceScore)packet);
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
            if (Settings.SettingByID[SettingMoveStyleID].Value == (int)MoveStyleMode.Force_MoveStyle)
                player.SwitchToEquippedMovestyle(true);
            else if (Settings.SettingByID[SettingMoveStyleID].Value == (int)MoveStyleMode.Force_On_Foot)
                player.SwitchToEquippedMovestyle(false);
        }

        public override void OnUpdate_InGame()
        {
            var timerUI = TimerUI.Instance;
            switch (_state)
            {
                case States.Countdown:
                    var timerRounded = Mathf.FloorToInt(_countdownTimer);
                    var timerText = Mathf.Max(1, Countdown - timerRounded);
                    timerUI.SetText(timerText.ToString());
                    break;

                case States.Main:
                    var score = Score;
                    if (TeamBased)
                    {
                        score = (int)Lobby.LobbyState.GetScoreForTeam(Lobby.LobbyState.Players[ClientController.LocalID].Team);
                    }
                    timerUI.SetText($"{score}/{_originalProgress.Count}");
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

        public void AddScore(GraffitiSpot gSpot)
        {
            if (!TeamBased)
            {
                Score++;
                var packet = new ClientGamemodeScore(Score);
                ClientController.SendPacket(packet, IMessage.SendModes.Reliable, NetChannels.Gamemodes);
            }
            else
            {
                var packet = new ClientTeamGraffRaceScore(Compression.HashString(gSpot.Uid));
                ClientController.SendPacket(packet, IMessage.SendModes.Reliable, NetChannels.Gamemodes);
            }
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
        }

        private GraffitiSpot GetGraffitiSpotByHash(int hash)
        {
            var spots = _worldHandler.SceneObjectsRegister.grafSpots;
            foreach(var spot in spots)
            {
                if (Compression.HashString(spot.Uid) == hash)
                    return spot;
            }
            return null;
        }

        private void OnReceive_GraffitiStart(ClientGraffitiRaceStart packet)
        {
            var player = WorldHandler.instance.GetCurrentPlayer();
            var spawnPos = packet.SpawnPosition.ToUnityVector3();
            var spawnRot = packet.SpawnRotation.ToUnityQuaternion();
            if (Settings.SettingByID[SettingSpawnModeID].Value == (int)SpawnMode.Current_Positions)
            {
                spawnPos = player.transform.position;
                spawnRot = player.transform.rotation;
            }
            MPUtility.PlaceCurrentPlayer(spawnPos, spawnRot);
            SetSpawnLocation();
        }

        private void OnReceive_TeamGraffitiRaceScore(ServerTeamGraffRaceScore packet)
        {
            var spot = GetGraffitiSpotByHash(packet.TagHash);
            MarkGraffitiSpotDone(spot);
            var playa = ClientController.Players[packet.PlayerId].Player;
            var art = TagUtils.GetRandomGraffitiArt(spot, playa);
            spot.Paint(Crew.PLAYERS, art, null);
        }

        private void OnReceive_GraffitiRaceData(ClientGraffitiRaceGSpots packet)
        {
            if (_fetchingState != FetchingStates.Started)
            {
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
                if (packet.GraffitiSpots.Contains(Compression.HashString(spot.Uid)))
                {
                    spot.gameObject.SetActive(true);
                    _otherSpots.Remove(spot);
                }
            }
            foreach(var hash in packet.GraffitiSpots)
            {
                var grafSpot = GetGraffitiSpotByHash(hash);
                if (grafSpot == null)
                {
                    ClientLobbyManager.LeaveLobby();
                    ChatUI.Instance.AddMessage("Kicked from lobby because your map doesn't match with the lobby host's.");
                    return;
                }
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

            var raceSpots = new List<GraffitiSpot>();

            var spotAmount = Settings.SettingByID[SettingGraffitiAmountID].Value;
            var spawnMode = (SpawnMode)Settings.SettingByID[SettingSpawnModeID].Value;
            var singleMode = (Settings.SettingByID[SettingSingleGraffitiID] as ToggleGamemodeSetting).IsOn;

            if ((Settings.SettingByID[SettingSingleGraffitiID] as ToggleGamemodeSetting).IsOn)
                spotAmount = 1;

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
                raceSpots.Add(spot);
            }

            var spawnSpot = raceSpots[0];


            var spawnForward = -spawnSpot.transform.forward;
            var spawnPosition = spawnSpot.transform.position - (spawnForward * DistanceOffWall);
            spawnForward.y = 0f;
            spawnForward = spawnForward.normalized;
            var spawnRotation = Quaternion.LookRotation(spawnForward, Vector3.up);
            var spawnSpots = raceSpots;

            if (singleMode)
            {
                validSpots.Remove(raceSpots[0]);
                validSpots.Shuffle();
                spawnSpots = validSpots;
                if (spawnSpots.Count <= 0)
                    spawnSpots = raceSpots;
            }

            if (spawnMode == SpawnMode.Automatic)
            {
                TurnOnAllStageChunks();
                foreach (var raceSpot in spawnSpots)
                {
                    var grafSpot = raceSpot;
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
                RestoreStageChunks();
            }
            else
            {
                var player = WorldHandler.instance.GetCurrentPlayer();
                spawnPosition = player.transform.position;
                spawnForward = player.transform.forward;
                spawnRotation = player.transform.rotation;
            }

            var startPacket = new ClientGraffitiRaceStart(spawnPosition.ToSystemVector3(), spawnRotation.ToSystemQuaternion());
            ClientController.SendPacket(startPacket, IMessage.SendModes.Reliable, NetChannels.Gamemodes);

            var spotLists = new List<List<int>>();
            var curSpotAmount = 0;
            var currentList = new List<int>();
            foreach(var raceSpot in raceSpots)
            {
                if (curSpotAmount >= ClientGraffitiRaceGSpots.MaxGraffitiSpotsPerPacket)
                {
                    spotLists.Add(currentList);
                    currentList = new List<int>();
                    curSpotAmount = 0;
                }
                currentList.Add(Compression.HashString(raceSpot.Uid));
                curSpotAmount++;
            }
            spotLists.Add(currentList);

            for (var i = 0; i < spotLists.Count; i++)
            {
                var spotList = spotLists[i];
                var final = i >= spotLists.Count - 1;
                var packet = new ClientGraffitiRaceGSpots(spotList, final);
                ClientController.SendPacket(packet, IMessage.SendModes.Reliable, NetChannels.Gamemodes);
            }
        }

        public override GamemodeSettings GetDefaultSettings()
        {
            var settings = base.GetDefaultSettings();
            settings.SettingByID[SettingMoveStyleID] = new GamemodeSetting("Spawn MoveStyle", MoveStyleMode.Force_On_Foot);
            settings.SettingByID[SettingSpawnModeID] = new GamemodeSetting("Spawn Mode", SpawnMode.Automatic);
            settings.SettingByID[SettingQuickGraffitiID] = new ToggleGamemodeSetting("Quick Graffiti", true);
            settings.SettingByID[SettingAutoGraffitiID] = new ToggleGamemodeSetting("Auto Graffiti", false);
            settings.SettingByID[SettingAutoGraffitiID].OnCheckVisibility += () =>
            {
                return QuickGraffitiEnabled;
            };
            var maxSpots = GetValidSpots().Count;
            var minSpots = MinGraffiti;
            if (maxSpots < MinGraffiti)
                minSpots = 0;
            settings.SettingByID[SettingGraffitiAmountID] = new GamemodeSetting("Graffiti Amount", DefaultGraffiti, minSpots, maxSpots, 5);
            settings.SettingByID[SettingSingleGraffitiID] = new ToggleGamemodeSetting("Single Graffiti", false);
            return settings;
        }
    }
}
