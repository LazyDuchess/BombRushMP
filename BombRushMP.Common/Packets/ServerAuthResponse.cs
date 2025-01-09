using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common.Packets
{
    public class ServerAuthResponse : Packet
    {
        public override Packets PacketId => Packets.ServerAuthResponse;
        public UserKinds UserKind = UserKinds.Player;

        public override void Read(BinaryReader reader)
        {
            UserKind = (UserKinds)reader.ReadInt32();
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write((int)UserKind);
        }
    }
}
