using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common.Packets
{
    public class ServerTeamGraffRaceScore : Packet
    {
        public override Packets PacketId => Packets.ServerTeamGraffRaceScore;
        public int TagHash;
        public ushort PlayerId;

        public ServerTeamGraffRaceScore()
        {

        }

        public ServerTeamGraffRaceScore(ushort playerId, int tagHash)
        {
            PlayerId = playerId;
            TagHash = tagHash;
        }

        public override void Read(BinaryReader reader)
        {
            PlayerId = reader.ReadUInt16();
            TagHash = reader.ReadInt32();
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(PlayerId);
            writer.Write(TagHash);
        }
    }
}
