using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common.Packets
{
    public class ClientLobbyDeclineInvite : Packet
    {
        public override Packets PacketId => Packets.ClientLobbyDeclineInvite;
        public uint LobbyId;
        public ClientLobbyDeclineInvite()
        {

        }

        public ClientLobbyDeclineInvite(uint lobbyId)
        {
            LobbyId = lobbyId;
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(LobbyId);
        }

        public override void Read(BinaryReader reader)
        {
            LobbyId = reader.ReadUInt32();
        }
    }
}
