using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common.Packets
{
    public class ServerLobbies : Packet
    {
        public override Packets PacketId => Packets.ServerLobbies;
        public List<Lobby> Lobbies = new();

        public ServerLobbies()
        {

        }

        public ServerLobbies(List<Lobby> lobbies)
        {
            Lobbies = lobbies;
        }

        public override void Read(BinaryReader reader)
        {
            var lobbyCount = reader.ReadInt32();
            for(var i = 0; i < lobbyCount; i++)
            {
                var lobby = new Lobby();
                lobby.Read(reader);
                Lobbies.Add(lobby);
            }
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(Lobbies.Count);
            foreach(var lobby in Lobbies)
            {
                lobby.Write(writer);
            }
        }
    }
}
