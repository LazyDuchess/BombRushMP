using BombRushMP.Common.Networking;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common.Packets
{
    public class PlayerAnimation : PlayerPacket
    {
        public static IMessage.SendModes SendMode = IMessage.SendModes.Unreliable;
        private enum BooleanMask
        {
            ForceOverwrite,
            Instant,
            MAX
        }
        public override Packets PacketId => Packets.PlayerAnimation;
        public int NewAnim;
        public bool ForceOverwrite;
        public bool Instant;
        public float AtTime;
        public PlayerAnimation()
        {

        }

        public PlayerAnimation(int newAnim, bool forceOverwrite = false, bool instant = false, float atTime = -1f)
        {
            NewAnim = newAnim;
            ForceOverwrite = forceOverwrite;
            Instant = instant;
            AtTime = atTime;
        }

        public override void Write(BinaryWriter writer)
        {
            base.Write(writer);
            writer.Write(ClientId);
            writer.Write(NewAnim);
            var bits = new Bitfield(BooleanMask.MAX);
            bits[BooleanMask.Instant] = Instant;
            bits[BooleanMask.ForceOverwrite] = ForceOverwrite;
            bits.WriteByte(writer);
            writer.Write(Compression.CompressNormal(AtTime));
        }

        public override void Read(BinaryReader reader)
        {
            base.Read(reader);
            ClientId = reader.ReadUInt16();
            NewAnim = reader.ReadInt32();
            var bits = Bitfield.ReadByte(reader);
            Instant = bits[BooleanMask.Instant];
            ForceOverwrite = bits[BooleanMask.ForceOverwrite];
            AtTime = Compression.DecompressNormal(reader.ReadSByte());
        }
    }
}
