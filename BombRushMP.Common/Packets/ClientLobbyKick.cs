using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common.Packets
{
    public class ClientLobbyKick : Packet
    {
        public override Packets PacketId => Packets.ClientLobbyKick;
        public ushort PlayerId = 0;
        public ClientLobbyKick()
        {

        }

        public ClientLobbyKick(ushort playerId)
        {
            PlayerId = playerId;
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(PlayerId);
        }

        public override void Read(BinaryReader reader)
        {
            PlayerId = reader.ReadUInt16();
        }
    }
}
