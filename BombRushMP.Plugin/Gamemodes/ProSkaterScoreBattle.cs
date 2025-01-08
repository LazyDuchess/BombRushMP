using Reptile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Plugin.Gamemodes
{
    public class ProSkaterScoreBattle : ScoreBattle
    {
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
    }
}
