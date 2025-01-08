using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common.Networking
{
    public class ConnectionFailedEventArgs
    {
        public readonly RejectReason Reason;
        public ConnectionFailedEventArgs(RejectReason reason)
        {
            Reason = reason;
        }
    }
}
