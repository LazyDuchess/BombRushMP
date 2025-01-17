using BombRushMP.Common;
using BombRushMP.Plugin.Gamemodes;
using CommonAPI;
using Reptile;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BombRushMP.Plugin
{
    public class PlayerSequence : CustomSequence
    {
        private PlayerInteractable _interactable;

        private const string MyCrew = "<color=yellow>{0}</color>";
        private const string TheirCrew = "<color=yellow>{1}</color>";
        private const string Gamemode = "<color=yellow>{2}</color>";
        public bool SaidYes = false;
        public CharacterVisual TheirPuppet;
        private Vector3 _myPosition;
        private Quaternion _myRotation;
        private GameObject _cam;

        private string[] RivalCrewsIntros = [
            $"{MyCrew}, huh? I'm with {TheirCrew}.",
            $"What's up? You're {MyCrew}? I'm with {TheirCrew}.",
            $"Hey. You're {MyCrew}, right? I'm representing {TheirCrew}.",
            $"I'm thinking {TheirCrew} are gonna put {MyCrew} in their place. What do you think?",
            $"{TheirCrew} own this spot, kid. What's {MyCrew} gonna do about it?",
            $"{MyCrew} got nothing on {TheirCrew}, I'll show you!"
            ];

        private string[] MyCrewIntros = [
            $"Yo! I think {MyCrew} got something to prove.",
            $"I'd love to see how good {MyCrew} really are.",
            $"Let's see what's the best {MyCrew} got to offer.",
            $"Are {MyCrew} looking for something to do? I've got an idea.",
            $"I've heard {MyCrew} doesn't back out of a challenge."
            ];

        private string[] GenericIntros = [
            $"How's it hanging? Down for a challenge?",
            $"What's up? I'd like to see what you got.",
            $"Hey! I'm bored. Let's do something.",
            $"Yo! Down to hang out?"
            ];

        private string[] FriendlyIntros = [
            $"Hi! Let's kick it!",
            $"Hey! Let's hang out.",
            $"How about some friendly competition?"
            ];

        private string[] ComeBackIntro = [
            $"Changed your mind?",
            $"You're ready now?",
            $"Come on! It'll be fun."
            ];

        private string[] GamemodeDialogues = [
            $"Down for a {Gamemode}?",
            $"Let's do a {Gamemode}!",
            $"{Gamemode}. Let's do it.",
            $"How about a {Gamemode}?"
            ];

        private string[] YesOutros = [
            $"Let's hit it then!",
            $"Let's do it!",
            $"Sick! Let's do it.",
            $"Sweet.",
            $"Dope, let's go!"
            ];

        private string[] NoOutros = [
            $"Maybe next time then.",
            $"Sucks.",
            $"Scared?",
            $"That's too bad.",
            $"Lame.",
            $"Boring."
            ];

        private string[] DeadDialogues = [
            "...",
            "Maybe I should call an ambulance.",
            "That doesn't look good.",
            "They're not dead are they?",
            "I don't think they're getting up.",
            "They won't be saying much.",
            "That looks painful."
            ];

        public PlayerSequence(PlayerInteractable interactable)
        {
            _interactable = interactable;
        }

        private void JoinOtherPlayer()
        {
            var clientController = ClientController.Instance;
            Lobby otherLobby = null;
            foreach (var lobby in clientController.ClientLobbyManager.Lobbies)
            {
                if (lobby.Value.LobbyState.HostId == _interactable.Player.ClientId)
                {
                    otherLobby = lobby.Value;
                    break;
                }
            }
            if (otherLobby == null) return;
            if (clientController.ClientLobbyManager.CurrentLobby?.LobbyState.Id == otherLobby.LobbyState.Id) return;
            clientController.ClientLobbyManager.JoinLobby(otherLobby.LobbyState.Id);
        }

        public override void Stop()
        {
            base.Stop();
            MPUtility.PlaceCurrentPlayer(_myPosition, _myRotation);
            if (TheirPuppet != null)
            {
                GameObject.Destroy(TheirPuppet.gameObject);
            }
            if (_cam != null)
            {
                GameObject.Destroy(_cam);
            }
            if (SaidYes)
            {
                JoinOtherPlayer();
            }
        }

        private const float DistanceToPuppet = 1.5f;

        private Vector3 GetBestPlayerPosition()
        {
            var player = WorldHandler.instance.GetCurrentPlayer();
            var frontPos = TheirPuppet.transform.position + TheirPuppet.transform.forward * DistanceToPuppet + Vector3.up * 1f;
            var frontRPos = TheirPuppet.transform.position + TheirPuppet.transform.forward * DistanceToPuppet + TheirPuppet.transform.right * DistanceToPuppet + Vector3.up * 1f;
            var frontLPos = TheirPuppet.transform.position + TheirPuppet.transform.forward * DistanceToPuppet - TheirPuppet.transform.right * DistanceToPuppet + Vector3.up * 1f;
            if (GetValidPosition(frontPos, out var result))
                return result;
            if (GetValidPosition(frontRPos, out var result2))
                return result2;
            if (GetValidPosition(frontLPos, out var result3))
                return result3;
            if (GetValidPosition(player.transform.position + Vector3.up * 1f, out var result4))
                return result4;
            return player.transform.position;
        }

        private static int GroundMask = (1 << Layers.Default);
        private static int ObstructionMask = (1 << Layers.Default) | (1 << Layers.Junk) | (1 << Layers.NonStableSurface) | (1 << Layers.Wallrun) | (1 << Layers.VertSurface);
        private static float ObstructionHeight = 1f;
        private bool GetValidPosition(Vector3 reference, out Vector3 result)
        {
            var rayToGround = new Ray(reference, Vector3.down);
            if (Physics.Raycast(rayToGround, out var hit, 5f, GroundMask))
            {
                var myHeadPos = hit.point + Vector3.up * ObstructionHeight;
                var theirHeadPos = TheirPuppet.transform.position + Vector3.up * ObstructionHeight;
                var obstructionDist = (theirHeadPos - myHeadPos);
                var obstructionHeading = obstructionDist.normalized;
                var obstructionRay = new Ray(myHeadPos, obstructionHeading);
                if (!Physics.Raycast(obstructionRay, out var hit2, obstructionDist.magnitude, ObstructionMask))
                {
                    result = hit.point;
                    return true;
                }
            }
            result = reference;
            return false;
        }

        private void CreatePuppets()
        {
            var player = WorldHandler.instance.GetCurrentPlayer();
            var puppetInstance = GameObject.Instantiate(_interactable.Player.Player.characterVisual.gameObject);
            puppetInstance.transform.SetPositionAndRotation(_interactable.Player.Player.transform.position, _interactable.Player.Player.transform.rotation);
            TheirPuppet = puppetInstance.GetComponent<CharacterVisual>();
            _myPosition = player.transform.position;
            _myRotation = player.transform.rotation;
            var newPlayerPos = GetBestPlayerPosition();
            var bestPosHeading = (puppetInstance.transform.position - newPlayerPos).normalized;
            bestPosHeading.y = 0f;
            bestPosHeading = bestPosHeading.normalized;
            var newPlayerRot = Quaternion.LookRotation(bestPosHeading);
            MPUtility.PlaceCurrentPlayer(newPlayerPos, newPlayerRot);
            player.PlayAnim(Animator.StringToHash("idle"), true, true);
            TheirPuppet.anim.CrossFade(_interactable.Player.Player.curAnim, 0f);
            _cam = CreateCam().gameObject;
            UpdateCameraPosition();
            _interactable.StartCoroutine(UpdateCamDelayed());
            SetCamera(_cam);
        }

        private IEnumerator UpdateCamDelayed()
        {
            yield return null;
            UpdateCameraPosition();
        }

        public void UpdateCameraPosition()
        {
            var player = WorldHandler.instance.GetCurrentPlayer();
            if (_cam == null) return;
            if (TheirPuppet == null) return;
            var theirHeadPos = TheirPuppet.head.position;
            var myHeadPos = player.characterVisual.head.position;
            var headingToTheirHead = (theirHeadPos - myHeadPos).normalized;
            var leftHead = Vector3.Cross(headingToTheirHead, Vector3.up);
            var camPos = myHeadPos + (leftHead * 0.75f) - (Vector3.up * 0.25f) - (headingToTheirHead * 1f);

            _cam.transform.position = camPos;
            var camToThemHeading = (theirHeadPos - camPos).normalized;
            _cam.transform.rotation = Quaternion.LookRotation(camToThemHeading);
        }

        private Camera CreateCam()
        {
            var camGo = new GameObject("Sequence Camera");
            camGo.gameObject.SetActive(false);
            var cam = camGo.AddComponent<Camera>();
            camGo.AddComponent<AudioListener>();
            camGo.AddComponent<CameraRegisterer>();
            cam.fieldOfView = 50f;
            return cam;
        }

        public override void Play()
        {
            base.Play();
            var clientController = ClientController.Instance;
            var localPlayer = clientController.Players[clientController.LocalID];
            Lobby otherLobby = null;
            foreach(var lobby in clientController.ClientLobbyManager.Lobbies)
            {
                if (lobby.Value.LobbyState.HostId == _interactable.Player.ClientId)
                {
                    otherLobby = lobby.Value;
                    break;
                }
            }

            if (otherLobby == null) return;

            var myCrew = TMPFilter.RemoveAllTags(MPUtility.GetCrewDisplayName(localPlayer.ClientState.CrewName)).Trim();
            var theirCrew = TMPFilter.RemoveAllTags(MPUtility.GetCrewDisplayName(_interactable.Player.ClientState.CrewName)).Trim();
            var theirName = TMPFilter.RemoveAllTags(MPUtility.GetPlayerDisplayName(_interactable.Player.ClientState.Name)).Trim();
            var myName = TMPFilter.RemoveAllTags(MPUtility.GetPlayerDisplayName(localPlayer.ClientState.Name)).Trim();
            var gamemodeName = GamemodeFactory.GetGamemodeName(otherLobby.LobbyState.Gamemode);

            var possibleIntros = new List<string>();

            if (_interactable.AlreadyTalked)
            {
                possibleIntros.AddRange(ComeBackIntro);
            }
            else if (string.IsNullOrWhiteSpace(myCrew) && string.IsNullOrWhiteSpace(theirCrew))
            {
                possibleIntros.AddRange(GenericIntros);
            }
            else if (!string.IsNullOrWhiteSpace(myCrew) && string.IsNullOrWhiteSpace(theirCrew))
            {
                possibleIntros.AddRange(MyCrewIntros);
            }
            else if (!string.IsNullOrWhiteSpace(myCrew) && !string.IsNullOrWhiteSpace(theirCrew) && myCrew == theirCrew)
            {
                possibleIntros.AddRange(FriendlyIntros);
                possibleIntros.AddRange(GenericIntros);
            }
            else if (!string.IsNullOrWhiteSpace(myCrew) && !string.IsNullOrWhiteSpace(theirCrew) && myCrew != theirCrew)
            {
                possibleIntros.AddRange(RivalCrewsIntros);
            }
            else
            {
                possibleIntros.AddRange(GenericIntros);
            }

            var introString = string.Format(possibleIntros[UnityEngine.Random.Range(0, possibleIntros.Count)], myCrew, theirCrew, gamemodeName);
            var questionString = string.Format(GamemodeDialogues[UnityEngine.Random.Range(0, GamemodeDialogues.Length)], myCrew, theirCrew, gamemodeName);
            var yesString = YesOutros[UnityEngine.Random.Range(0, YesOutros.Length)];
            var noString = NoOutros[UnityEngine.Random.Range(0, NoOutros.Length)];
            var deadString = DeadDialogues[UnityEngine.Random.Range(0, DeadDialogues.Length)];

            var yesDialogue = new CustomDialogue(theirName, yesString) { EndSequenceOnFinish = true };
            var noDialogue = new CustomDialogue(theirName, noString) { EndSequenceOnFinish = true };
            var questionDialogue = new CustomDialogue(theirName, questionString);
            var introDialogue = new CustomDialogue(theirName, introString, questionDialogue);
            var deadDialogue = new CustomDialogue(myName, deadString) { EndSequenceOnFinish = true };

            questionDialogue.OnDialogueBegin = () =>
            {
                _interactable.AlreadyTalked = true;
                RequestYesNoPrompt();
            };

            questionDialogue.OnDialogueEnd = () =>
            {
                if (questionDialogue.AnsweredYes)
                {
                    SaidYes = true;
                    StartDialogue(yesDialogue);
                }
                else
                {
                    SaidYes = false;
                    StartDialogue(noDialogue);
                }
            };

            CreatePuppets();

            if (_interactable.Player.ClientVisualState.State != PlayerStates.Dead)
            {
                StartDialogue(introDialogue);
                SaidYes = true;
            }
            else
            {
                StartDialogue(deadDialogue);
                SaidYes = false;
            }
        }
    }
}
