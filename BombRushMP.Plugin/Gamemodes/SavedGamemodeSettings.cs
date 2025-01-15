using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Plugin.Gamemodes
{
    public class SavedGamemodeSettings
    {
        public Dictionary<int, int> ValueByID = new();
        private const byte Version = 0;

        public void Read(BinaryReader reader)
        {
            var version = reader.ReadByte();
            var count = reader.ReadInt32();
            for(var i = 0; i < count; i++)
            {
                var key = reader.ReadInt32();
                var val = reader.ReadInt32();
                ValueByID[key] = val;
            }
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(Version);
            writer.Write(ValueByID.Count);
            foreach(var value in ValueByID)
            {
                writer.Write(value.Key);
                writer.Write(value.Value);
            }
        }
    }
}
