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
        public readonly DisconnectReason Reason;
        public ServerDisconnectedEventArgs(INetConnection client, DisconnectReason reason)
        {
            Client = client;
            Reason = reason;
        }
    }
}
