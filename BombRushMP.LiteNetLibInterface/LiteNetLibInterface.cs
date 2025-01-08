using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BombRushMP.Common.Networking;
using LiteNetLib;

namespace BombRushMP.LiteNetLibInterface
{
    public class LiteNetLibInterface : INetworkingInterface
    {
        public int MaxPayloadSize
        {
            get
            {
                return _netManager.MtuOverride;
            }
            set
            {

            }
        }
        private EventBasedNetListener _netListener;
        private NetManager _netManager;

        public LiteNetLibInterface()
        {
            _netListener = new EventBasedNetListener();
            _netManager = new NetManager(_netListener);
        }

        public INetClient CreateClient()
        {
            return new LiteNetLibClient(_netListener, _netManager);
        }

        public IMessage CreateMessage(IMessage.SendModes sendMode, Enum packetId)
        {
            return new LiteNetLibMessage(sendMode, Convert.ToUInt16(packetId));
        }

        public INetServer CreateServer()
        {
            return new LiteNetLibServer(_netListener, _netManager);
        }
    }
}
