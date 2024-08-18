using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reptile;
using HarmonyLib;
using UnityEngine;
using BombRushMP.Common;

namespace BombRushMP.Plugin.Patches
{
    [HarmonyPatch(typeof(AudioManager))]
    internal static class AudioManagerPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(AudioManager.PlayVoice), new Type[] { typeof(VoicePriority), typeof(Characters), typeof(AudioClipID), typeof(AudioSource), typeof(VoicePriority) }, new ArgumentType[] { ArgumentType.Ref, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal , ArgumentType.Normal })]
        private static bool PlayVoice_Prefix(AudioManager __instance, ref VoicePriority currentPriority, Characters character, AudioClipID audioClipID, AudioSource audioSource, VoicePriority playbackPriority)
        {
            var player = audioSource.GetComponentInParent<Player>();
            if (player == null) return true;
            var playerComponent = PlayerComponent.Get(player);
            if (playerComponent == null) return true;
            if (playerComponent.SpecialSkin == SpecialSkins.None) return true;
            var library = SpecialSkinManager.Instance.GetAudioLibrary(playerComponent.SpecialSkin);
            if (library == null) return false;
            if (playbackPriority > currentPriority || !audioSource.isPlaying)
            {
                var clip = library.GetRandom(audioClipID);
                if (clip == null) return false;
                __instance.PlayNonloopingSfx(audioSource, clip, __instance.mixerGroups[5], 0f);
                currentPriority = playbackPriority;
            }
            return false;
        }
    }
}
