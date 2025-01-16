using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common.Packets
{
    public class ClientLobbySetPlayerTeam : Packet
    {
        public override Packets PacketId => Packets.ClientLobbySetPlayerTeam;
        public ushort PlayerId;
        public byte TeamId;
        public ClientLobbySetPlayerTeam()
        {

        }

        public ClientLobbySetPlayerTeam(ushort playerId, byte teamId)
        {
            PlayerId = playerId;
            TeamId = teamId;
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(PlayerId);
            writer.Write(TeamId);
        }

        public override void Read(BinaryReader reader)
        {
            PlayerId = reader.ReadUInt16();
            TeamId = reader.ReadByte();
        }
    }
}
