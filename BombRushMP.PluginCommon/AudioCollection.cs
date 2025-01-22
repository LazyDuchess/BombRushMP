using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BombRushMP.PluginCommon
{
    public class AudioCollection
    {
        public List<AudioClip> Clips = new();
        private AudioClip _lastAudioClip = null;

        public AudioClip GetRandom()
        {
            if (Clips.Count == 0) return null;
            if (Clips.Count == 1) return Clips[0];
            var clips = new List<AudioClip>(Clips);
            if (_lastAudioClip != null)
                clips.Remove(_lastAudioClip);
            var clip = clips[UnityEngine.Random.Range(0, clips.Count)];
            _lastAudioClip = clip;
            return clip;
        }
    }
}
