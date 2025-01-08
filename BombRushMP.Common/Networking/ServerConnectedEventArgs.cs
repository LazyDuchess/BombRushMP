using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common.Networking
{
    public class ServerConnectedEventArgs
    {
        public readonly INetConnection Client;
        public ServerConnectedEventArgs(INetConnection client)
        {
            Client = client;
        }
    }
}
