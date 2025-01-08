using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common.Networking
{
    public class ConnectionFailedEventArgs
    {
        public readonly string Reason;
        public ConnectionFailedEventArgs(string reason)
        {
            Reason = reason;
        }
    }
}
