using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common.Packets
{
    public class ServerClientStates : Packet
    {
        public override Packets PacketId => Packets.ServerClientStates;
        public Dictionary<ushort, ClientState> ClientStates = new();
        public override void Read(BinaryReader reader)
        {
            var count = reader.ReadInt32();
            for(var i = 0; i < count; i++)
            {
                var clientId = reader.ReadUInt16();
                var clientState = new ClientState();
                clientState.Read(reader);
                ClientStates[clientId] = clientState;
            }
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(ClientStates.Count);
            foreach(var clientState in ClientStates)
            {
                writer.Write(clientState.Key);
                clientState.Value.Write(writer);
            }
        }
    }
}
