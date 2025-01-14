using BombRushMP.Common;
using BombRushMP.Common.Packets;
using Reptile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Plugin.Gamemodes
{
    public class Gamemode
    {
        public MinimapOverrideModes MinimapOverrideMode = MinimapOverrideModes.None;
        public Lobby Lobby;
        public bool InGame { get; private set; }
        public GamemodeSettings Settings;
        protected ClientController ClientController;
        protected ClientLobbyManager ClientLobbyManager;

        public Gamemode()
        {
            ClientController = ClientController.Instance;
            ClientLobbyManager = ClientController.ClientLobbyManager;
        }

        public virtual void OnStart()
        {
            InGame = true;
            MPUtility.SetUpPlayerForGameStateUpdate();
            Settings = GamemodeFactory.ParseGamemodeSettings(Lobby.LobbyState.Gamemode, Lobby.LobbyState.GamemodeSettings);
            LobbyUI.Instance.UpdateUI();
        }

        public virtual void OnEnd(bool cancelled)
        {
            InGame = false;
            MPUtility.SetUpPlayerForGameStateUpdate();
            LobbyUI.Instance.UpdateUI();
            if (!cancelled)
                ProcessSpecialPlayerUnlock();
        }

        private void ProcessSpecialPlayerUnlock()
        {
            if (Lobby == null) return;
            if (Lobby.LobbyState == null) return;
            if (Lobby.LobbyState.Players == null) return;
            var mpSaveData = MPSaveData.Instance;
            if (mpSaveData.ShouldDisplayGoonieBoard()) return;

            var winner = Lobby.GetHighestScoringPlayer();
            var user = ClientController.GetUser(winner.Id);
            if (user == null) return;

            if (user.HasTag(SpecialPlayerUtils.SpecialPlayerTag)) return;

            var eliteInLobby = false;
            var eliteScore = 0f;

            foreach(var player in Lobby.LobbyState.Players)
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

            if (!eliteInLobby) return;

            if (!Lobby.LobbyState.Players.TryGetValue(ClientController.LocalID, out var me))
                return;

            if (me.Score > eliteScore)
            {
                ChatUI.Instance.AddMessage(SpecialPlayerUtils.SpecialPlayerUnlockNotification);
                mpSaveData.UnlockedGoonieBoard = true;
                Core.Instance.SaveManager.SaveCurrentSaveSlot();
            }
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
            return new GamemodeSettings();
        }
    }
}
