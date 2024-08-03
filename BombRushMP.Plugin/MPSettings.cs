using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using BepInEx.Configuration;

namespace BombRushMP.Plugin
{
    public class MPSettings
    {
        public static MPSettings Instance { get; private set; }
        private const byte Version = 0;
        public bool PlayerAudioEnabled = true;
        private string _savePath;
        public MPSettings(string savePath)
        {
            Instance = this;
            _savePath = savePath;
        }

        public void Load()
        {
            if (File.Exists(_savePath))
            {
                using var stream = new FileStream(_savePath, FileMode.Open);
                using var reader = new BinaryReader(stream);
                var version = reader.ReadByte();
                PlayerAudioEnabled = reader.ReadBoolean();
            }
        }

        public void Save()
        {
            var _saveDirectory = Path.GetDirectoryName(_savePath);

            if (!Directory.Exists(_saveDirectory))
                Directory.CreateDirectory(_saveDirectory);

            using var stream = new FileStream(_savePath, FileMode.Create);
            using var writer = new BinaryWriter(stream);
            writer.Write(Version);
            writer.Write(PlayerAudioEnabled);
        }
    }
}
