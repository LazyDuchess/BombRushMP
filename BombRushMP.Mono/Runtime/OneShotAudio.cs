using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
#if PLUGIN
using Reptile;
#endif

namespace BombRushMP.Mono.Runtime
{
    public class OneShotAudio : MonoBehaviour
    {
        public float SFXTime = Mathf.Infinity;
        private float _timer = 0f;
#if PLUGIN
        private void Update()
        {
            if (_timer >= SFXTime)
            {
                Destroy(gameObject);
            }
            _timer += Time.deltaTime;
        }

        public static OneShotAudio Create(AudioClip clip, Vector3 pos, float pitch, float dist, float falloff)
        {
            var go = new GameObject("Ohe Shot Audio");
            go.transform.position = pos;
            var aud = go.AddComponent<AudioSource>();
            aud.outputAudioMixerGroup = Core.Instance.AudioManager.mixerGroups[3];
            aud.clip = clip;
            aud.loop = false;
            aud.playOnAwake = false;
            aud.pitch = pitch;
            aud.spatialBlend = 1f;
            aud.minDistance = dist;
            aud.maxDistance = dist + falloff;
            aud.dopplerLevel = 0f;
            aud.Play();
            var me = go.AddComponent<OneShotAudio>();
            me.SFXTime = clip.length;
            return me;
        }
#endif
    }
}
