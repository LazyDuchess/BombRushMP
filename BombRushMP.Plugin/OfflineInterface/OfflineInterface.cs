using BombRushMP.Common.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Plugin.OfflineInterface
{
    public class OfflineInterface : INetworkingInterface
    {
        public static List<OfflineMessage> ClientToServerMessages = new();
        public static List<OfflineMessage> ServerToClientMessages = new();
        public const ushort ClientId = 1;
        public const ushort ServerId = 0;
        public int MaxPayloadSize { get; set; }

        public INetClient CreateClient()
        {
            return new OfflineClient();
        }

        public IMessage CreateMessage(IMessage.SendModes sendMode, Enum packetId)
        {
            return new OfflineMessage(Convert.ToUInt16(packetId));
        }

        public INetServer CreateServer()
        {
            return new OfflineServer();
        }

        public override string ToString()
        {
            return "Offline";
        }
    }
}
