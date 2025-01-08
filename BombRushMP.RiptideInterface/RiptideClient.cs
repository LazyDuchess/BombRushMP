using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BombRushMP.Common.Networking;

namespace BombRushMP.RiptideInterface
{
    internal class RiptideClient : INetClient
    {
        private Riptide.Client _riptideClient;
        public bool IsConnected => _riptideClient.IsConnected;
        public EventHandler Connected { get; set; }
        public EventHandler<IMessageReceivedEventArgs> MessageReceived { get; set; }
        public EventHandler<IDisconnectedEventArgs> Disconnected { get; set; }
        public EventHandler<IConnectionFailedEventArgs> ConnectionFailed { get; set; }
        public EventHandler<ushort> ClientDisconnected { get; set; }

        public RiptideClient()
        {
            _riptideClient = new Riptide.Client();
            _riptideClient.Connected += InternalConnected;
            _riptideClient.MessageReceived += InternalMessageReceived;
            _riptideClient.Disconnected += InternalDisconnected;
            _riptideClient.ConnectionFailed += InternalConnectionFailed;
            _riptideClient.ClientDisconnected += InternalClientDisconnected;
        }

        private void InternalClientDisconnected(object sender, Riptide.ClientDisconnectedEventArgs args)
        {
            ClientDisconnected?.Invoke(sender, args.Id);
        }

        private void InternalConnected(object sender, EventArgs args)
        {
            Connected?.Invoke(sender, args);
        }
        private void InternalConnectionFailed(object sender, Riptide.ConnectionFailedEventArgs args)
        {
            var genArgs = new RiptideConnectionFailedEventArgs(args);
            ConnectionFailed?.Invoke(sender, genArgs);
        }

        private void InternalDisconnected(object sender, Riptide.DisconnectedEventArgs args)
        {
            var genArgs = new RiptideDisconnectedEventArgs(args);
            Disconnected?.Invoke(sender, genArgs);
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
