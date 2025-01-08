using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Server
{
    public static class ServerConstants
    {
        public const string JoinMessage = "{0} Connected.";
        public const string LeaveMessage = "{0} Disconnected.";
        public const string AFKMessage = "{0} is now AFK.";
        public const string LeaveAFKMessage = "{0} is no longer AFK.";
        public const float PlayerCountTickRate = 1f;
    }
}
