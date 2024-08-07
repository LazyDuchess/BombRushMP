using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common.Packets
{
    public class ClientLobbyJoin : Packet
    {
        public override Packets PacketId => Packets.ClientLobbyJoin;
        public uint LobbyId = 0;

        public ClientLobbyJoin()
        {

        }

        public ClientLobbyJoin(uint lobbyId)
        {
            LobbyId = lobbyId;
        }

        public override void Read(BinaryReader reader)
        {
            LobbyId = reader.ReadUInt32();
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(LobbyId);
        }
    }
}
