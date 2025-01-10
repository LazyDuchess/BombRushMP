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
            AuthKey = reader.ReadString();
            State = new ClientState();
            State.Read(reader);
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(AuthKey);
            State.Write(writer);
        }
    }
}
