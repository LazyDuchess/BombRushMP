using BombRushMP.CrewBoom;
using BombRushMP.Mono;
using Reptile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using BombRushMP.PluginCommon;

namespace BombRushMP.Plugin
{
    public static class AudioLibraryUtils
    {
        public static AudioLibrary CreateFromSpecialSkin(SpecialSkinDefinition definition)
        {
            var library = new AudioLibrary();

            var jumpCollection = new AudioCollection();
            var getHitCollection = new AudioCollection();
            var dieCollection = new AudioCollection();
            var dieFallCollection = new AudioCollection();
            var comboCollection = new AudioCollection();
            var boostTrickCollection = new AudioCollection();
            var talkCollection = new AudioCollection();

            jumpCollection.Clips.AddRange(definition.Jump);
            getHitCollection.Clips.AddRange(definition.GetHit);
            dieCollection.Clips.AddRange(definition.Die);
            dieFallCollection.Clips.AddRange(definition.DieFall);
            comboCollection.Clips.AddRange(definition.Combo);
            boostTrickCollection.Clips.AddRange(definition.BoostTrick);
            talkCollection.Clips.AddRange(definition.Talk);

            library.Collections[AudioClipID.VoiceJump] = jumpCollection;
            library.Collections[AudioClipID.VoiceGetHit] = getHitCollection;
            library.Collections[AudioClipID.VoiceDie] = dieCollection;
            library.Collections[AudioClipID.VoiceDieFall] = dieFallCollection;
            library.Collections[AudioClipID.VoiceCombo] = comboCollection;
            library.Collections[AudioClipID.VoiceBoostTrick] = boostTrickCollection;
            library.Collections[AudioClipID.VoiceTalk] = talkCollection;

            return library;
        }
    }
}
