using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common.Packets
{
    public class ClientAuth : Packet
    {
        public override Packets PacketId => Packets.ClientAuth;
        public string AuthKey = "";
        public bool Invisible = false;
        public ClientState State;

        public ClientAuth()
        {

        }

        public ClientAuth(string authKey, ClientState state)
        {
            AuthKey = authKey;
            State = state;
        }

        public override void Read(BinaryReader reader)
        {
            var protocol = reader.ReadUInt32();
            if (protocol != Constants.ProtocolVersion)
                throw new IncompatibleProtocolException();
            AuthKey = reader.ReadString();
            Invisible = reader.ReadBoolean();
            State = new ClientState();
            State.Read(reader);
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(Constants.ProtocolVersion);
            writer.Write(AuthKey);
            writer.Write(Invisible);
            State.Write(writer);
        }
    }
}
