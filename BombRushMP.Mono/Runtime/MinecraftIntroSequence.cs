#if PLUGIN
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reptile;
using UnityEngine;
using CommonAPI;
using BombRushMP.PluginCommon;

namespace BombRushMP.Mono.Runtime
{
    public class MinecraftIntroSequence : CustomSequence
    {
        private GameObject _introDialogueCamera;
        private GameObject _noteCamera;

        private CustomDialogue _diag1;
        private CustomDialogue _diag2;
        private CustomDialogue _diag3;
        private CustomDialogue _diag4;
        private CustomDialogue _diag5;
        private CustomDialogue _diag6;

        private const string DJName = "DJ CYBER";

        public MinecraftIntroSequence(GameObject introCamera, GameObject noteCamera)
        {
            _introDialogueCamera = introCamera;
            _noteCamera = noteCamera;

            _diag1 = new CustomDialogue(DJName, "Red. I've been looking for you.");
            _diag2 = new CustomDialogue(DJName, "Does this look familiar to you?");
            _diag3 = new CustomDialogue(DJName, "Looks innocent enough, but this little Christmas wish of yours is a huge roadblock.");
            _diag4 = new CustomDialogue(DJName, "I suspect it has created an alternate reality. One where <color=yellow>Minecraft</color> is real. Part of your mind is stuck in there right now.");
            _diag5 = new CustomDialogue(DJName, "We can't bring your memories back, much less defeat Faux and go All City if we don't get your mind out of that reality.");
            _diag6 = new CustomDialogue(DJName, "Please, jump in. Find a way out of there, and make it quick. It all depends on it.");

            _diag1.OnDialogueEnd += () =>
            {
                SetCamera(_noteCamera);
                StartDialogue(_diag2);
            };

            _diag2.NextDialogue = _diag3;

            _diag3.OnDialogueEnd += () =>
            {
                SetCamera(_introDialogueCamera);
                StartDialogue(_diag4);
            };
            _diag4.NextDialogue = _diag5;
            _diag5.NextDialogue = _diag6;
            _diag6.EndSequenceOnFinish = true;
        }

        public override void Play()
        {
            base.Play();
            SharedUtils.SetLocalCharacter(Characters.metalHead);
            SetCamera(_introDialogueCamera);
            StartDialogue(_diag1);
        }
    }
}
#endif