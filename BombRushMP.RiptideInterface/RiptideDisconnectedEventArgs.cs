using BombRushMP.Common.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.RiptideInterface
{
    public class RiptideDisconnectedEventArgs : IDisconnectedEventArgs
    {
        public DisconnectReason Reason => (DisconnectReason)RiptideMessageEvent.Reason;

        public Riptide.DisconnectedEventArgs RiptideMessageEvent;

        public RiptideDisconnectedEventArgs(Riptide.DisconnectedEventArgs args)
        {
            RiptideMessageEvent = args;
        }
    }
}
