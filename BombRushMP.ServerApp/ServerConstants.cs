using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.ServerApp
{
    public static class ServerConstants
    {
        public const string JoinMessage = "{0} Connected.";
        public const string LeaveMessage = "{0} Disconnected.";
        public const float PlayerCountTickRate = 1f;
    }
}
