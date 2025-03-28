using BombRushMP.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Server
{
    public static class AprilServer
    {
        public const string AprilTag = AprilCommon.Tag;

        public static bool GetAprilEventEnabled()
        {
            return BRCServer.Instance.ServerState.Tags.Contains(AprilTag);
        }
    }
}
