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
    }
}
