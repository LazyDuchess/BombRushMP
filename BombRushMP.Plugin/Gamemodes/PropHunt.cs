using BombRushMP.Common.Networking;
using BombRushMP.Common.Packets;
using Reptile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BombRushMP.Plugin.Gamemodes
{
    public class PropHunt : Gamemode
    {
        public enum States
        {
            Setup,
            Main,
            Finished
        }

        private static int SettingSetupDurationID = Animator.StringToHash("SetupDuration");
        private const int DefaultSetupDuration = 1;
        private const int MinSetupDuration = 1;
        private const int MaxSetupDuration = 10;

        private static int SettingMatchDurationID = Animator.StringToHash("MatchDuration");
        private const int DefaultMatchDuration = 5;
        private const int MinMatchDuration = 1;
        private const int MaxMatchDuration = 20;

        private static int SettingPingIntervalID = Animator.StringToHash("PingInterval");
        private const int DefaultPingInterval = 30;
        private const int MinPingInterval = 10;
        private const int MaxPingInterval = 60;

        private static int SettingBecomeHunterOnKillID = Animator.StringToHash("BecomeHunterOnKill");
        private static int SettingRespawnOnKillID = Animator.StringToHash("RespawnOnKill");

        public int SetupDuration => Settings.SettingByID[SettingSetupDurationID].Value;
        public int MatchDuration => Settings.SettingByID[SettingMatchDurationID].Value;
        public int PingInterval => Settings.SettingByID[SettingPingIntervalID].Value;
        public bool BecomeHunterOnKill => (Settings.SettingByID[SettingBecomeHunterOnKillID] as ToggleGamemodeSetting).IsOn;
        public bool RespawnOnKill => (Settings.SettingByID[SettingRespawnOnKillID] as ToggleGamemodeSetting).IsOn;

        private States _state = States.Setup;
        private float _stateTimer = 0f;

        public PropHunt() : base()
        {
            TeamBased = true;
            CanChangeCountdown = false;
            MinimapOverrideMode = MinimapOverrideModes.ForceOn;
        }

        public override void OnStart()
        {
            base.OnStart();
            var timerUI = TimerUI.Instance;
            timerUI.Activate();
            var propDisguiseController = PropDisguiseController.Instance;
            propDisguiseController.FreezeProps();
            propDisguiseController.InPropHunt = true;
            propDisguiseController.InSetupPhase = true;
            propDisguiseController.LocalPropHuntTeam = (PropHuntTeams)Lobby.LobbyState.Players[ClientController.LocalID].Team;
            var player = WorldHandler.instance.GetCurrentPlayer();
            if (propDisguiseController.LocalPropHuntTeam == PropHuntTeams.Hunters)
                player.userInputEnabled = false;
            player.gameObject.AddComponent<PropHuntPlayer>();
            XHairUI.Create();
            if (ClientController.LocalID == Lobby.LobbyState.HostId)
                OnStart_Host();
            else
                OnStart_Client();
        }

        private void OnStart_Client()
        {
            var stageHashPacket = new ClientPropHuntStageHash(PropDisguiseController.Instance.StageHash);
            ClientController.SendPacket(stageHashPacket, IMessage.SendModes.Reliable, NetChannels.Gamemodes);
        }

        private void OnStart_Host()
        {
            var propHuntPacket = new ClientPropHuntSettings((float)SetupDuration * 60f, (float)MatchDuration * 60f, (float)PingInterval, PropDisguiseController.Instance.StageHash, BecomeHunterOnKill, RespawnOnKill, 5f);
            ClientController.SendPacket(propHuntPacket, IMessage.SendModes.Reliable, NetChannels.Gamemodes);
        }

        public override void OnPacketReceived_InGame(Packets packetId, Packet packet)
        {
            switch (packetId)
            {
                case Packets.ServerPropHuntBegin:
                    SetState(States.Main);
                    break;

                case Packets.ServerPropHuntSpawn:
                    var spawnPacket = packet as ServerPropHuntSpawn;
                    MPUtility.PlaceCurrentPlayer(spawnPacket.Position.ToUnityVector3(), spawnPacket.Rotation.ToUnityQuaternion());
                    SetSpawnLocation();
                    break;

                case Packets.ServerPropHuntPing:
                    PingProps();
                    break;

                case Packets.ClientPropHuntShoot:
                    var player = WorldHandler.instance.GetCurrentPlayer();
                    player.ChangeHP(2);
                    break;

                case Packets.ServerPropHuntRespawn:
                    Respawn();
                    break;
            }
        }

        public void OnDie()
        {
            var diePacket = new ClientPropHuntDeath();
            ClientController.SendPacket(diePacket, IMessage.SendModes.Reliable, NetChannels.Gamemodes);
            SpectatorController.StartSpectating(true);
            PlayerComponent.GetLocal().LocalIgnore = true;
        }

        private void Respawn()
        {
            var spec = SpectatorController.Instance;
            if (spec != null)
            {
                spec.EndSpectating();
            }
            var player = WorldHandler.instance.GetCurrentPlayer();
            player.ResetHP();
            MPUtility.PlaceCurrentPlayer(SpawnPos, SpawnRot);
            PlayerComponent.GetLocal().LocalIgnore = false;
        }

        private void PingProps()
        {
            foreach(var player in Lobby.LobbyState.Players)
            {
                if (player.Value.Team != (byte)PropHuntTeams.Props) continue;
                var go = WorldHandler.instance.GetCurrentPlayer().gameObject;
                if (player.Key != ClientController.LocalID)
                {
                    var ply = ClientController.Players[player.Key];
                    if (ply.ClientVisualState != null && ply.ClientVisualState.Ignore) continue;
                    if (ply.Player == null) continue;
                    go = ply.Player.gameObject;
                }
                MPUtility.PingInMap(go, 3f);
            }
        }

        private void SetState(States newState)
        {
            if (newState == _state) return;
            if (newState != States.Setup)
            {
                var propDisguiseController = PropDisguiseController.Instance;
                propDisguiseController.InSetupPhase = false;
                var player = WorldHandler.instance.GetCurrentPlayer();
                player.userInputEnabled = true;
            }
            _state = newState;
            _stateTimer = 0f;
        }

        public override void OnUpdate_InGame()
        {
            var propDisguiseController = PropDisguiseController.Instance;
            propDisguiseController.LocalPropHuntTeam = (PropHuntTeams)Lobby.LobbyState.Players[ClientController.LocalID].Team;
            var timerUI = TimerUI.Instance;
            switch (_state)
            {
                case States.Setup:
                    {
                        var diff = (SetupDuration * 60f) - _stateTimer;
                        if (diff <= 0f)
                            diff = 0f;
                        timerUI.SetText($"Setup: {timerUI.GetTimeString(diff)}");
                    }
                    break;

                case States.Main:
                    {
                        var diff = (MatchDuration * 60f) - _stateTimer;
                        if (diff <= 0f)
                            diff = 0f;
                        timerUI.SetTime(diff);
                    }
                    break;
            }
            _stateTimer += Time.deltaTime;
        }

        public override void OnEnd(bool cancelled)
        {
            var player = WorldHandler.instance.GetCurrentPlayer();
            player.ResetHP();
            MPUtility.PlaceCurrentPlayer(player.transform.position, player.transform.rotation);
            base.OnEnd(cancelled);
            if (_state == States.Setup)
            {
                player.userInputEnabled = true;
            }
            var propDisguiseController = PropDisguiseController.Instance;
            propDisguiseController.UnfreezeProps();
            propDisguiseController.InPropHunt = false;
            propDisguiseController.InSetupPhase = false;
            propDisguiseController.LocalPropHuntTeam = PropHuntTeams.None;
            var playerComp = PlayerComponent.GetLocal();
            playerComp.LocalIgnore = false;
            playerComp.RemovePropDisguise();
            var localPropHuntPlayer = PropHuntPlayer.GetLocal();
            if (localPropHuntPlayer != null)
                GameObject.Destroy(localPropHuntPlayer);
            if (XHairUI.Instance != null)
            {
                GameObject.Destroy(XHairUI.Instance.gameObject);
            }
            var timerUI = TimerUI.Instance;
            if (timerUI != null)
                timerUI.DeactivateDelayed();
        }

        public override GamemodeSettings GetDefaultSettings()
        {
            var settings = base.GetDefaultSettings();
            settings.SettingByID[SettingSetupDurationID] = new GamemodeSetting("Setup Duration (Minutes)", DefaultSetupDuration, MinSetupDuration, MaxSetupDuration);
            settings.SettingByID[SettingMatchDurationID] = new GamemodeSetting("Match Duration (Minutes)", DefaultMatchDuration, MinMatchDuration, MaxMatchDuration);
            settings.SettingByID[SettingPingIntervalID] = new GamemodeSetting("Ping Interval (Seconds)", DefaultPingInterval, MinPingInterval, MaxPingInterval, 10);
            settings.SettingByID[SettingBecomeHunterOnKillID] = new ToggleGamemodeSetting("Props Become Hunters on Death", true);
            settings.SettingByID[SettingRespawnOnKillID] = new ToggleGamemodeSetting("Hunters Respawn on Death", true);
            return settings;
        }
    }
}
