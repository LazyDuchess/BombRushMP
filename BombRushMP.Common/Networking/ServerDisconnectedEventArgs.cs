using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common.Networking
{
    public class ServerDisconnectedEventArgs
    {
        public readonly INetConnection Client;
        public readonly string Reason;
        public ServerDisconnectedEventArgs(INetConnection client, string reason)
        {
            Client = client;
            Reason = reason;
        }
    }
}
