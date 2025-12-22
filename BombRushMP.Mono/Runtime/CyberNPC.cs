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
#if PLUGIN
        private CustomSequence _dialogueSequence;
        private CustomSequence _minecraftIntroSequence;
        private void Awake()
        {
            _minecraftIntroSequence = new MinecraftIntroSequence(DialogueCamera, NoteCamera);
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
