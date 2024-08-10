using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common.Packets
{
    public class ServerLobbyInvite : Packet
    {
        public override Packets PacketId => Packets.ServerLobbyInvite;
        public ushort InviteeId = 0;
        public ushort InviterId = 0;
        public uint LobbyId = 0;

        public ServerLobbyInvite()
        {

        }

        public ServerLobbyInvite(ushort inviteeId, ushort inviterId, uint lobbyId)
        {
            InviteeId = inviteeId;
            InviterId = inviterId;
            LobbyId = lobbyId;
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(InviteeId);
            writer.Write(InviterId);
            writer.Write(LobbyId);
        }

        public override void Read(BinaryReader reader)
        {
            InviteeId = reader.ReadUInt16();
            InviterId = reader.ReadUInt16();
            LobbyId = reader.ReadUInt32();
        }
    }
}
