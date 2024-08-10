using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common.Packets
{
    public class ClientLobbyInvite : Packet
    {
        public override Packets PacketId => Packets.ClientLobbyInvite;
        public ushort InviteeId = 0;

        public ClientLobbyInvite()
        {

        }

        public ClientLobbyInvite(ushort inviteeId)
        {
            InviteeId = inviteeId;
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(InviteeId);
        }

        public override void Read(BinaryReader reader)
        {
            InviteeId = reader.ReadUInt16();
        }
    }
}
