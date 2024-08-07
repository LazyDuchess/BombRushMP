using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common.Packets
{
    public class ServerClientVisualStates : Packet
    {
        public override Packets PacketId => Packets.ServerClientVisualStates;
        public Dictionary<ushort, ClientVisualState> ClientVisualStates = new();
        public override void Read(BinaryReader reader)
        {
            var count = reader.ReadInt32();
            for (var i = 0; i < count; i++)
            {
                var clientId = reader.ReadUInt16();
                var clientVisualState = new ClientVisualState();
                clientVisualState.Read(reader);
                ClientVisualStates[clientId] = clientVisualState;
            }
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(ClientVisualStates.Count);
            foreach (var clientVisualState in ClientVisualStates)
            {
                writer.Write(clientVisualState.Key);
                clientVisualState.Value.Write(writer);
            }
        }
    }
}
