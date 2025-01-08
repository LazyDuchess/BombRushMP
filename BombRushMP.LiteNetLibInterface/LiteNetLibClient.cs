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
        public bool IsConnected => _netManager.FirstPeer != null && _netManager.FirstPeer.ConnectionState == ConnectionState.Connected;

        public EventHandler Connected { get; set; }
        public EventHandler<MessageReceivedEventArgs> MessageReceived { get; set; }
        public EventHandler<DisconnectedEventArgs> Disconnected { get; set; }
        public EventHandler<ConnectionFailedEventArgs> ConnectionFailed { get; set; }
        public EventHandler<ushort> ClientDisconnected { get; set; }

        private EventBasedNetListener _netListener;
        private NetManager _netManager;

        public LiteNetLibClient()
        {
            _netListener = new EventBasedNetListener();
            _netListener.PeerConnectedEvent += _netListener_PeerConnectedEvent;
            _netListener.NetworkReceiveEvent += _netListener_NetworkReceiveEvent;
            _netListener.PeerDisconnectedEvent += _netListener_PeerDisconnectedEvent;
        }

        private void _netListener_PeerDisconnectedEvent(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            if (disconnectInfo.Reason == DisconnectReason.ConnectionFailed)
            {
                ConnectionFailed?.Invoke(this, new ConnectionFailedEventArgs(DisconnectReason.ConnectionFailed.ToString()));
                return;
            }
            Disconnected?.Invoke(this, new DisconnectedEventArgs(disconnectInfo.Reason.ToString()));
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

        public bool Connect(string address, int port)
        {
            if (_netManager != null)
            {
                _netManager.Stop();
            }
            _netManager = new NetManager(_netListener);
            if (NetworkingEnvironment.UseNativeSocketsIfAvailable)
            {
                NetworkingEnvironment.Log("LiteNetLib: Using native sockets if supported.");
                _netManager.UseNativeSockets = true;
            }
            if (_netManager.Start())
            {
                _netManager.Connect(address, port, "BRCMP");
                return true;
            }
            return false;
        }

        public void Disconnect()
        {
            _netManager.DisconnectAll();
        }

        public void Send(IMessage message)
        {
            _netManager.SendToAll((message as LiteNetLibMessage).GetBytesForSend(), (message as LiteNetLibMessage).DeliveryMethod);
        }

        public void Update()
        {
            _netManager.TriggerUpdate();
            _netManager.PollEvents();
        }

        public override string ToString()
        {
            return _netManager.ToString();
        }
    }
}
