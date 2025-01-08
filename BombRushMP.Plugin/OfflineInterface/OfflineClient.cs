using BombRushMP.Common.Networking;
using BombRushMP.Plugin.LocalServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Plugin.OfflineInterface
{
    public class OfflineClient : INetClient
    {
        public OfflineServer Server => ServerController.Instance.Server.Server as OfflineServer;
        public bool IsConnected => Server.LocalConnected;

        public EventHandler Connected { get; set; }
        public EventHandler<MessageReceivedEventArgs> MessageReceived { get; set; }
        public EventHandler<DisconnectedEventArgs> Disconnected { get; set; }
        public EventHandler<ConnectionFailedEventArgs> ConnectionFailed { get; set; }
        public EventHandler<ushort> ClientDisconnected { get; set; }

        public bool Connect(string address, int port)
        {
            Server.ConnectLocal();
            Connected?.Invoke(this, null);
            return true;
        }

        public void Disconnect()
        {
            Server.DisconnectLocal();
        }

        public void Send(IMessage message)
        {
            var serverConnection = new OfflineConnection(true);
            serverConnection.Send(message);
        }

        public void Update()
        {
            lock (OfflineInterface.ServerToClientMessages)
            {
                foreach (var packet in OfflineInterface.ServerToClientMessages)
                {
                    MessageReceived?.Invoke(this, new MessageReceivedEventArgs(packet.PacketId, packet, new OfflineConnection(true)));
                }
                OfflineInterface.ServerToClientMessages.Clear();
            }
        }

        public override string ToString()
        {
            return "Client";
        }
    }
}
