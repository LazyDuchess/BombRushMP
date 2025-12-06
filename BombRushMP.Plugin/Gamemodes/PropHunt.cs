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
        private static int SettingSetupDurationID = Animator.StringToHash("SetupDuration");
        private const int DefaultSetupDuration = 1;
        private const int MinSetupDuration = 1;
        private const int MaxSetupDuration = 10;

        private static int SettingMatchDurationID = Animator.StringToHash("MatchDuration");
        private const int DefaultMatchDuration = 5;
        private const int MinMatchDuration = 1;
        private const int MaxMatchDuration = 20;

        public PropHunt() : base()
        {
            TeamBased = true;
            CanChangeCountdown = false;
            MinimapOverrideMode = MinimapOverrideModes.ForceOn;
        }

        public override void OnStart()
        {
            base.OnStart();
            var propDisguiseController = PropDisguiseController.Instance;
            propDisguiseController.FreezeProps();
            propDisguiseController.InPropHunt = true;
            propDisguiseController.InSetupPhase = true;
            var player = WorldHandler.instance.GetCurrentPlayer();
            player.gameObject.AddComponent<PropHuntPlayer>();
        }

        public override void OnEnd(bool cancelled)
        {
            base.OnEnd(cancelled);
            var propDisguiseController = PropDisguiseController.Instance;
            propDisguiseController.UnfreezeProps();
            propDisguiseController.InPropHunt = false;
            propDisguiseController.InSetupPhase = false;
            propDisguiseController.LocalPropHuntTeam = PropHuntTeams.None;
            PlayerComponent.GetLocal().RemovePropDisguise();
            var localPropHuntPlayer = PropHuntPlayer.GetLocal();
            if (localPropHuntPlayer != null)
                GameObject.Destroy(localPropHuntPlayer);
        }

        public override GamemodeSettings GetDefaultSettings()
        {
            var settings = base.GetDefaultSettings();
            settings.SettingByID[SettingSetupDurationID] = new GamemodeSetting("Setup Duration (Minutes)", DefaultSetupDuration, MinSetupDuration, MaxSetupDuration);
            settings.SettingByID[SettingMatchDurationID] = new GamemodeSetting("Match Duration (Minutes)", DefaultMatchDuration, MinMatchDuration, MaxMatchDuration);
            return settings;
        }
    }
}
