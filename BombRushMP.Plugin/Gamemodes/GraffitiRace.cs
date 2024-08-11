using BombRushMP.Common;
using BombRushMP.Common.Packets;
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
        public enum States
        {
            Countdown,
            Main,
            Finished
        }
        public int Score = 0;
        private States _state = States.Countdown;
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
            ClientController.SendPacket(packet, Riptide.MessageSendMode.Reliable);
        }

        public override void OnEnd(bool cancelled)
        {
            base.OnEnd(cancelled);
            var mapController = Mapcontroller.Instance;
            foreach (var pin in _mapPins.Values)
            {
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
                spot.gameObject.SetActive(true);
            }
            foreach(var og in _originalProgress)
            {
                og.Key.progressableData = og.Value;
                og.Key.ReadFromData();
            }
            if (_state == States.Countdown)
            {
                var player = _worldHandler.GetCurrentPlayer();
                player.userInputEnabled = true;
            }
            var timerUI = TimerUI.Instance;
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
            var mapController = Mapcontroller.Instance;
            _worldHandler.PlaceCurrentPlayerAt(packet.SpawnPosition.ToUnityVector3(), packet.SpawnRotation.ToUnityQuaternion(), true);
            _otherSpots.Clear();
            _originalProgress.Clear();
            foreach(var spot in _worldHandler.SceneObjectsRegister.grafSpots)
            {
                if (packet.GraffitiSpots.Contains(spot.Uid)) continue;
                spot.gameObject.SetActive(false);
                _otherSpots.Add(spot);
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

        private void OnStart_Host()
        {
            var validSpots = _worldHandler.SceneObjectsRegister.grafSpots.Where(
               x =>
               x.deactivateDuringEncounters == false &&
               x is not GraffitiSpotFinisher &&
               x.attachedTo == GraffitiSpot.AttachType.DEFAULT &&
               x.notAllowedToPaint != PlayerType.HUMAN &&
               x.GetComponentInParent<Rigidbody>(true) == null &&
               x.beTargetForObjective == Story.ObjectiveID.NONE &&
               x.GetComponentInParent<ActiveOnChapter>(true) == null
            ).ToList();

            var raceSpots = new List<string>();

            var spotAmount = ClientController.GraffitiRaceGraffiti;

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
            var spawnPosition = spawnSpot.transform.position - (spawnForward * 1f);
            spawnForward.y = 0f;
            spawnForward = spawnForward.normalized;
            var spawnRotation = Quaternion.LookRotation(spawnForward, Vector3.up);

            var packet = new ClientGraffitiRaceData(spawnPosition.ToSystemVector3(), spawnRotation.ToSystemQuaternion(), raceSpots);
            ClientController.SendPacket(packet, Riptide.MessageSendMode.Reliable);
        }
    }
}
