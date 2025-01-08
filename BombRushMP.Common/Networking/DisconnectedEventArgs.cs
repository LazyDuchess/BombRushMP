using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common.Networking
{
    public class DisconnectedEventArgs
    {
        public readonly string Reason;

        public DisconnectedEventArgs(string reason)
        {
            Reason = reason;
        }
    }
}
