using BombRushMP.PluginCommon;
using Reptile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BombRushMP.PluginCommon
{
    public class AudioLibrary
    {
        public Dictionary<AudioClipID, AudioCollection> Collections = new();

        public AudioClip GetRandom(AudioClipID audioClipID)
        {

            if (!Collections.ContainsKey(audioClipID))
                return null;
            return Collections[audioClipID].GetRandom();
        }
    }
}
