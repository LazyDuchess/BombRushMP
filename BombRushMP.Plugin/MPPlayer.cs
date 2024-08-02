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
                Player = MPUtility.CreateMultiplayerPlayer(chara, fit, this);
                Outfit = fit;
            }

            if (Player.ability != null)
                Player.StopCurrentAbility();

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

            Player.motor._rigidbody.velocity = ClientVisualState.GetUnityVelocity();

            if (Teleporting)
            {
                Teleporting = false;
                Player.SetPosAndRotHard(ClientVisualState.GetUnityPosition(), ClientVisualState.GetUnityRotation());
                Player.visualTf.localPosition = ClientVisualState.GetUnityVisualPosition();
                Player.visualTf.localRotation = ClientVisualState.GetUnityVisualRotation();
                Player.anim.SetFloat(ClientConstants.GrindDirectionHash, ClientVisualState.GrindDirection);
                Player.anim.SetFloat(ClientConstants.PhoneDirectionXHash, ClientVisualState.PhoneDirectionX);
                Player.anim.SetFloat(ClientConstants.PhoneDirectionYHash, ClientVisualState.PhoneDirectionY);
                Player.anim.SetFloat(ClientConstants.TurnDirection1Hash, ClientVisualState.TurnDirection1);
                Player.anim.SetFloat(ClientConstants.TurnDirection2Hash, ClientVisualState.TurnDirection2);
                Player.anim.SetFloat(ClientConstants.TurnDirection3Hash, ClientVisualState.TurnDirection3);
                Player.anim.SetFloat(ClientConstants.TurnDirectionSkateboardHash, ClientVisualState.TurnDirectionSkateboard);
            }
            else
            {
                Player.transform.position = Vector3.Lerp(Player.transform.position, ClientVisualState.GetUnityPosition(), Time.deltaTime * ClientConstants.PlayerInterpolation);
                Player.transform.rotation = Quaternion.Lerp(Player.transform.rotation, ClientVisualState.GetUnityRotation(), Time.deltaTime * ClientConstants.PlayerInterpolation);
                Player.visualTf.localRotation = Quaternion.Lerp(Player.visualTf.localRotation, ClientVisualState.GetUnityVisualRotation(), Time.deltaTime * ClientConstants.PlayerInterpolation);
                Player.visualTf.localPosition = Vector3.Lerp(Player.visualTf.localPosition, ClientVisualState.GetUnityVisualPosition(), Time.deltaTime * ClientConstants.PlayerInterpolation);
                Player.anim.SetFloat(ClientConstants.GrindDirectionHash, Mathf.Lerp(Player.anim.GetFloat(ClientConstants.GrindDirectionHash), ClientVisualState.GrindDirection, Time.deltaTime * ClientConstants.PlayerInterpolation));
                Player.anim.SetFloat(ClientConstants.PhoneDirectionXHash, Mathf.Lerp(Player.anim.GetFloat(ClientConstants.PhoneDirectionXHash), ClientVisualState.PhoneDirectionX, Time.deltaTime * ClientConstants.PlayerInterpolation));
                Player.anim.SetFloat(ClientConstants.PhoneDirectionYHash, Mathf.Lerp(Player.anim.GetFloat(ClientConstants.PhoneDirectionYHash), ClientVisualState.PhoneDirectionY, Time.deltaTime * ClientConstants.PlayerInterpolation));
                Player.anim.SetFloat(ClientConstants.TurnDirection1Hash, Mathf.Lerp(Player.anim.GetFloat(ClientConstants.TurnDirection1Hash), ClientVisualState.TurnDirection1, Time.deltaTime * ClientConstants.PlayerInterpolation));
                Player.anim.SetFloat(ClientConstants.TurnDirection2Hash, Mathf.Lerp(Player.anim.GetFloat(ClientConstants.TurnDirection2Hash), ClientVisualState.TurnDirection2, Time.deltaTime * ClientConstants.PlayerInterpolation));
                Player.anim.SetFloat(ClientConstants.TurnDirection3Hash, Mathf.Lerp(Player.anim.GetFloat(ClientConstants.TurnDirection3Hash), ClientVisualState.TurnDirection3, Time.deltaTime * ClientConstants.PlayerInterpolation));
                Player.anim.SetFloat(ClientConstants.TurnDirectionSkateboardHash, Mathf.Lerp(Player.anim.GetFloat(ClientConstants.TurnDirectionSkateboardHash), ClientVisualState.TurnDirectionSkateboard, Time.deltaTime * ClientConstants.PlayerInterpolation));
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
                clientController.MultiplayerPlayerByPlayer.Remove(Player);
                worldHandler.SceneObjectsRegister.players.Remove(Player);
                GameObject.Destroy(Player.gameObject);
                Player = null;
            }
        }
    }
}
