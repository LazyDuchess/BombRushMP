using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
#if PLUGIN
using CommonAPI;
using Reptile;
#endif

namespace BombRushMP.Mono.Runtime
{
#if PLUGIN
    public class CyberNPC : CustomInteractable
#else
    public class CyberNPC : MonoBehaviour
#endif
    {
        public GameObject DialogueCamera;
        public GameObject NoteCamera;

        public GameObject Note;
        public Animator DjAnimator;
        public Animator VehicleAnimator;

        public Transform MinecraftSpawnPoint;
        public GameObject RedGetIn;
        public GameObject GetInCamera;
        public GameObject MinecraftCamera;
#if PLUGIN
        private CustomSequence _dialogueSequence;
        private CustomSequence _minecraftIntroSequence;
        private void Awake()
        {
            _minecraftIntroSequence = new MinecraftIntroSequence(DialogueCamera, NoteCamera, DjAnimator, Note, VehicleAnimator, MinecraftSpawnPoint, RedGetIn, GetInCamera, MinecraftCamera); ;
            _dialogueSequence = new CyberStartDialogue(DialogueCamera, _minecraftIntroSequence);
            PlacePlayerAtSnapPosition = true;
        }

        public override void Interact(Player player)
        {
            CustomSequenceHandler.instance.StartEnteringSequence(_dialogueSequence, this, false, true, false, true, true, true, true, false);
        }
#endif
    }
}
