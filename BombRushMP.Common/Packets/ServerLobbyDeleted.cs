using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common.Packets
{
    public class ServerLobbyDeleted : Packet
    {
        public override Packets PacketId => Packets.ServerLobbyDeleted;
        public uint LobbyUID = 0;

        public ServerLobbyDeleted()
        {

        }

        public ServerLobbyDeleted(uint lobbyUid)
        {
            LobbyUID = lobbyUid;
        }

        public override void Read(BinaryReader reader)
        {
            LobbyUID = reader.ReadUInt32();
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(LobbyUID);
        }
    }
}
