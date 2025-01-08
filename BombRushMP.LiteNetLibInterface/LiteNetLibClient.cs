using BombRushMP.Common.Networking;
using LiteNetLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.LiteNetLibInterface
{
    public class LiteNetLibClient : INetClient
    {
        public bool IsConnected => _netManager.FirstPeer.ConnectionState == ConnectionState.Connected;

        public EventHandler Connected { get; set; }
        public EventHandler<MessageReceivedEventArgs> MessageReceived { get; set; }
        public EventHandler<DisconnectedEventArgs> Disconnected { get; set; }
        public EventHandler<ConnectionFailedEventArgs> ConnectionFailed { get; set; }
        public EventHandler<ushort> ClientDisconnected { get; set; }

        private EventBasedNetListener _netListener;
        private NetManager _netManager;

        public LiteNetLibClient(EventBasedNetListener listener, NetManager manager)
        {
            _netListener = listener;
            _netManager = manager;
            _netListener.PeerConnectedEvent += _netListener_PeerConnectedEvent;
            _netListener.NetworkReceiveEvent += _netListener_NetworkReceiveEvent;
            _netListener.PeerDisconnectedEvent += _netListener_PeerDisconnectedEvent;
        }

        private void _netListener_PeerDisconnectedEvent(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            var disconnectReason = Common.Networking.DisconnectReason.Disconnected;
            if (disconnectInfo.Reason == LiteNetLib.DisconnectReason.ConnectionFailed)
            {
                ConnectionFailed?.Invoke(this, new ConnectionFailedEventArgs(RejectReason.NoConnection));
                return;
            }
            switch (disconnectInfo.Reason)
            {
                case LiteNetLib.DisconnectReason.DisconnectPeerCalled:
                    disconnectReason = Common.Networking.DisconnectReason.Kicked;
                    break;
                case LiteNetLib.DisconnectReason.Timeout:
                    disconnectReason = Common.Networking.DisconnectReason.TimedOut;
                    break;
                case LiteNetLib.DisconnectReason.ConnectionRejected:
                    disconnectReason = Common.Networking.DisconnectReason.ConnectionRejected;
                    break;
                case LiteNetLib.DisconnectReason.RemoteConnectionClose:
                    disconnectReason = Common.Networking.DisconnectReason.ServerStopped;
                    break;
            }
            Disconnected?.Invoke(this, new DisconnectedEventArgs(disconnectReason));
        }

        private void _netListener_NetworkReceiveEvent(NetPeer peer, NetPacketReader reader, byte channel, DeliveryMethod deliveryMethod)
        {
            var isUser = reader.GetByte();
            if (isUser == 0)
            {
                var userId = reader.GetUShort();
                reader.Recycle();
                ClientDisconnected?.Invoke(this, userId);
                return;
            }
            var packetId = reader.GetUShort();
            var data = reader.GetRemainingBytes();
            var connection = new LiteNetLibConnection(peer);
            var reliability = deliveryMethod == DeliveryMethod.Unreliable ? IMessage.SendModes.Unreliable : IMessage.SendModes.Reliable;
            var msg = new LiteNetLibMessage(reliability, packetId);
            msg.Add(data);
            MessageReceived?.Invoke(this, new MessageReceivedEventArgs(packetId, msg, connection));
            reader.Recycle();
        }

        private void _netListener_PeerConnectedEvent(NetPeer peer)
        {
            Connected?.Invoke(this, null);
        }

        public bool Connect(string address)
        {
            var startResult = _netManager.Start();
            if (startResult)
            {
                var split = address.Split(':');
                var addr = split[0];
                var port = int.Parse(split[1]);
                _netManager.Connect(address, port, "BRCMP");
            }
            return startResult;
        }

        public void Disconnect()
        {
            _netManager.DisconnectAll();
            _netManager.Stop();
        }

        public void Send(IMessage message)
        {
            _netManager.SendToAll((message as LiteNetLibMessage).GetBytesForSend(), (message as LiteNetLibMessage).DeliveryMethod);
        }

        public void Update()
        {
            _netManager.PollEvents();
        }
    }
}
