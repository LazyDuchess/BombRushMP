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
        private const int MinCountdown = 3;
        private const int MaxCountdown = 10;
        public int DefaultCountdown = 3;
        public bool CanChangeCountdown = true;
        public MinimapOverrideModes MinimapOverrideMode = MinimapOverrideModes.None;
        public Lobby Lobby;
        public bool InGame { get; private set; }
        public GamemodeSettings Settings;
        protected int Countdown => CanChangeCountdown ? Settings.SettingByID[SettingCountdownID].Value : DefaultCountdown;
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
            Core.Instance.AudioManager.PlaySfxUI(SfxCollectionID.MenuSfx, AudioClipID.confirm);
            ClientController.SendPacket(new ClientGamemodeCountdown((ushort)Countdown), Common.Networking.IMessage.SendModes.Reliable, Common.Networking.NetChannels.Gamemodes);
        }

        public virtual void OnEnd(bool cancelled)
        {
            InGame = false;
            MPUtility.SetUpPlayerForGameStateUpdate();
            LobbyUI.Instance.UpdateUI();
            if (!cancelled)
            {
                ProcessSpecialPlayerUnlock();
                Core.Instance.AudioManager.PlaySfxUI(SfxCollectionID.EnvironmentSfx, AudioClipID.MascotUnlock);
            }
            else
            {
                Core.Instance.AudioManager.PlaySfxUI(SfxCollectionID.MenuSfx, AudioClipID.cancel);
            }
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
            var settings = new GamemodeSettings();
            if (CanChangeCountdown)
                settings.SettingByID[SettingCountdownID] = new GamemodeSetting("Countdown (Seconds)", DefaultCountdown, MinCountdown, MaxCountdown);
            return settings;
        }
    }
}
