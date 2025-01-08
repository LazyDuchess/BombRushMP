using BombRushMP.Common.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.RiptideInterface
{
    public class RiptideServerDisconnectedEventArgs : IServerDisconnectedEventArgs
    {
        public INetConnection Client => _connection;

        public DisconnectReason Reason => (DisconnectReason)RiptideMessageEvent.Reason;

        public Riptide.ServerDisconnectedEventArgs RiptideMessageEvent;
        private RiptideConnection _connection;
        public RiptideServerDisconnectedEventArgs(Riptide.ServerDisconnectedEventArgs args)
        {
            RiptideMessageEvent = args;
            _connection = new RiptideConnection(args.Client);
        }
    }
}
