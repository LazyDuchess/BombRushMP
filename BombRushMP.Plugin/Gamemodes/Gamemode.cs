using BombRushMP.Common;
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
    public class Gamemode
    {
        private static int SettingCountdownID = Animator.StringToHash("Countdown");
        private static int SettingTPToSpawnOnEnd = Animator.StringToHash("TPToSpawnOnEnd");
        private const int MinCountdown = 3;
        private const int MaxCountdown = 10;
        public bool TeamBased = false;
        public int DefaultCountdown = 3;
        public bool CanChangeCountdown = true;
        public MinimapOverrideModes MinimapOverrideMode = MinimapOverrideModes.None;
        public Lobby Lobby;
        public bool InGame { get; private set; }
        public GamemodeSettings Settings;
        protected int Countdown => CanChangeCountdown ? Settings.SettingByID[SettingCountdownID].Value : DefaultCountdown;
        protected bool TeleportToSpawnOnEnd => (Settings.SettingByID[SettingTPToSpawnOnEnd] as ToggleGamemodeSetting).IsOn;
        protected ClientController ClientController;
        protected ClientLobbyManager ClientLobbyManager;
        private Vector3 _spawnPos = Vector3.zero;
        private Quaternion _spawnRot = Quaternion.identity;

        public Gamemode()
        {
            ClientController = ClientController.Instance;
            ClientLobbyManager = ClientController.ClientLobbyManager;
        }

        protected void SetSpawnLocation()
        {
            var player = WorldHandler.instance.GetCurrentPlayer();
            _spawnPos = player.transform.position;
            _spawnRot = player.transform.rotation;
            player.saveLocation = player.GenerateSafeLocation();
        }

        public virtual void OnStart()
        {
            InGame = true;
            MPUtility.SetUpPlayerForGameStateUpdate();
            Settings = GamemodeFactory.ParseGamemodeSettings(Lobby.LobbyState.Gamemode, Lobby.LobbyState.GamemodeSettings);
            LobbyUI.Instance.UpdateUI();
            Core.Instance.AudioManager.PlaySfxUI(SfxCollectionID.MenuSfx, AudioClipID.confirm);
            if (Lobby.LobbyState.HostId == ClientController.Instance.LocalID)
                ClientController.SendPacket(new ClientGamemodeCountdown((ushort)Countdown), Common.Networking.IMessage.SendModes.Reliable, Common.Networking.NetChannels.Gamemodes);
            SetSpawnLocation();
        }
        
        protected int GetTeamAmount()
        {
            var teams = new HashSet<byte>();
            foreach(var player in Lobby.LobbyState.Players)
            {
                teams.Add(player.Value.Team);
            }
            return teams.Count;
        }

        public virtual void OnEnd(bool cancelled)
        {
            var saveData = MPSaveData.Instance;
            var gamemode = Lobby.LobbyState.Gamemode;
            InGame = false;
            MPUtility.SetUpPlayerForGameStateUpdate();
            LobbyUI.Instance.UpdateUI();
            if (!cancelled)
            {
                Core.Instance.AudioManager.PlaySfxUI(SfxCollectionID.EnvironmentSfx, AudioClipID.MascotUnlock);

                if (!TeamBased)
                {
                    if (Lobby.LobbyState.Players.Count > 1)
                    {
                        saveData.Stats.IncreaseGamemodesPlayed(gamemode);
                        var winners = GetWinningPlayers();
                        if (winners.Contains(ClientController.LocalID))
                            saveData.Stats.IncreaseGamemodesWon(gamemode);
                    }
                    else
                    {
                        saveData.Stats.IncreaseGamemodesPlayedAlone(gamemode);
                    }
                }
                else
                {
                    if (GetTeamAmount() > 1)
                    {
                        saveData.Stats.IncreaseGamemodesPlayed(gamemode);
                        var winners = GetWinningPlayers();
                        if (winners.Contains(ClientController.LocalID))
                            saveData.Stats.IncreaseGamemodesWon(gamemode);
                    }
                    else
                    {
                        saveData.Stats.IncreaseGamemodesPlayedAlone(gamemode);
                    }
                }

                if (!AmIElite())
                {
                    if (AnyEliteInLobby())
                    {
                        saveData.Stats.ElitesPlayedAgainst++;
                        if (WonAgainstElite())
                        {
                            saveData.Stats.ElitesBeaten++;
                            if (!saveData.ShouldDisplayGoonieBoard())
                            {
                                ChatUI.Instance.AddMessage(SpecialPlayerUtils.SpecialPlayerUnlockNotification);
                                saveData.UnlockedGoonieBoard = true;
                            }
                        }
                    }
                }

                Core.Instance.SaveManager.SaveCurrentSaveSlot();
            }
            else
            {
                Core.Instance.AudioManager.PlaySfxUI(SfxCollectionID.MenuSfx, AudioClipID.cancel);
            }
            if (TeleportToSpawnOnEnd)
                MPUtility.PlaceCurrentPlayer(_spawnPos, _spawnRot);
        }

        private bool AmIElite()
        {
            return ClientController.Instance.GetLocalUser()?.HasTag(SpecialPlayerUtils.SpecialPlayerTag) == true;
        }

        private bool AnyEliteInOppositeTeam()
        {
            var myTeam = Lobby.LobbyState.Players[ClientController.LocalID].Team;
            foreach(var player in Lobby.LobbyState.Players)
            {
                var user = ClientController.GetUser(player.Key);
                if (user.HasTag(SpecialPlayerUtils.SpecialPlayerTag) && player.Value.Team != myTeam)
                    return true;
            }
            return false;
        }

        private bool AnyEliteInLobby()
        {
            foreach(var player in Lobby.LobbyState.Players)
            {
                var user = ClientController.GetUser(player.Key);
                if (user.HasTag(SpecialPlayerUtils.SpecialPlayerTag))
                    return true;
            }
            return false;
        }

        protected List<ushort> GetWinningPlayers()
        {
            if (!TeamBased)
                return new List<ushort>() { Lobby.GetHighestScoringPlayer().Id };
            var ls = new List<ushort>();
            var highestScoringTeam = -1;
            var highestTeamScore = 0f;
            for(var i = 0; i < TeamManager.Teams.Length; i++)
            {
                var teamScore = Lobby.LobbyState.GetScoreForTeam((byte)i);
                if (highestScoringTeam == -1)
                {
                    highestScoringTeam = i;
                    highestTeamScore = teamScore;
                }
                else
                {
                    if (teamScore > highestTeamScore)
                    {
                        highestScoringTeam = i;
                        highestTeamScore = teamScore;
                    }
                }
            }
            foreach(var playa in Lobby.LobbyState.Players)
            {
                if (playa.Value.Team == highestScoringTeam)
                    ls.Add(playa.Key);
            }
            return ls;
        }

        private bool WonAgainstElite()
        {
            if (Lobby == null) return false;
            if (Lobby.LobbyState == null) return false;
            if (Lobby.LobbyState.Players == null) return false;

            var winner = Lobby.GetHighestScoringPlayer();
            var user = ClientController.GetUser(winner.Id);
            if (user == null) return false;

            if (user.HasTag(SpecialPlayerUtils.SpecialPlayerTag)) return false;

            if (TeamBased)
            {
                if (AnyEliteInOppositeTeam())
                {
                    var winners = GetWinningPlayers();
                    if (winners.Contains(ClientController.LocalID))
                        return true;
                }
                return false;
            }

            var eliteInLobby = false;
            var eliteScore = 0f;

            foreach (var player in Lobby.LobbyState.Players)
            {
                user = ClientController.GetUser(player.Key);
                if (user == null) continue;
                if (user.HasTag(SpecialPlayerUtils.SpecialPlayerTag))
                {
                    eliteInLobby = true;
                    if (player.Value.Score > eliteScore)
                    {
                        eliteScore = player.Value.Score;
                    }
                }
            }

            if (!eliteInLobby) return false;

            if (!Lobby.LobbyState.Players.TryGetValue(ClientController.LocalID, out var me))
                return false;

            if (me.Score > eliteScore)
            {
                return true;
            }

            return false;
        }

        public virtual void OnUpdate_InGame()
        {

        }

        public virtual void OnTick_InGame()
        {

        }

        public virtual void OnPacketReceived_InGame(Packets packetId, Packet packet)
        {

        }

        public virtual GamemodeSettings GetDefaultSettings()
        {
            var settings = new GamemodeSettings();
            if (CanChangeCountdown)
                settings.SettingByID[SettingCountdownID] = new GamemodeSetting("Countdown (Seconds)", DefaultCountdown, MinCountdown, MaxCountdown);
            settings.SettingByID[SettingTPToSpawnOnEnd] = new ToggleGamemodeSetting("Teleport to Spawn On End", false);
            return settings;
        }
    }
}
