using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BombRushMP.Mono
{
    public class SpecialSkinDefinition : MonoBehaviour
    {
        [Header("Visuals")]
        public bool CanBlink = false;
        public string MainRendererName;
        public Material[] Variants;
        [Header("Voice Lines")]
        public AudioClip[] Jump;
        public AudioClip[] GetHit;
        public AudioClip[] Die;
        public AudioClip[] DieFall;
        public AudioClip[] Combo;
        public AudioClip[] BoostTrick;
        public AudioClip[] Talk;
    }
}
