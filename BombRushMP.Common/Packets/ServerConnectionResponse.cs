using BombRushMP.Common.Networking;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common.Packets
{
    /// <summary>
    /// From server to client, tells client what their client ID is on connect.
    /// </summary>
    public class ServerConnectionResponse : Packet
    {
        public override Packets PacketId => Packets.ServerConnectionResponse;
        public ushort LocalClientId = 0;
        public float TickRate = Constants.DefaultNetworkingTickRate;
        public IMessage.SendModes ClientAnimationSendMode = IMessage.SendModes.ReliableUnordered;
        public AuthUser User;
        public ServerState ServerState;

        public override void Read(BinaryReader reader)
        {
            LocalClientId = reader.ReadUInt16();
            TickRate = reader.ReadSingle();
            ClientAnimationSendMode = (IMessage.SendModes)reader.ReadByte();
            User = new AuthUser();
            User.Read(reader);
            ServerState = new ServerState();
            ServerState.Read(reader);
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(LocalClientId);
            writer.Write(TickRate);
            writer.Write((byte)ClientAnimationSendMode);
            User.Write(writer);
            ServerState.Write(writer);
        }
    }
}
