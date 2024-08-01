using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common.Packets
{
    /// <summary>
    /// From client to server, on first join, tells username, stage and protocol version.
    /// </summary>
    public class ClientState : Packet
    {
        public override Packets PacketId => Packets.ClientState;
        private const byte Version = 0;
        public int Character;
        public int Outfit;
        public string Name;
        public int Stage;
        public uint ProtocolVersion;

        public override void Read(BinaryReader reader)
        {
            var version = reader.ReadByte();
            Character = reader.ReadInt32();
            Outfit = reader.ReadInt32();
            ProtocolVersion = reader.ReadUInt32();
            Stage = reader.ReadInt32();
            Name = reader.ReadString();
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(Version);
            writer.Write(Character);
            writer.Write(Outfit);
            writer.Write(ProtocolVersion);
            writer.Write(Stage);
            writer.Write(Name);
        }
    }
}
