using BombRushMP.Common.Packets;
using BombRushMP.Plugin.Patches;
using Reptile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BombRushMP.Plugin
{
    public class MPPlayer
    {
        public ushort ClientId = 0;
        public ClientState ClientState = null;
        public ClientVisualState ClientVisualState = null;
        public Player Player;
        public bool Teleporting = true;
        public int Outfit = 0;

        public void FrameUpdate()
        {
            var clientController = ClientController.Instance;
            if (!clientController.Connected) return;
            //if (ClientId == clientController.LocalID) return;

            var worldHandler = WorldHandler.instance;
            if (ClientState == null || ClientVisualState == null)
            {
                if (Player != null)
                {
                    GameObject.Destroy(Player.gameObject);
                    Player = null;
                }
                return;
            }
            var chara = (Characters)ClientState.Character;
            if (chara >= Characters.MAX || chara <= Characters.NONE)
                chara = Characters.metalHead;
            var fit = ClientState.Outfit;
            if (fit < 0 || fit > 3)
                fit = 0;

            if (Player == null)
            {
                Player = MPUtility.CreateMultiplayerPlayer(chara, fit);
                Outfit = fit;
            }

            if (Player.character != chara)
            {
                Player.SetCharacter(chara, fit);
                Player.InitVisual();
                Outfit = fit;
            }

            if (Outfit != fit)
            {
                Player.SetOutfit(fit);
                Outfit = fit;
            }

            if (Teleporting)
            {
                Teleporting = false;
                worldHandler.PlacePlayerAt(Player, ClientVisualState.GetUnityPosition(), ClientVisualState.GetUnityRotation(), true);
            }
            else
            {
                Player.motor._rigidbody.velocity = ClientVisualState.GetUnityVelocity();
                Player.transform.position = Vector3.Lerp(Player.transform.position, ClientVisualState.GetUnityPosition(), Time.deltaTime * ClientConstants.PlayerInterpolation);
                Player.transform.rotation = Quaternion.Lerp(Player.transform.rotation, ClientVisualState.GetUnityRotation(), Time.deltaTime * ClientConstants.PlayerInterpolation);
                Player.visualTf.rotation = Quaternion.Lerp(Player.visualTf.rotation, ClientVisualState.GetUnityVisualRotation(), Time.deltaTime * ClientConstants.PlayerInterpolation);
            }

            PlayerPatch.PlayAnimPatchEnabled = false;

            try
            {

                if (Player.moveStyleEquipped != (MoveStyle)ClientVisualState.MoveStyle)
                {
                    Player.SetCurrentMoveStyleEquipped((MoveStyle)ClientVisualState.MoveStyle);
                }

                if (Player.usingEquippedMovestyle != ClientVisualState.MoveStyleEquipped)
                {
                    Player.SwitchToEquippedMovestyle(ClientVisualState.MoveStyleEquipped, false, true, true);
                }
            }
            finally
            {
                PlayerPatch.PlayAnimPatchEnabled = true;
            }
        }

        public void Delete()
        {
            var clientController = ClientController.Instance;
            var worldHandler = WorldHandler.instance;
            ClientState = null;
            ClientVisualState = null;
            if (Player != null)
            {
                clientController.PlayerRegistry.Remove(Player);
                worldHandler.SceneObjectsRegister.players.Remove(Player);
                GameObject.Destroy(Player.gameObject);
                Player = null;
            }
        }
    }
}
