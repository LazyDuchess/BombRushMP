using BombRushMP.CrewBoom;
using BombRushMP.Mono;
using Reptile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BombRushMP.Plugin
{
    public class AudioLibrary
    {
        public Dictionary<AudioClipID, AudioCollection> Collections = new();

        public static AudioLibrary CreateFromCrewBoomCharacterDefinition(object definition)
        {
            var library = new AudioLibrary();

            var jumpCollection = new AudioCollection();
            var getHitCollection = new AudioCollection();
            var dieCollection = new AudioCollection();
            var dieFallCollection = new AudioCollection();
            var comboCollection = new AudioCollection();
            var boostTrickCollection = new AudioCollection();
            var talkCollection = new AudioCollection();

            var jumpClips = CrewBoomTypes.CharacterDefinitionVoiceJumpField.GetValue(definition) as AudioClip[];
            var getHitClips = CrewBoomTypes.CharacterDefinitionVoiceGetHitField.GetValue(definition) as AudioClip[];
            var dieClips = CrewBoomTypes.CharacterDefinitionVoiceDieField.GetValue(definition) as AudioClip[];
            var dieFallClips = CrewBoomTypes.CharacterDefinitionVoiceDieFallField.GetValue(definition) as AudioClip[];
            var comboClips = CrewBoomTypes.CharacterDefinitionVoiceComboField.GetValue(definition) as AudioClip[];
            var boostTrickClips = CrewBoomTypes.CharacterDefinitionVoiceBoostTrickField.GetValue(definition) as AudioClip[];
            var talkClips = CrewBoomTypes.CharacterDefinitionVoiceTalkField.GetValue(definition) as AudioClip[];

            jumpCollection.Clips.AddRange(jumpClips);
            getHitCollection.Clips.AddRange(getHitClips);
            dieCollection.Clips.AddRange(dieClips);
            dieFallCollection.Clips.AddRange(dieFallClips);
            comboCollection.Clips.AddRange(comboClips);
            boostTrickCollection.Clips.AddRange(boostTrickClips);
            talkCollection.Clips.AddRange(talkClips);

            library.Collections[AudioClipID.VoiceJump] = jumpCollection;
            library.Collections[AudioClipID.VoiceGetHit] = getHitCollection;
            library.Collections[AudioClipID.VoiceDie] = dieCollection;
            library.Collections[AudioClipID.VoiceDieFall] = dieFallCollection;
            library.Collections[AudioClipID.VoiceCombo] = comboCollection;
            library.Collections[AudioClipID.VoiceBoostTrick] = boostTrickCollection;
            library.Collections[AudioClipID.VoiceTalk] = talkCollection;
            return library;
        }

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

        public AudioClip GetRandom(AudioClipID audioClipID)
        {

           if (!Collections.ContainsKey(audioClipID))
                return null;
            return Collections[audioClipID].GetRandom();
        }
    }
}
