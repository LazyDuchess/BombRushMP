using BombRushMP.Common.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Plugin.OfflineInterface
{
    public class OfflineMessage : IMessage
    {
        public ushort PacketId;
        private byte[] _data;

        public OfflineMessage(ushort packetId)
        {
            PacketId = packetId;
        }

        public OfflineMessage(ushort packetId, byte[] data)
        {
            PacketId = packetId;
            _data = data;
        }

        public IMessage Add(byte[] data)
        {
            _data = data;
            return this;
        }

        public byte[] GetBytes()
        {
            return _data;
        }
    }
}
