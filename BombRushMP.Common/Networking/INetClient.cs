using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common.Networking
{
    public interface INetClient
    {
        public bool IsConnected { get; }
        public EventHandler Connected { get; set; }
        public EventHandler<MessageReceivedEventArgs> MessageReceived { get; set; }
        public EventHandler<DisconnectedEventArgs> Disconnected { get; set; }
        public EventHandler<ConnectionFailedEventArgs> ConnectionFailed { get; set; }
        public bool Connect(string address, int port);
        public void Disconnect();
        public void Update();
        public void Send(IMessage message);
    }
}
