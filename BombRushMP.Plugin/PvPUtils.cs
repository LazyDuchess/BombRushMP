using Reptile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Plugin
{
    public static class PvPUtils
    {
        public const string PvPTag = "pvp";
        public const string CantGetHitTag = "canthit";
        public static bool CanReptilePlayerPvP(Player player)
        {
            if (player == null) return false;
            var mpPlayer = MPUtility.GetMuliplayerPlayer(player);
            if (mpPlayer == null) return false;
            return mpPlayer.ClientState.User.HasTag(PvPTag);
        }

        public static bool CanIGetHit()
        {
            var clientController = ClientController.Instance;
            if (clientController == null) return true;
            var user = clientController.GetLocalUser();
            if (user == null) return true;
            return !user.HasTag(CantGetHitTag);
        }
    }
}
