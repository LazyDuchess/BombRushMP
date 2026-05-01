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
            Players
        }
        public override Packets PacketId => Packets.ClientCustomPacket;
        public IMessage.SendModes SendMode = IMessage.SendModes.ReliableUnordered;
        public SendTargets TargetMode = SendTargets.Broadcast;
        public ushort Sender = 0;
        public ushort[] Targets;
        public byte[] Data;
        public int CustomPacketId = 0;

        public override void Read(BinaryReader reader)
        {
            CustomPacketId = reader.ReadInt32();
            Sender = reader.ReadUInt16();
            var targetLen = reader.ReadInt32();
            Targets = new ushort[targetLen];
            for(var i = 0; i < targetLen; i++)
            {
                Targets[i] = reader.ReadUInt16();
            }
            SendMode = (IMessage.SendModes)reader.ReadInt32();
            TargetMode = (SendTargets)reader.ReadInt32();
            var len = reader.ReadInt32();
            Data = reader.ReadBytes(len);
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(CustomPacketId);
            writer.Write(Sender);
            writer.Write(Targets.Length);
            foreach(var target in Targets)
            {
                writer.Write(target);
            }
            writer.Write((int)SendMode);
            writer.Write((int)TargetMode);
            writer.Write(Data.Length);
            writer.Write(Data);
        }
    }
}
