using BombRushMP.Common;
using BombRushMP.CrewBoom;
using BombRushMP.Plugin.Gamemodes;
using CommonAPI;
using Reptile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Plugin
{
    public class MPSaveData : CustomSaveData
    {
        private const byte Version = 1;
        public static MPSaveData Instance { get; private set; }
        public Dictionary<Characters, MPCharacterData> CharacterData = new();
        public Dictionary<string, MPCharacterData> CrewBoomCharacterData = new();
        public Dictionary<GamemodeIDs, SavedGamemodeSettings> GamemodeSettings = new();
        public bool UnlockedGoonieBoard = false;
        public MPSaveData() : base("BRCMP", "saveSlot{0}.sav", SaveLocations.LocalAppData)
        {
            Instance = this;
        }

        public override void Initialize()
        {
            CharacterData = new();
            CrewBoomCharacterData = new();
            GamemodeSettings = new();
            UnlockedGoonieBoard = false;
        }

        public SavedGamemodeSettings GetSavedSettings(GamemodeIDs gamemode)
        {
            if (GamemodeSettings.TryGetValue(gamemode, out var result)) return result;
            return null;
        }

        public override void Read(BinaryReader reader)
        {
            var version = reader.ReadByte();
            if (version > Version)
            {
                FailedToLoad = true;
                return;
            }
            UnlockedGoonieBoard = reader.ReadBoolean();
            var count = reader.ReadInt32();
            for(var i = 0; i < count; i++)
            {
                var key = reader.ReadInt32();
                var moveStyleSkin = reader.ReadInt32();
                CharacterData[(Characters)key] = new MPCharacterData
                {
                    MPMoveStyleSkin = moveStyleSkin
                };
            }
            count = reader.ReadInt32();
            for (var i = 0; i < count; i++)
            {
                var key = reader.ReadString();
                var moveStyleSkin = reader.ReadInt32();
                CrewBoomCharacterData[key] = new MPCharacterData
                {
                    MPMoveStyleSkin = moveStyleSkin
                };
            }
            if (version > 0)
            {
                var gamemodeCount = reader.ReadInt32();
                for(var i = 0; i < gamemodeCount; i++)
                {
                    var gamemode = (GamemodeIDs)reader.ReadInt32();
                    var settings = new SavedGamemodeSettings();
                    settings.Read(reader);
                    GamemodeSettings[gamemode] = settings;
                }
            }
        }

        public override void Write(BinaryWriter writer)
        {
            writer.Write(Version);
            writer.Write(UnlockedGoonieBoard);
            writer.Write(CharacterData.Count);
            foreach(var charData in CharacterData)
            {
                writer.Write((int)charData.Key);
                writer.Write(charData.Value.MPMoveStyleSkin);
            }
            writer.Write(CrewBoomCharacterData.Count);
            foreach (var charData in CrewBoomCharacterData)
            {
                writer.Write(charData.Key);
                writer.Write(charData.Value.MPMoveStyleSkin);
            }
            writer.Write(GamemodeSettings.Count);
            foreach(var savedSettings in GamemodeSettings)
            {
                writer.Write((int)savedSettings.Key);
                savedSettings.Value.Write(writer);
            }
        }

        public MPCharacterData GetCharacterData(Characters character)
        {
            if (character >= Characters.MAX)
            {
                var guid = CrewBoomSupport.GetGuidForCharacter(character);
                if (!CrewBoomCharacterData.ContainsKey(guid.ToString()))
                    CrewBoomCharacterData[guid.ToString()] = new MPCharacterData();
                return CrewBoomCharacterData[guid.ToString()];
            }
            else
            {
                if (!CharacterData.ContainsKey(character))
                    CharacterData[character] = new MPCharacterData();
                return CharacterData[character];
            }
        }

        public class MPCharacterData
        {
            public int MPMoveStyleSkin = -1;
        }

        public bool ShouldDisplayGoonieBoard()
        {
            if (UnlockedGoonieBoard) return true;
            var clientController = ClientController.Instance;
            if (clientController == null) return false;
            var user = clientController.GetLocalUser();
            if (user == null) return false;
            if (user.HasTag(SpecialPlayerUtils.SpecialPlayerTag)) return true;
            return false;
        }
    }
}
