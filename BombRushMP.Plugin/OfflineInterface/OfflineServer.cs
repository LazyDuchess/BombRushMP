using BombRushMP.Common.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Plugin.OfflineInterface
{
    public class OfflineServer : INetServer
    {
        public EventHandler<MessageReceivedEventArgs> MessageReceived { get; set; }
        public EventHandler<ServerDisconnectedEventArgs> ClientDisconnected { get; set; }
        public EventHandler<ServerConnectedEventArgs> ClientConnected { get; set; }
        public int TimeoutTime
        {
            set
            {

            }
        }
        public bool LocalConnected = false;

        public void DisconnectClient(ushort id)
        {
            DisconnectLocal();
        }

        public void DisconnectClient(INetConnection client)
        {
            DisconnectLocal();
        }

        public void Start(ushort port, ushort maxPlayers)
        {
            
        }

        public void Stop()
        {
            DisconnectLocal();
        }

        public void ConnectLocal()
        {
            if (LocalConnected) return;
            lock (ClientConnected)
            {
                LocalConnected = true;
                ClientConnected?.Invoke(this, new ServerConnectedEventArgs(new OfflineConnection(false)));
            }
        }

        public void DisconnectLocal()
        {
            if (!LocalConnected) return;
            lock (ClientConnected)
            {
                LocalConnected = false;
                ClientDisconnected?.Invoke(this, new ServerDisconnectedEventArgs(new OfflineConnection(false), "Disconnect"));
            }
        }

        public void Update()
        {
            lock (OfflineInterface.ClientToServerMessages)
            {
                foreach (var packet in OfflineInterface.ClientToServerMessages)
                {
                    MessageReceived?.Invoke(this, new MessageReceivedEventArgs(packet.PacketId, packet, new OfflineConnection(false)));
                }
                OfflineInterface.ClientToServerMessages.Clear();
            }
        }

        public override string ToString()
        {
            return "Server";
        }
    }
}
