using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common
{
    public class Lobby
    {
        public int Stage = 0;
        public uint Id = 0;
        public ushort HostId = 0;
        public List<ushort> Players = new();

        public Lobby()
        {

        }

        public Lobby(int stage, uint id, ushort hostId)
        {
            Stage = stage;
            Id = id;
            HostId = hostId;
        }

        public void Read(BinaryReader reader)
        {
            Stage = reader.ReadInt32();
            Id = reader.ReadUInt32();
            HostId = reader.ReadUInt16();
            Players.Clear();
            var playerCount = reader.ReadInt32();
            for (var i = 0; i < playerCount; i++)
            {
                var playerId = reader.ReadUInt16();
                Players.Add(playerId);
            }
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(Stage);
            writer.Write(Id);
            writer.Write(HostId);
            writer.Write(Players.Count);
            foreach(var playerId in Players)
            {
                writer.Write(playerId);
            }
        }
    }
}
