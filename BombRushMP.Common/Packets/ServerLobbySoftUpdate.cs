using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common.Packets
{
    public class ServerLobbySoftUpdate : Packet
    {
        public override Packets PacketId => Packets.ServerLobbySoftUpdate;
        public uint LobbyId;
        public ushort PlayerId;
        public float Score;

        public ServerLobbySoftUpdate()
        {

        }

        public ServerLobbySoftUpdate(uint lobbyId, ushort playerId, float score)
        {
            LobbyId = lobbyId;
            PlayerId = playerId;
            Score = score;
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(LobbyId);
            writer.Write(PlayerId);
            writer.Write(Score);
        }

        public override void Read(BinaryReader reader)
        {
            LobbyId = reader.ReadUInt32();
            PlayerId = reader.ReadUInt16();
            Score = reader.ReadSingle();
        }
    }
}
