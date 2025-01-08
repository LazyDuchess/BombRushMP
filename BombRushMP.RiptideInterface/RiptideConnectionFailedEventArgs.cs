using BombRushMP.Common.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.RiptideInterface
{
    public class RiptideConnectionFailedEventArgs : IConnectionFailedEventArgs
    {
        public RejectReason Reason => (RejectReason)RiptideMessageEvent.Reason;

        public Riptide.ConnectionFailedEventArgs RiptideMessageEvent;

        public RiptideConnectionFailedEventArgs(Riptide.ConnectionFailedEventArgs args)
        {
            RiptideMessageEvent = args;
        }
    }
}
