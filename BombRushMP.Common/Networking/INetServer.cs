using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common.Networking
{
    public interface INetServer
    {
        public EventHandler<IMessageReceivedEventArgs> MessageReceived { get; set; }
        public EventHandler<IServerDisconnectedEventArgs> ClientDisconnected { get; set; }
        public EventHandler<IServerConnectedEventArgs> ClientConnected { get; set; }
        public int TimeoutTime { set; }
        public void Start(ushort port, ushort maxPlayers);
        public void DisconnectClient(ushort id);
        public void DisconnectClient(INetConnection client);
        public void Update();
        public void Stop();
    }
}
