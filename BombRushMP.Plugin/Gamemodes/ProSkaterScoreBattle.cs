using Reptile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static BombRushMP.Plugin.Gamemodes.GraffitiRace;

namespace BombRushMP.Plugin.Gamemodes
{
    public class ProSkaterScoreBattle : ScoreBattle
    {
        public bool RewardTilting => (Settings.SettingByID[SettingRewardTiltingID] as ToggleGamemodeSetting).IsOn;
        private static int SettingRewardTiltingID = Animator.StringToHash("RewardTilting");
        public ProSkaterScoreBattle() : base()
        {
            ComboBased = true;
        }

        public override void OnStart()
        {
            base.OnStart();
            var player = WorldHandler.instance.GetCurrentPlayer();
            player.CompletelyStop();
            ProSkaterPlayer.Set(player, true);
        }

        public override void OnEnd(bool cancelled)
        {
            base.OnEnd(cancelled);
            if (WorldHandler.instance != null)
            {
                var player = WorldHandler.instance.GetCurrentPlayer();
                if (player != null)
                {
                    player.CompletelyStop();
                    ProSkaterPlayer.Set(player, false);
                }
            }
        }

        public override GamemodeSettings GetDefaultSettings()
        {
            var settings = new GamemodeSettings();
            settings.SettingByID[SettingRewardTiltingID] = new ToggleGamemodeSetting("Reward Grind Corners", true);
            return settings;
        }
    }
}
