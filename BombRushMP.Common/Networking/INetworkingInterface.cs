using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common.Networking
{
    public interface INetworkingInterface
    {
        public INetClient CreateClient();
        public IMessage CreateMessage(IMessage.SendModes sendMode, Enum packetId);
    }
}
