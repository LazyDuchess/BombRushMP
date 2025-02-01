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
        public const string NoDamageTag = "nodamage";
        public static bool CanReptilePlayerPvP(Player player)
        {
            if (player == null) return false;
            var mpPlayer = MPUtility.GetMuliplayerPlayer(player);
            if (mpPlayer == null) return false;
            return mpPlayer.ClientState.User.HasTag(PvPTag) || ClientController.Instance.ServerState.Tags.Contains(PvPTag);
        }

        public static bool CanIGetHit()
        {
            var clientController = ClientController.Instance;
            if (clientController == null) return true;
            var user = clientController.GetLocalUser();
            if (user == null) return true;
            return !user.HasTag(CantGetHitTag);
        }

        public static bool CanIGetDamaged()
        {
            var clientController = ClientController.Instance;
            if (clientController == null) return true;
            var user = clientController.GetLocalUser();
            if (user == null) return true;
            return !user.HasTag(NoDamageTag);
        }

        public static void SetHitboxesFromBits(int bits, Player player)
        {
            var hitBoxVal = (1 << 0);
            var hitBoxLeftLegVal = (1 << 1);
            var hitboxRightLegVal = (1 << 2);
            var hitboxUpperVal = (1 << 3);
            var hitboxAerialVal = (1 << 4);
            var hitboxRadialVal = (1 << 5);
            var hitboxSprayVal = (1 << 6);

            if ((bits & hitBoxVal) != 0)
                player.hitbox.SetActive(true);
            else
                player.hitbox.SetActive(false);

            if ((bits & hitBoxLeftLegVal) != 0)
                player.hitboxLeftLeg.SetActive(true);
            else
                player.hitboxLeftLeg.SetActive(false);

            if ((bits & hitboxRightLegVal) != 0)
                player.hitboxRightLeg.SetActive(true);
            else
                player.hitboxRightLeg.SetActive(false);

            if ((bits & hitboxUpperVal) != 0)
                player.hitboxUpperBody.SetActive(true);
            else
                player.hitboxUpperBody.SetActive(false);

            if ((bits & hitboxAerialVal) != 0)
                player.airialHitbox.SetActive(true);
            else
                player.airialHitbox.SetActive(false);

            if ((bits & hitboxRadialVal) != 0)
                player.radialHitbox.SetActive(true);
            else
                player.radialHitbox.SetActive(false);

            if ((bits & hitboxSprayVal) != 0)
                player.sprayHitbox.SetActive(true);
            else
                player.sprayHitbox.SetActive(false);
        }

        public static int HitboxesToBits(Player player)
        {
            var hitBoxVal = (1 << 0);
            var hitBoxLeftLegVal = (1 << 1);
            var hitboxRightLegVal = (1 << 2);
            var hitboxUpperVal = (1 << 3);
            var hitboxAerialVal = (1 << 4);
            var hitboxRadialVal = (1 << 5);
            var hitboxSprayVal = (1 << 6);

            var val = 0;

            if (player.hitbox.activeSelf)
                val |= hitBoxVal;

            if (player.hitboxLeftLeg.activeSelf)
                val |= hitBoxLeftLegVal;

            if (player.hitboxRightLeg.activeSelf)
                val |= hitboxRightLegVal;

            if (player.hitboxUpperBody.activeSelf)
                val |= hitboxUpperVal;

            if (player.airialHitbox.activeSelf)
                val |= hitboxAerialVal;

            if (player.radialHitbox.activeSelf)
                val |= hitboxRadialVal;

            if (player.sprayHitbox)
                val |= hitboxSprayVal;

            return val;
        }
    }
}
