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
        private enum BooleanMask
        {
            InGame,
            AllowTeamSwitching,
            MAX
        }
        public bool InGame = false;
        public bool AllowTeamSwitching = true;
        public GamemodeIDs Gamemode = GamemodeIDs.ScoreBattle;
        public byte[] GamemodeSettings = [];
        public int Stage = 0;
        public uint Id = 0;
        public ushort HostId = 0;
        public Dictionary<ushort, LobbyPlayer> Players = new();
        public Dictionary<ushort, DateTime> InvitedPlayers = new();

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
            var score = 0f;
            foreach(var player in Players)
            {
                if (player.Value.Team == team)
                    score += player.Value.Score;
            }
            return score;
        }

        public void Read(BinaryReader reader)
        {
            var fields = Bitfield.ReadByte(reader);
            InGame = fields[BooleanMask.InGame];
            AllowTeamSwitching = fields[BooleanMask.AllowTeamSwitching];
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
        }

        public void Write(BinaryWriter writer)
        {
            var fields = new Bitfield(BooleanMask.MAX);
            fields[BooleanMask.InGame] = InGame;
            fields[BooleanMask.AllowTeamSwitching] = AllowTeamSwitching;
            fields.WriteByte(writer);
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
        }
    }
}
