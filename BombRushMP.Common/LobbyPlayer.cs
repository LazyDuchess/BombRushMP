using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common
{
    public class LobbyPlayer
    {
        public uint LobbyId = 0;
        public ushort Id = 0;
        public float Score = 0;

        public LobbyPlayer()
        {

        }

        public LobbyPlayer(uint lobbyId, ushort playerId)
        {
            LobbyId = lobbyId;
            Id = playerId;
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(Id);
            writer.Write(Score);
        }

        public void Read(BinaryReader reader)
        {
            Id = reader.ReadUInt16();
            Score = reader.ReadSingle();
        }
    }
}
