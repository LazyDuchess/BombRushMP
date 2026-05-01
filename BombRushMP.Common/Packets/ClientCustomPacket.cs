using BombRushMP.Common.Networking;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common.Packets
{
    public class ClientCustomPacket : Packet
    {
        public enum SendTargets
        {
            Broadcast,
            Lobby,
            Player
        }
        public override Packets PacketId => Packets.ClientCustomPacket;
        public IMessage.SendModes SendMode = IMessage.SendModes.ReliableUnordered;
        public SendTargets TargetMode = SendTargets.Broadcast;
        public ushort Sender = 0;
        public ushort Target = 0;
        public byte[] Data;

        public override void Read(BinaryReader reader)
        {
            Sender = reader.ReadUInt16();
            Target = reader.ReadUInt16();
            SendMode = (IMessage.SendModes)reader.ReadInt32();
            TargetMode = (SendTargets)reader.ReadInt32();
            var len = reader.ReadInt32();
            Data = reader.ReadBytes(len);
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(Sender);
            writer.Write(Target);
            writer.Write((int)SendMode);
            writer.Write((int)TargetMode);
            writer.Write(Data.Length);
            writer.Write(Data);
        }
    }
}
