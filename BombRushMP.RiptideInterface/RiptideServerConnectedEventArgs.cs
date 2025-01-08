using BombRushMP.Common.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.RiptideInterface
{
    public class RiptideServerConnectedEventArgs : IServerConnectedEventArgs
    {
        public INetConnection Client => _connection;

        public Riptide.ServerConnectedEventArgs RiptideMessageEvent;
        private RiptideConnection _connection;
        public RiptideServerConnectedEventArgs(Riptide.ServerConnectedEventArgs args)
        {
            RiptideMessageEvent = args;
            _connection = new RiptideConnection(args.Client);
        }
    }
}
