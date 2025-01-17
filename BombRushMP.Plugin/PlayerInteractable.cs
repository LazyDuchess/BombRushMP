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
        private PlayerSequence _sequence = null;

        private void Start()
        {
            _sequence = new PlayerSequence(this);
            PlacePlayerAtSnapPosition = false;
        }

        public override Vector3 GetLookAtPos()
        {
            return Player.Player.characterVisual.head.transform.position;
        }

        public override void Interact(Player player)
        {
            CustomSequenceHandler.instance.StartEnteringSequence(_sequence, this, true, true, false, true, true, true, true, false);
        }

        public static PlayerInteractable Create(MPPlayer player)
        {
            var interactBox = GameObject.CreatePrimitive(PrimitiveType.Cube);
            interactBox.layer = Layers.TriggerDetectPlayer;
            interactBox.transform.SetParent(player.Player.transform, false);
            interactBox.transform.SetLocalPositionAndRotation(new Vector3(0f, 0.5f, 0f), Quaternion.identity);
            interactBox.transform.localScale = new Vector3(2f, 2f, 2f);
            interactBox.GetComponent<BoxCollider>().isTrigger = true;
            var interactable = interactBox.AddComponent<PlayerInteractable>();
            interactable.Player = player;
            return interactable;
        }
    }
}
