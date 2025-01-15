using BombRushMP.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Plugin
{
    public class MPStats
    {
        private const byte Version = 0;
        public uint ElitesPlayedAgainst = 0;
        public uint ElitesBeaten = 0;
        public uint TimesFallenAsleep = 0;
        public double TimeSpentAsleep = 0f;
        public double TimeSpentSpectating = 0f;
        public uint PlayersHit = 0;
        public uint TimesHit = 0;
        public uint Deaths = 0;
        public uint TimesSaidHello = 0;
        public Dictionary<GamemodeIDs, uint> GamemodesPlayed = new();
        public Dictionary<GamemodeIDs, uint> GamemodesWon = new();

        public void IncreaseGamemodesPlayed(GamemodeIDs gamemode)
        {
            uint val = 0;
            if (GamemodesPlayed.TryGetValue(gamemode, out var result))
                val = result;
            val++;
            GamemodesPlayed[gamemode] = val;
        }

        public void IncreaseGamemodesWon(GamemodeIDs gamemode)
        {
            uint val = 0;
            if (GamemodesWon.TryGetValue(gamemode, out var result))
                val = result;
            val++;
            GamemodesWon[gamemode] = val;
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(Version);
            writer.Write(ElitesPlayedAgainst);
            writer.Write(ElitesBeaten);
            writer.Write(TimesFallenAsleep);
            writer.Write(TimeSpentAsleep);
            writer.Write(TimeSpentSpectating);
            writer.Write(PlayersHit);
            writer.Write(TimesHit);
            writer.Write(Deaths);
            writer.Write(TimesSaidHello);
            writer.Write(GamemodesPlayed.Count);
            foreach(var gamemodePlayed in GamemodesPlayed)
            {
                writer.Write((int)gamemodePlayed.Key);
                writer.Write(gamemodePlayed.Value);
            }
            writer.Write(GamemodesWon.Count);
            foreach(var gamemodeWon in GamemodesWon)
            {
                writer.Write((int)gamemodeWon.Key);
                writer.Write(gamemodeWon.Value);
            }
        }

        public void Read(BinaryReader reader)
        {
            var version = reader.ReadByte();
            ElitesPlayedAgainst = reader.ReadUInt32();
            ElitesBeaten = reader.ReadUInt32();
            TimesFallenAsleep = reader.ReadUInt32();
            TimeSpentAsleep = reader.ReadDouble();
            TimeSpentSpectating = reader.ReadDouble();
            PlayersHit = reader.ReadUInt32();
            TimesHit = reader.ReadUInt32();
            Deaths = reader.ReadUInt32();
            TimesSaidHello = reader.ReadUInt32();
            var gamemodesPlayed = reader.ReadInt32();
            for(var i = 0; i < gamemodesPlayed; i++)
            {
                var key = reader.ReadInt32();
                var value = reader.ReadUInt32();
                GamemodesPlayed[(GamemodeIDs)key] = value;
            }
            var gamemodesWon = reader.ReadInt32();
            for (var i = 0; i < gamemodesWon; i++)
            {
                var key = reader.ReadInt32();
                var value = reader.ReadUInt32();
                GamemodesWon[(GamemodeIDs)key] = value;
            }
        }
    }
}
