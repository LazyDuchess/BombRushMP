#if PLUGIN
using UnityEngine;
using CommonAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Mono.Runtime
{
    public class CyberStartDialogue : CustomSequence
    {
        private GameObject _camera;
        private bool _saidYes = true;
        private CustomSequence _followUpSequence;

        public CyberStartDialogue(GameObject camera, CustomSequence followUpSequence)
        {
            _camera = camera;
            _followUpSequence = followUpSequence;
        }

        public override void Play()
        {
            base.Play();
            _saidYes = true;
            SetCamera(_camera);
            var askDialogue = new CustomDialogue(string.Empty, "Start Event Course?");
            askDialogue.EndSequenceOnFinish = true;
            askDialogue.OnDialogueBegin += () => {
                RequestYesNoPrompt();
            };
            askDialogue.OnDialogueEnd += () =>
            {
                _saidYes = askDialogue.AnsweredYes;
            };
            StartDialogue(askDialogue);
        }

        public override void Stop()
        {
            base.Stop();
            if (_saidYes)
            {
                CustomSequenceHandler.instance.StartEnteringSequence(_followUpSequence, null, false, true, false, true, true, true, true, false);
            }
        }
    }
}
#endif