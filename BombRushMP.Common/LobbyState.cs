using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common
{
    public class LobbyState
    {
        public bool InGame = false;
        public GamemodeIDs Gamemode = GamemodeIDs.ScoreBattle;
        public byte[] GamemodeSettings = [];
        public int Stage = 0;
        public uint Id = 0;
        public ushort HostId = 0;
        public Dictionary<ushort, LobbyPlayer> Players = new();
        public Dictionary<ushort, DateTime> InvitedPlayers = new();
        public Dictionary<byte, float> TeamScores = new();

        public LobbyState()
        {

        }

        public LobbyState(int stage, uint id, ushort hostId)
        {
            Stage = stage;
            Id = id;
            HostId = hostId;
        }

        public float GetScoreForTeam(byte team)
        {
            if (TeamScores.TryGetValue(team, out var score))
                return score;
            return 0f;
        }

        public void Read(BinaryReader reader)
        {
            InGame = reader.ReadBoolean();
            Gamemode = (GamemodeIDs)reader.ReadInt32();
            var gameModeSettingsLength = reader.ReadInt32();
            GamemodeSettings = reader.ReadBytes(gameModeSettingsLength);
            Stage = reader.ReadInt32();
            Id = reader.ReadUInt32();
            HostId = reader.ReadUInt16();
            Players.Clear();
            var playerCount = reader.ReadInt32();
            for (var i = 0; i < playerCount; i++)
            {
                var player = new LobbyPlayer();
                player.Read(reader);
                player.LobbyId = Id;
                Players[player.Id] = player;
            }
            InvitedPlayers.Clear();
            var inviteCount = reader.ReadInt32();
            for(var i = 0; i < inviteCount; i++)
            {
                var inviteTime = DateTime.FromBinary(reader.ReadInt64());
                var playerId = reader.ReadUInt16();
                InvitedPlayers[playerId] = inviteTime;
            }
            var teamCount = reader.ReadByte();
            for(var i=0;i < teamCount; i++)
            {
                var teamId = reader.ReadByte();
                var teamScore = reader.ReadSingle();
                TeamScores[teamId] = teamScore;
            }
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(InGame);
            writer.Write((int)Gamemode);
            writer.Write(GamemodeSettings.Length);
            writer.Write(GamemodeSettings);
            writer.Write(Stage);
            writer.Write(Id);
            writer.Write(HostId);
            writer.Write(Players.Count);
            foreach(var player in Players)
            {
                player.Value.Write(writer);
            }
            writer.Write(InvitedPlayers.Count);
            foreach(var invite in InvitedPlayers)
            {
                writer.Write(invite.Value.ToBinary());
                writer.Write(invite.Key);
            }
            writer.Write(TeamScores.Count);
            foreach(var team in TeamScores)
            {
                writer.Write(team.Key);
                writer.Write(team.Value);
            }
        }
    }
}
