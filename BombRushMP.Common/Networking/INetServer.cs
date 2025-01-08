using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common.Networking
{
    public interface INetServer
    {
        public EventHandler<MessageReceivedEventArgs> MessageReceived { get; set; }
        public EventHandler<ServerDisconnectedEventArgs> ClientDisconnected { get; set; }
        public EventHandler<ServerConnectedEventArgs> ClientConnected { get; set; }
        public int TimeoutTime { set; }
        public void Start(ushort port, ushort maxPlayers, bool local);
        public void DisconnectClient(ushort id);
        public void DisconnectClient(INetConnection client);
        public void Update();
        public void Stop();
    }
}
