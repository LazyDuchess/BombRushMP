using BombRushMP.Common.Networking;
using Riptide;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.RiptideInterface
{
    public class RiptideInterface : INetworkingInterface
    {
        public int MaxPayloadSize
        {
            get
            {
                return Message.MaxPayloadSize;
            }
            set
            {
                Message.MaxPayloadSize = value;
            }
        }
        public INetClient CreateClient()
        {
            return new RiptideClient();
        }

        public INetServer CreateServer()
        {
            return new RiptideServer();
        }

        public IMessage CreateMessage(IMessage.SendModes sendMode, NetChannels channel, Enum packetId)
        {
            return new RiptideMessage(Message.Create(RiptideUtils.SendModeToRiptide(sendMode), packetId));
        }

        public override string ToString()
        {
            return "Riptide Networking";
        }
    }
}