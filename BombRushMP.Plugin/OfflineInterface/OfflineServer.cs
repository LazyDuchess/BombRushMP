using BombRushMP.Common.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
        public volatile bool LocalConnected = false;

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
            LocalConnected = true;
            ClientConnected?.Invoke(this, new ServerConnectedEventArgs(new OfflineConnection(false)));
        }

        public void DisconnectLocal()
        {
            if (!LocalConnected) return;
            LocalConnected = false;
            ClientDisconnected?.Invoke(this, new ServerDisconnectedEventArgs(new OfflineConnection(false), "Disconnect"));
        }

        public void Update()
        {
            List<OfflineMessage> _snapshot;
            lock (OfflineInterface.ClientToServerMessages)
            {
                _snapshot = new List<OfflineMessage>(OfflineInterface.ClientToServerMessages);
                OfflineInterface.ClientToServerMessages.Clear();
            }
            foreach (var packet in _snapshot)
            {
                try
                {
                    MessageReceived?.Invoke(this, new MessageReceivedEventArgs(packet.PacketId, packet, new OfflineConnection(false)));
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogError(e);
                }
            }
        }

        public override string ToString()
        {
            return "Server";
        }
    }
}
