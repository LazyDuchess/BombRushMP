using System;
using System.Collections.Generic;
using System.Linq;
using BombRushMP.Common.Networking;
using System.Text;
using System.Threading.Tasks;
using LiteNetLib.Utils;
using System.IO;
using LiteNetLib;

namespace BombRushMP.LiteNetLibInterface
{
    public class LiteNetLibMessage : IMessage
    {
        public DeliveryMethod DeliveryMethod;
        public NetChannels Channel;
        private ushort _packetId;
        private NetDataWriter _writer;

        public LiteNetLibMessage(IMessage.SendModes sendMode, NetChannels channel, ushort packetId)
        {
            DeliveryMethod = LiteNetLibUtils.SendModeToDeliveryMethod(sendMode);
            Channel = channel;
            _writer = new NetDataWriter();
            _packetId = packetId;
        }

        public IMessage Add(byte[] data)
        {
            _writer.Put(data);
            return this;
        }

        public byte[] GetBytes()
        {
            return _writer.CopyData();
        }

        public byte[] GetBytesForSend()
        {
            using (var ms = new MemoryStream())
            {
                using (var writer = new BinaryWriter(ms))
                {
                    // Indicate this is an user packet
                    writer.Write((byte)1);
                    writer.Write(_packetId);
                    writer.Write(GetBytes());
                    return ms.ToArray();
                }
            }
        }
    }
}
