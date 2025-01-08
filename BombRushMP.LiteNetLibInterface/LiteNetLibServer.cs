using BombRushMP.Common.Networking;
using LiteNetLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.LiteNetLibInterface
{
    public class LiteNetLibServer : INetServer
    {
        public EventHandler<MessageReceivedEventArgs> MessageReceived { get; set; }
        public EventHandler<ServerDisconnectedEventArgs> ClientDisconnected { get; set; }
        public EventHandler<ServerConnectedEventArgs> ClientConnected { get; set; }
        public int TimeoutTime {
            set
            {
                _disconnectTimeout = value;
            }
        }
        private int _disconnectTimeout = 10000;
        private EventBasedNetListener _netListener;
        private NetManager _netManager;

        private ushort _maxPlayers = 0;

        public LiteNetLibServer()
        {
            _netListener = new EventBasedNetListener();
            _netListener.ConnectionRequestEvent += _netListener_ConnectionRequestEvent;
            _netListener.PeerConnectedEvent += _netListener_PeerConnectedEvent;
            _netListener.PeerDisconnectedEvent += _netListener_PeerDisconnectedEvent;
            _netListener.NetworkReceiveEvent += _netListener_NetworkReceiveEvent;
        }

        private void _netListener_NetworkReceiveEvent(NetPeer peer, NetPacketReader reader, byte channel, DeliveryMethod deliveryMethod)
        {
            var isUser = reader.GetByte();
            if (isUser == 0) return;
            var packetId = reader.GetUShort();
            var data = reader.GetRemainingBytes();
            var connection = new LiteNetLibConnection(peer);
            var reliability = deliveryMethod == DeliveryMethod.Unreliable ? IMessage.SendModes.Unreliable : IMessage.SendModes.Reliable;
            var msg = new LiteNetLibMessage(reliability, packetId);
            msg.Add(data);
            MessageReceived?.Invoke(this, new MessageReceivedEventArgs(packetId, msg, connection));
            reader.Recycle();
        }

        private void _netListener_PeerDisconnectedEvent(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            ClientDisconnected?.Invoke(this, new ServerDisconnectedEventArgs(new LiteNetLibConnection(peer), disconnectInfo.Reason.ToString()));
            byte[] data;
            using (var ms = new MemoryStream())
            {
                using (var writer = new BinaryWriter(ms))
                {
                    writer.Write((byte)0);
                    writer.Write((ushort)peer.Id);
                    data = ms.ToArray();
                }
            }
            _netManager.SendToAll(data, DeliveryMethod.ReliableOrdered);
        }

        private void _netListener_PeerConnectedEvent(NetPeer peer)
        {
            ClientConnected?.Invoke(this, new ServerConnectedEventArgs(new LiteNetLibConnection(peer)));
        }

        private void _netListener_ConnectionRequestEvent(ConnectionRequest request)
        {
            if (_netManager.ConnectedPeersCount < _maxPlayers)
            {
                request.AcceptIfKey("BRCMP");
            }
            else
                request.Reject();
        }

        public void DisconnectClient(ushort id)
        {
            var peer = _netManager.GetPeerById(LiteNetLibUtils.GameIdToPeerId(id));
            _netManager.DisconnectPeer(peer);
        }

        public void DisconnectClient(INetConnection client)
        {
            var connection = client as LiteNetLibConnection;
            _netManager.DisconnectPeer(connection.Peer);
        }

        public void Start(ushort port, ushort maxPlayers)
        {
            if (_netManager != null)
            {
                _netManager.Stop();
            }
            _netManager = new NetManager(_netListener);
            _netManager.DisconnectTimeout = _disconnectTimeout;
            _netManager.Start(port);
            _maxPlayers = maxPlayers;
        }

        public void Stop()
        {
            _netManager.Stop();
        }

        public void Update()
        {
            _netManager.PollEvents();
        }

        public override string ToString()
        {
            return _netManager.ToString();
        }
    }
}
