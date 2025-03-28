using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Plugin
{
    public static class AprilUtils
    {
        public const string AprilTag = "april";

        public static int[] AprilBadges = [
            38,
            47,
            34,
            35,
            25,
            54,
            55,
            56,
            57,
            58,
            59,
            60,
            61,
            62,
            2
            ];

        public static int GetBadgeForName(string name)
        {
            var rand = new Random(name.GetHashCode());
            return AprilBadges[rand.Next(AprilBadges.Length)];
        }

        public static bool GetAprilEventEnabled()
        {
            var clientController = ClientController.Instance;
            if (clientController == null) return false;
            if (clientController.ServerState == null) return false;
            return clientController.ServerState.Tags.Contains(AprilTag);
        }
    }
}
