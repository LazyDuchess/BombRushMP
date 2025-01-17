using CommonAPI;
using Reptile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BombRushMP.Plugin
{
    public class PlayerInteractable : CustomInteractable
    {
        public MPPlayer Player = null;
        public bool AlreadyTalked = false;
        public PlayerSequence Sequence = null;

        private void Start()
        {
            Sequence = new PlayerSequence(this);
            PlacePlayerAtSnapPosition = false;
        }

        private Lobby GetLobby()
        {
            var clientController = ClientController.Instance;
            foreach(var lobby in clientController.ClientLobbyManager.Lobbies)
            {
                if (lobby.Value.LobbyState.HostId == Player.ClientId)
                    return lobby.Value;
            }
            return null;
        }

        private void Update()
        {
            var player = WorldHandler.instance.GetCurrentPlayer();
            var sequenceHandler = CustomSequenceHandler.instance;
            if (sequenceHandler.sequence == Sequence && (player.sequenceState == SequenceState.IN_SEQUENCE || player.sequenceState == SequenceState.EXITING))
            {
                var lobby = GetLobby();
                if (Player == null || lobby == null || !lobby.LobbyState.Challenge || lobby.LobbyState.InGame)
                {
                    Sequence.SaidYes = false;
                    sequenceHandler.ExitCurrentSequence();
                }
            }
        }

        private void OnDestroy()
        {
            var sequenceHandler = CustomSequenceHandler.instance;
            if (sequenceHandler.sequence == Sequence && sequenceHandler.IsInSequence())
            {
                Sequence.SaidYes = false;
                sequenceHandler.ExitCurrentSequence();
            }
        }

        private void OnDisable()
        {
            var sequenceHandler = CustomSequenceHandler.instance;
            if (sequenceHandler.sequence == Sequence && sequenceHandler.IsInSequence())
            {
                Sequence.SaidYes = false;
                sequenceHandler.ExitCurrentSequence();
            }
        }

        public override Vector3 GetLookAtPos()
        {
            return Player.Player.characterVisual.head.transform.position;
        }

        public override void Interact(Player player)
        {
            CustomSequenceHandler.instance.StartEnteringSequence(Sequence, this, false, true, false, true, true, true, true, false);
        }

        public static PlayerInteractable Create(MPPlayer player)
        {
            var interactBox = GameObject.CreatePrimitive(PrimitiveType.Cube);
            interactBox.layer = Layers.TriggerDetectPlayer;
            interactBox.transform.SetParent(player.Player.transform, false);
            interactBox.transform.SetLocalPositionAndRotation(new Vector3(0f, 0.5f, 0f), Quaternion.identity);
            interactBox.transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);
            interactBox.GetComponent<BoxCollider>().isTrigger = true;
            var interactable = interactBox.AddComponent<PlayerInteractable>();
            interactable.Player = player;
            return interactable;
        }
    }
}
