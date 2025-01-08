using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common.Networking
{
    public class RiptideInterface : INetworkingInterface
    {
        public INetClient CreateClient()
        {
            return new RiptideClient();
        }

        public IMessage CreateMessage(IMessage.SendModes sendMode, Enum packetId)
        {
            return new RiptideMessage(Riptide.Message.Create(RiptideUtils.SendModeToRiptide(sendMode), packetId));
        }
    }
}
