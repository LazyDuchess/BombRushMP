using BombRushMP.CrewBoom;
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
        public static MPSaveData Instance { get; private set; }
        public Dictionary<Characters, MPCharacterData> CharacterData = new();
        public Dictionary<string, MPCharacterData> CrewBoomCharacterData = new();
        public MPSaveData() : base("BRCMP", "saveSlot{0}.sav", SaveLocations.LocalAppData)
        {
            Instance = this;
        }

        public override void Initialize()
        {
            CharacterData = new();
            CrewBoomCharacterData = new();
        }

        public override void Read(BinaryReader reader)
        {
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
        }

        public override void Write(BinaryWriter writer)
        {
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
    }
}
