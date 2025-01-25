using BepInEx.Bootstrap;
using BombRushMP.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.BunchOfEmotes
{
    public static class BunchOfEmotesSupport
    {
        public static bool Installed { get; private set; } = false;
        private static object BoEPluginInstance = null;
        private static Type BoEPluginType = null;
        private static FieldInfo CustomAnimsField = null;
        private static bool Cached = false;
        private static Dictionary<int, int> GameAnimationByCustomAnimationHash = new();
        private static Dictionary<int, int> CustomAnimationHashByGameAnimation = new();

        public static void Initialize()
        {
            Installed = true;
            BoEPluginInstance = Chainloader.PluginInfos["com.Dragsun.BunchOfEmotes"].Instance;
            BoEPluginType = ReflectionUtility.GetTypeByName("BunchOfEmotes.BunchOfEmotesPlugin");
            CustomAnimsField = BoEPluginType.GetField("myCustomAnims2");
        }

        public static void CacheAnimationsIfNecessary()
        {
            if (Cached) return;
            Cached = true;
            var boeDictionary = CustomAnimsField.GetValue(BoEPluginInstance) as Dictionary<int, string>;
            foreach(var customAnim in boeDictionary)
            {
                var gameAnim = customAnim.Key;
                var hash = Compression.HashString(customAnim.Value);
                GameAnimationByCustomAnimationHash[hash] = gameAnim;
                CustomAnimationHashByGameAnimation[gameAnim] = hash;
            }
        }

        public static bool TryGetGameAnimationForCustomAnimationHash(int hash, out int gameAnim)
        {
            if (!Installed)
            {
                gameAnim = 0;
                return false;
            }
            CacheAnimationsIfNecessary();
            if (GameAnimationByCustomAnimationHash.TryGetValue(hash, out gameAnim))
                return true;
            return false;
        }

        public static bool TryGetCustomAnimationHashByGameAnimation(int gameAnim, out int hash)
        {
            if (!Installed)
            {
                hash = 0;
                return false;
            }
            CacheAnimationsIfNecessary();
            if (CustomAnimationHashByGameAnimation.TryGetValue(gameAnim, out hash))
                return true;
            return false;
        }
    }
}
