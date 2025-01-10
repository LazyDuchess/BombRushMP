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
        public Guid CrewBoomCharacter = Guid.Empty;
        public int Character = 0;
        public int Outfit = 0;
        public SpecialSkins SpecialSkin = SpecialSkins.None;
        public int SpecialSkinVariant = -1;
        public string Name = "Player";
        public int Stage = 0;
        public uint ProtocolVersion = Constants.ProtocolVersion;
        public AuthUser User = new AuthUser();

        public override void Read(BinaryReader reader)
        {
            if (Guid.TryParse(reader.ReadString(), out var result))
                CrewBoomCharacter = result;
            Character = reader.ReadInt32();
            Outfit = reader.ReadInt32();
            ProtocolVersion = reader.ReadUInt32();
            Stage = reader.ReadInt32();
            Name = reader.ReadString();
            SpecialSkin = (SpecialSkins)reader.ReadInt32();
            SpecialSkinVariant = reader.ReadInt32();
            var user = new AuthUser();
            User.Read(reader);
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(CrewBoomCharacter.ToString());
            writer.Write(Character);
            writer.Write(Outfit);
            writer.Write(ProtocolVersion);
            writer.Write(Stage);
            writer.Write(Name);
            writer.Write((int)SpecialSkin);
            writer.Write(SpecialSkinVariant);
            User.Write(writer);
        }
    }
}
