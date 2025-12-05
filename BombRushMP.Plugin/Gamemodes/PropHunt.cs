using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Plugin.Gamemodes
{
    public class PropHunt : Gamemode
    {
        public PropHunt() : base()
        {
            TeamBased = true;
            CanChangeCountdown = false;
        }

        public override void OnStart()
        {
            base.OnStart();
            var propDisguiseController = PropDisguiseController.Instance;
            propDisguiseController.FreezeProps();
            propDisguiseController.InPropHunt = true;
            propDisguiseController.InSetupPhase = true;
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
        }
    }
}
