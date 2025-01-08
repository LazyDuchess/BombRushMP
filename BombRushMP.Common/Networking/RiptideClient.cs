using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common.Networking
{
    internal class RiptideClient : INetClient
    {
        private Riptide.Client _riptideClient;
        public EventHandler Connected { get; set; }
        public EventHandler<IMessageReceivedEventArgs> MessageReceived { get; set; }

        public RiptideClient()
        {
            _riptideClient = new Riptide.Client();
            _riptideClient.Connected += InternalConnected;
            _riptideClient.MessageReceived += InternalMessageReceived;
        }

        private void InternalConnected(object sender, EventArgs args)
        {
            Connected?.Invoke(sender, args);
        }

        private void InternalMessageReceived(object sender, Riptide.MessageReceivedEventArgs args)
        {
            var genArgs = new RiptideMessageReceivedEventArgs(args);
            MessageReceived?.Invoke(sender, genArgs);
        }

        public bool Connect(string address)
        {
            return _riptideClient.Connect(address);
        }

        public void Disconnect()
        {
            _riptideClient.Disconnect();
        }

        public void Send(IMessage message)
        {
            _riptideClient.Send((message as RiptideMessage).InternalRiptideMessage);
        }

        public void Update()
        {
            _riptideClient.Update();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
