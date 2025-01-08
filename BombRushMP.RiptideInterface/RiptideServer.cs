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
        public EventHandler<IMessageReceivedEventArgs> MessageReceived { get; set; }
        public EventHandler<IServerDisconnectedEventArgs> ClientDisconnected { get; set; }
        public EventHandler<IServerConnectedEventArgs> ClientConnected { get; set; }

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
            var genArgs = new RiptideServerDisconnectedEventArgs(args);
            ClientDisconnected?.Invoke(sender, genArgs);
        }

        private void InternalClientConnected(object sender, Riptide.ServerConnectedEventArgs args)
        {
            var genArgs = new RiptideServerConnectedEventArgs(args);
            ClientConnected?.Invoke(sender, genArgs);
        }

        private void InternalMessageReceived(object sender, Riptide.MessageReceivedEventArgs args)
        {
            var genArgs = new RiptideMessageReceivedEventArgs(args);
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
    }
}
