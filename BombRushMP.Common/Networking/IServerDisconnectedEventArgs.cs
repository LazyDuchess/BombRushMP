using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common.Networking
{
    public interface IServerDisconnectedEventArgs
    {
        public INetConnection Client { get; }
        public DisconnectReason Reason { get; }
    }
}
