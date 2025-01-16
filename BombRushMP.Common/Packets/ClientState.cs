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
        public sbyte Character = 0;
        public sbyte FallbackCharacter = 0;
        public byte FallbackOutfit = 0;
        public byte Outfit = 0;
        public SpecialSkins SpecialSkin = SpecialSkins.None;
        public int SpecialSkinVariant = -1;
        public string Name = "Player";
        public string CrewName = "";
        public int Stage = 0;
        public AuthUser User = new AuthUser();

        public override void Read(BinaryReader reader)
        {
            if (Guid.TryParse(reader.ReadString(), out var result))
                CrewBoomCharacter = result;
            Character = reader.ReadSByte();
            FallbackCharacter = reader.ReadSByte();
            FallbackOutfit = reader.ReadByte();
            Outfit = reader.ReadByte();
            Stage = reader.ReadInt32();
            Name = reader.ReadString();
            CrewName = reader.ReadString();
            SpecialSkin = (SpecialSkins)reader.ReadInt32();
            SpecialSkinVariant = reader.ReadInt32();
            var user = new AuthUser();
            User.Read(reader);
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(CrewBoomCharacter.ToString());
            writer.Write(Character);
            writer.Write(FallbackCharacter);
            writer.Write(FallbackOutfit);
            writer.Write(Outfit);
            writer.Write(Stage);
            writer.Write(Name);
            writer.Write(CrewName);
            writer.Write((int)SpecialSkin);
            writer.Write(SpecialSkinVariant);
            User.Write(writer);
        }
    }

    public class IncompatibleProtocolException : Exception
    {
    }
}
