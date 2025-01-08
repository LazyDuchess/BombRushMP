using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common.Networking
{
    public interface INetClient : IDisposable
    {
        public EventHandler Connected { get; set; }
        public EventHandler<IMessageReceivedEventArgs> MessageReceived { get; set; }
        public bool Connect(string address);
        public void Disconnect();
        public void Update();
        public void Send(IMessage message);
    }
}
