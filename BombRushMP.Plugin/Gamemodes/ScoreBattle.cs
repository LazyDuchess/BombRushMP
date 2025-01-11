using BombRushMP.Common;
using BombRushMP.Common.Packets;
using Reptile;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using BombRushMP.Common.Networking;
using TMPro;

namespace BombRushMP.Plugin.Gamemodes
{
    public class ScoreBattle : Gamemode
    {
        public enum SpawnMode
        {
            Current_Positions,
            At_Host
        }
        private static int SettingSpawnModeID = Animator.StringToHash("SpawnMode");
        public enum States
        {
            Countdown,
            Main,
            Finished
        }
        private States _state = States.Countdown;
        private float _stateTimer = 0f;
        private DateTime _startTime = DateTime.UtcNow;
        public bool ComboBased = false;
        private bool _comboOverRegistered = false;

        public ScoreBattle() : base() { }

        public override void OnStart()
        {
            base.OnStart();
            var spawnMode = (SpawnMode)Settings.SettingByID[SettingSpawnModeID].Value;
            if (spawnMode == SpawnMode.At_Host)
            {
                var host = ClientController.Instance.Players[Lobby.LobbyState.HostId].ClientVisualState;
                MPUtility.PlaceCurrentPlayer(host.Position.ToUnityVector3(), host.Rotation.ToUnityQuaternion());
            }
            TimerUI.Instance.Activate();
        }

        public override void OnUpdate_InGame()
        {
            var player = WorldHandler.instance.GetCurrentPlayer();
            var timerUI = TimerUI.Instance;
            switch (_state)
            {
                case States.Countdown:
                    timerUI.SetText("1");

                    if (_stateTimer < 2f)
                        timerUI.SetText("2");

                    if (_stateTimer < 1f)
                        timerUI.SetText("3");
                    break;

                case States.Main:
                    var timeElapsed = (float)(DateTime.UtcNow - _startTime).TotalSeconds;
                    var timeLeft = Constants.ScoreBattleDuration - timeElapsed;
                    if (timeLeft <= 0f)
                    {
                        timeLeft = 0f;
                        if (ComboBased && !player.IsComboing())
                        {
                            if (!_comboOverRegistered)
                            {
                                _comboOverRegistered = true;
                                ClientController.SendPacket(new ClientComboOver(player.score), IMessage.SendModes.ReliableUnordered);
                            }
                        }
                    }
                    timerUI.SetTime(timeLeft);
                    if (timeLeft <= 0f && ComboBased && (player.IsComboing() || _comboOverRegistered))
                    {
                        if (player.IsComboing() && !_comboOverRegistered)
                            timerUI.SetText("Overtime");
                        else if (!player.IsComboing() && _comboOverRegistered)
                            timerUI.SetText("Waiting for other players...");
                    }
                    break;
            }
            _stateTimer += Time.deltaTime;
        }

        private void BeginMainEvent()
        {
            var player = WorldHandler.instance.GetCurrentPlayer();
            if (player.IsComboing())
            {
                player.scoreMultiplier = 1f;
            }
            else
            {
                player.scoreMultiplier = 0f;
                player.tricksInCombo = 1;
            }

            player.boostCharge = player.maxBoostCharge;
            player.lastScore = 0f;
            player.lastCornered = 0;
            player.ClearMultipliersDone();
            player.comboTimeOutTimer = 1f;
            player.RefreshAirTricks();
            player.RefreshAllDegrade();
            player.score = 0f;
            player.baseScore = 0f;
            
            player.grindAbility.trickTimer = 0f;
            _state = States.Main;
        }

        public override void OnEnd(bool cancelled)
        {
            base.OnEnd(cancelled);
            if (TimerUI.Instance != null)
            {
                TimerUI.Instance.SetText("Finish!");
                TimerUI.Instance.DeactivateDelayed();
            }
            if (WorldHandler.instance != null)
            {
                var player = WorldHandler.instance.GetCurrentPlayer();
                if (!cancelled)
                {
                    Core.Instance.AudioManager.PlaySfxUI(SfxCollectionID.EnvironmentSfx, AudioClipID.MascotUnlock);
                    if (Lobby.LobbyState.Players.Count >= ClientConstants.MinimumPlayersToCheer)
                    {
                        var winner = Lobby.GetHighestScoringPlayer();
                        if (winner.Id == ClientController.Instance.LocalID && winner.Score >= ClientConstants.MinimumScoreToCheer)
                        {
                            if (player != null)
                                player.StartCoroutine(Cheer(player));
                        }
                    }
                }
                if (player != null)
                    player.userInputEnabled = true;
            }
        }

        private IEnumerator Cheer(Player player)
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.5f,1f));
            if (player != null && !player.IsDead() && !player.IsBusyWithSequence())
                player.PlayVoice(AudioClipID.VoiceBoostTrick, VoicePriority.COMBAT, true);
        }

        public override void OnPacketReceived_InGame(Packets packetId, Packet packet)
        {
            switch(packetId){
                case Packets.ServerGamemodeBegin:
                    var beginPacket = (ServerGamemodeBegin)packet;
                    _startTime = beginPacket.StartTime;
                    BeginMainEvent();
                    break;
            }
        }

        public override void OnTick_InGame()
        {
            if (_state == States.Main)
            {
                var player = WorldHandler.instance.GetCurrentPlayer();
                var score = player.score;
                if (player.IsComboing() && !ComboBased)
                    score += player.baseScore * player.scoreMultiplier;
                ClientController.SendPacket(new ClientGamemodeScore(score), IMessage.SendModes.Reliable);
            }
        }

        public override GamemodeSettings GetDefaultSettings()
        {
            var settings = new GamemodeSettings();
            settings.SettingByID[SettingSpawnModeID] = new GamemodeSetting("Spawn Mode", SpawnMode.Current_Positions);
            return settings;
        }
    }
}
