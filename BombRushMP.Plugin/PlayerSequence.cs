using BombRushMP.Common;
using BombRushMP.Plugin.Gamemodes;
using CommonAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Plugin
{
    public class PlayerSequence : CustomSequence
    {
        private PlayerInteractable _interactable;

        private const string MyCrew = "<color=yellow>{0}</color>";
        private const string TheirCrew = "<color=yellow>{1}</color>";
        private const string Gamemode = "<color=yellow>{2}</color>";
        public bool SaidYes = false;

        private string[] RivalCrewsIntros = [
            $"{MyCrew}, huh? I'm with {TheirCrew}.",
            $"What's up? You're {MyCrew}? I'm with {TheirCrew}.",
            $"Hey. You're {MyCrew}, right? I'm representing {TheirCrew}.",
            $"I'm thinking {TheirCrew} are gonna put {MyCrew} in their place. What do you think?",
            $"{TheirCrew} own this spot, kid. What's {MyCrew} gonna do about it?",
            $"{MyCrew}'s got nothing on {TheirCrew}, I'll show you!"
            ];

        private string[] MyCrewIntros = [
            $"Yo! I think {MyCrew}'s got something to prove.",
            $"I'd love to see how good {MyCrew} really are.",
            $"What's the best {MyCrew}'s got to offer?",
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
            if (SaidYes)
            {
                JoinOtherPlayer();
            }
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
