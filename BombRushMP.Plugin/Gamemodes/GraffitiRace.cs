using Reptile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BombRushMP.Plugin.Gamemodes
{
    public class GraffitiRace : Gamemode
    {
        private WorldHandler _worldHandler;
        private Dictionary<GraffitiSpot, GraffitiSpotProgress> _originalProgress = new();
        public GraffitiRace() : base()
        {
            _worldHandler = WorldHandler.instance;
        }

        public override void OnStart()
        {
            if (ClientController.LocalID == Lobby.LobbyState.HostId)
                OnStart_Host();
        }

        public override void OnEnd(bool cancelled)
        {
            foreach(var og in _originalProgress)
            {
                og.Key.progressableData = og.Value;
                og.Key.ReadFromData();
            }
        }

        private void OnStart_Host()
        {
            var validSpots = _worldHandler.SceneObjectsRegister.grafSpots.Where(
               x =>
               x.deactivateDuringEncounters == false &&
               x is not GraffitiSpotFinisher &&
               x.attachedTo == GraffitiSpot.AttachType.DEFAULT &&
               x.notAllowedToPaint != PlayerType.HUMAN
            ).ToList();

            var raceSpots = new List<GraffitiSpot>();

            for(var i = 0; i < 10; i++)
            {
                var index = UnityEngine.Random.Range(0, validSpots.Count);
                var spot = validSpots[index];
                validSpots.RemoveAt(index);
                raceSpots.Add(spot);
            }

            foreach(var spot in raceSpots)
            {
                _originalProgress[spot] = (GraffitiSpotProgress)spot.progressableData;
                spot.ClearPaint();
                spot.bottomCrew = Crew.PLAYERS;
            }

            var spawnSpot = raceSpots[0];
            var spawnPosition = raceSpots[0].transform.position;
            var spawnForward = -raceSpots[0].transform.forward;
            spawnForward.y = 0f;
            spawnForward = spawnForward.normalized;
            var spawnRotation = Quaternion.LookRotation(spawnForward, Vector3.up);
            _worldHandler.PlaceCurrentPlayerAt(spawnPosition, spawnRotation, true);
        }
    }
}
