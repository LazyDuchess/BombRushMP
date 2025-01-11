using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common.Networking
{
    public interface INetworkingInterface
    {
        public int MaxPayloadSize { get; set; }
        public INetClient CreateClient();
        public INetServer CreateServer();
        public IMessage CreateMessage(IMessage.SendModes sendMode, NetChannels channel, Enum packetId);
    }
}
