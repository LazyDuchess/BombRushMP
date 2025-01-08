using BombRushMP.Common.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.RiptideInterface
{
    public class RiptideServer : INetServer
    {
        private Riptide.Server _riptideServer;
        public int TimeoutTime
        {
            set
            {
                _riptideServer.TimeoutTime = value;
            }
        }
        public EventHandler<MessageReceivedEventArgs> MessageReceived { get; set; }
        public EventHandler<ServerDisconnectedEventArgs> ClientDisconnected { get; set; }
        public EventHandler<ServerConnectedEventArgs> ClientConnected { get; set; }

        public RiptideServer()
        {
            _riptideServer = new Riptide.Server();
            _riptideServer.MessageReceived += InternalMessageReceived;
            _riptideServer.ClientDisconnected += InternalClientDisconnected;
        }

        public void Start(ushort port, ushort maxPlayers)
        {
            _riptideServer.Start(port, maxPlayers);
        }

        private void InternalClientDisconnected(object sender, Riptide.ServerDisconnectedEventArgs args)
        {
            var genArgs = new ServerDisconnectedEventArgs(new RiptideConnection(args.Client), (DisconnectReason)args.Reason);
            ClientDisconnected?.Invoke(sender, genArgs);
        }

        private void InternalClientConnected(object sender, Riptide.ServerConnectedEventArgs args)
        {
            var genArgs = new ServerConnectedEventArgs(new RiptideConnection(args.Client));
            ClientConnected?.Invoke(sender, genArgs);
        }

        private void InternalMessageReceived(object sender, Riptide.MessageReceivedEventArgs args)
        {
            var genArgs = new MessageReceivedEventArgs(args.MessageId, new RiptideMessage(args.Message), new RiptideConnection(args.FromConnection));
            MessageReceived?.Invoke(sender, genArgs);
        }

        public void DisconnectClient(ushort id)
        {
            _riptideServer.DisconnectClient(id);
        }

        public void DisconnectClient(INetConnection client)
        {
            _riptideServer.DisconnectClient(client.Id);
        }

        public void Update()
        {
            _riptideServer.Update();
        }

        public void Stop()
        {
            _riptideServer.Stop();
        }

        public override string ToString()
        {
            return _riptideServer.ToString();
        }
    }
}
