using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reptile;
using System.Reflection;
using BombRushMP.Common;

namespace BombRushMP.CrewBoom
{
    /// <summary>
    /// Doing this in reflection so that I don't have to depend on CB. Dynamically loading the library like I do in CarJack gets the mod flagged every update.
    /// Maybe this will work.
    /// </summary>
    public static class CrewBoomSupport
    {
        public static bool Installed { get; private set; } = false;
        private static Dictionary<Characters, List<Guid>> _characterIds;
        private static MethodInfo _customCharactersTryGetValueMethod;
        private static PropertyInfo _customCharacterDefinitionProperty;
        private static FieldInfo _characterDefinitionIdField;
        public static void Initialize()
        {
            Installed = true;
            var characterDatabase = ReflectionUtility.GetTypeByName("CrewBoom.CharacterDatabase");
            _characterIds = characterDatabase.GetField("_characterIds", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null) as Dictionary<Characters, List<Guid>>;
        }

        public static Characters GetCharacterForGuid(Guid guid)
        {
            if (guid.Equals(Guid.Empty))
                return Characters.metalHead;
            foreach(var keyValue in _characterIds)
            {
               if (keyValue.Value == null) continue;
               if (keyValue.Value.Contains(guid))
                    return keyValue.Key;
            }
            return Characters.metalHead;
        }

        public static Guid GetGuidForCharacter(Characters character)
        {
            if (character < Characters.MAX)
                return Guid.Empty;
            if (_characterIds.TryGetValue(character, out var guids))
                return guids[0];
            return Guid.Empty;
        }
    }
}
