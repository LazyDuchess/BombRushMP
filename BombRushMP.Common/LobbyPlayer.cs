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
        public bool Ready = false;
        public byte Team = 0;
        public int Wins = 0;

        public LobbyPlayer()
        {

        }

        public LobbyPlayer(uint lobbyId, ushort playerId, byte team)
        {
            LobbyId = lobbyId;
            Id = playerId;
            Team = team;
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(Id);
            writer.Write(Score);
            writer.Write(Ready);
            writer.Write(Team);
            writer.Write(Wins);
        }

        public void Read(BinaryReader reader)
        {
            Id = reader.ReadUInt16();
            Score = reader.ReadSingle();
            Ready = reader.ReadBoolean();
            Team = reader.ReadByte();
            Wins = reader.ReadInt32();
        }
    }
}
