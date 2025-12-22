#if PLUGIN
using BombRushMP.Common;
using BombRushMP.PluginCommon;
using CommonAPI;
using Reptile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace BombRushMP.Mono.Runtime
{
    public class MinecraftIntroSequence : CustomSequence
    {
        private GameObject _introDialogueCamera;
        private GameObject _noteCamera;
        private GameObject _getInCamera;
        private GameObject _minecraftCamera;

        private CustomDialogue _diag1;
        private CustomDialogue _diag2;
        private CustomDialogue _diag3;
        private CustomDialogue _diag4;
        private CustomDialogue _diag5;
        private CustomDialogue _diag6;
        private CustomDialogue _diag7;

        private Animator _djAnimator;
        private Animator _vehicleAnimator;
        private GameObject _noteGameObject;
        private GameObject _redGetInGameObject;

        private Transform _minecraftSpawnPoint;

        private const string DJName = "DJ CYBER";
        private const string RedName = "RED";

        private void CleanUp()
        {
            _djAnimator.Play("NPC Idle");
            _vehicleAnimator.Play("Idle");
            _noteGameObject.SetActive(false);
            SharedUtils.SetLocalSpecialSkin(SpecialSkins.Minecraft);
            PlaceAtMinecraft();
            _redGetInGameObject.SetActive(false);
        }

        private void PlaceAtMinecraft()
        {
            player.CompletelyStop();
            WorldHandler.instance.PlacePlayerAt(player, _minecraftSpawnPoint.position, _minecraftSpawnPoint.rotation, true);
            player.CompletelyStop();
            player.SetPosAndRotHard(_minecraftSpawnPoint.position, _minecraftSpawnPoint.rotation);
        }

        public MinecraftIntroSequence(GameObject introCamera, GameObject noteCamera, Animator djAnimator, GameObject note, Animator vehicleAnimator, Transform minecraftSpawnPoint, GameObject redGetIn, GameObject getInCamera, GameObject minecraftCamera)
        {
            _introDialogueCamera = introCamera;
            _noteCamera = noteCamera;
            _djAnimator = djAnimator;
            _noteGameObject = note;
            _minecraftSpawnPoint = minecraftSpawnPoint;
            _vehicleAnimator = vehicleAnimator;
            _redGetInGameObject = redGetIn;
            _getInCamera = getInCamera;
            _minecraftCamera = minecraftCamera;
        }

        private void InitializeDialogue()
        {
            _diag1 = new CustomDialogue(DJName, "Red. I've been looking for you.");
            _diag2 = new CustomDialogue(DJName, "Does this look familiar to you?");
            _diag3 = new CustomDialogue(DJName, "Looks innocent enough, but this little Christmas wish of yours is a huge roadblock.");
            _diag4 = new CustomDialogue(DJName, "I suspect it has created an alternate reality. One where <color=yellow>Minecraft</color> is real. Part of your mind is stuck in there right now.");
            _diag5 = new CustomDialogue(DJName, "We can't bring your memories back, much less defeat Faux and go All City if we don't get your mind out of that reality.");
            _diag6 = new CustomDialogue(DJName, "Please, jump in. Find a way out of there, and make it quick. It all depends on it.");
            _diag7 = new CustomDialogue(RedName, "Alright, let's find a way out.");

            _diag1.OnDialogueEnd += () =>
            {
                _djAnimator.SetTrigger("ShowNote");
                _noteGameObject.SetActive(true);
                SetCamera(_noteCamera);
                StartDialogue(_diag2);
            };

            _diag2.NextDialogue = _diag3;

            _diag3.OnDialogueEnd += () =>
            {
                _djAnimator.SetTrigger("HideNote");
                SetCamera(_introDialogueCamera);
                StartDialogue(_diag4);
            };
            _diag4.NextDialogue = _diag5;
            _diag5.NextDialogue = _diag6;

            _diag6.OnDialogueEnd += () =>
            {
                _vehicleAnimator.SetTrigger("Open");
                _redGetInGameObject.SetActive(true);
                SharedUtils.SetLocalSpecialSkin(SpecialSkins.Minecraft);
                PlaceAtMinecraft();
                StartDialogue(_diag7, 10f);
                SetCamera(_getInCamera);
            };

            _diag7.EndSequenceOnFinish = true;
            _diag7.OnDialogueBegin += () =>
            {
                SetCamera(_minecraftCamera);
            };
        }

        public override void Play()
        {
            base.Play();
            InitializeDialogue();
            SharedUtils.SetLocalCharacter(Characters.metalHead);
            SetCamera(_introDialogueCamera);
            StartDialogue(_diag1);
        }

        public override void Stop()
        {
            base.Stop();
            CleanUp();
        }
    }
}
#endif