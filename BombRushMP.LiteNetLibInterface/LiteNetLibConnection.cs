using BombRushMP.Common.Networking;
using LiteNetLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.LiteNetLibInterface
{
    public class LiteNetLibConnection : INetConnection
    {
        public bool CanQualityDisconnect {
            get
            {
                return false;
            }
            set
            {

            }
        }

        public ushort Id => LiteNetLibUtils.PeerIdToGameId(Peer.Id);

        public string Address => Peer.ToString().Split(':')[0];

        public NetPeer Peer;

        public LiteNetLibConnection(NetPeer peer)
        {
            Peer = peer;
        }

        public void Send(IMessage message)
        {
            var liteNetLibMessage = message as LiteNetLibMessage;
            var messageData = liteNetLibMessage.GetBytesForSend();
            Peer.Send(messageData, (byte)liteNetLibMessage.Channel,liteNetLibMessage.DeliveryMethod);
        }

        public override string ToString()
        {
            return Peer.ToString();
        }
    }
}
