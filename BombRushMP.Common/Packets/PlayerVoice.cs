using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common.Packets
{
    public class PlayerVoice : Packet
    {
        public override Packets PacketId => Packets.PlayerVoice;
        private const byte Version = 0;
        public ushort ClientId = 0;
        public int AudioClipId;
        public int VoicePriority;

        public override void Read(BinaryReader reader)
        {
            var version = reader.ReadByte();
            ClientId = reader.ReadUInt16();
            AudioClipId = reader.ReadInt32();
            VoicePriority = reader.ReadInt32();
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(Version);
            writer.Write(ClientId);
            writer.Write(AudioClipId);
            writer.Write(VoicePriority);
        }
    }
}
