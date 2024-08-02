using BombRushMP.Common;
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
        private PlayerStates _previousState = PlayerStates.None;

        public void FrameUpdate()
        {
            var clientController = ClientController.Instance;

            if (!clientController.Connected) return;

            if (!clientController.DebugNetworkedLocalPlayer)
                if (ClientId == clientController.LocalID) return;

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

            var justCreated = false;

            if (Player == null)
            {
                Player = MPUtility.CreateMultiplayerPlayer(chara, fit, this);
                Outfit = fit;
                justCreated = true;
            }

            if (Player.character != chara)
            {
                Player.SetCharacter(chara, fit);
                Player.InitVisual();
                Outfit = fit;
                justCreated = true;
            }

            if (Outfit != fit)
            {
                Player.SetOutfit(fit);
                Outfit = fit;
            }

            if (justCreated)
            {
                if (ClientVisualState.CurrentAnimation != 0)
                {
                    PlayerPatch.PlayAnimPatchEnabled = false;
                    try
                    {
                        Player.PlayAnim(ClientVisualState.CurrentAnimation, true, true, ClientVisualState.CurrentAnimationTime);
                    }
                    finally
                    {
                        PlayerPatch.PlayAnimPatchEnabled = true;
                    }
                }
            }

            if (!Player.anim.GetComponent<InverseKinematicsRelay>())
                Player.anim.gameObject.AddComponent<InverseKinematicsRelay>();

            var clientVisualStatePosition = ClientVisualState.Position.ToUnityVector3();
            var clientVisualStateVisualPosition = ClientVisualState.VisualPosition.ToUnityVector3();
            var clientVisualStateRotation = ClientVisualState.Rotation.ToUnityQuaternion();
            var clientVisualStateVisualRotation = ClientVisualState.VisualRotation.ToUnityQuaternion();
            var clientVisualStateVelocity = ClientVisualState.Velocity.ToUnityVector3();

            Player.motor._rigidbody.velocity = clientVisualStateVelocity;

            if (Teleporting)
            {
                Teleporting = false;
                Player.SetPosAndRotHard(clientVisualStatePosition, clientVisualStateRotation);
                Player.visualTf.localPosition = clientVisualStateVisualPosition;
                Player.visualTf.localRotation = clientVisualStateVisualRotation;
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
                Player.transform.position = Vector3.Lerp(Player.transform.position, clientVisualStatePosition, Time.deltaTime * ClientConstants.PlayerInterpolation);
                Player.transform.rotation = Quaternion.Lerp(Player.transform.rotation, clientVisualStateRotation, Time.deltaTime * ClientConstants.PlayerInterpolation);
                Player.visualTf.localRotation = Quaternion.Lerp(Player.visualTf.localRotation, clientVisualStateVisualRotation, Time.deltaTime * ClientConstants.PlayerInterpolation);
                Player.visualTf.localPosition = Vector3.Lerp(Player.visualTf.localPosition, clientVisualStateVisualPosition, Time.deltaTime * ClientConstants.PlayerInterpolation);
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
                    Player.SwitchToEquippedMovestyle(ClientVisualState.MoveStyleEquipped, false, true, false);
                }
            }
            finally
            {
                PlayerPatch.PlayAnimPatchEnabled = true;
            }

            UpdateLookAt();
            UpdatePhone();
            UpdateSprayCan();

            if (Player.characterVisual.boostpackEffectMode != (BoostpackEffectMode)ClientVisualState.BoostpackEffectMode)
                Player.characterVisual.SetBoostpackEffect((BoostpackEffectMode)ClientVisualState.BoostpackEffectMode);

            if (Player.characterVisual.frictionEffectMode != (FrictionEffectMode)ClientVisualState.FrictionEffectMode)
                Player.characterVisual.SetFrictionEffect((FrictionEffectMode)ClientVisualState.FrictionEffectMode);

            Player.SetDustEmission(ClientVisualState.DustEmissionRate);

            if (ClientVisualState.State != PlayerStates.Graffiti)
                Player.RemoveGraffitiSlash();

            _previousState = ClientVisualState.State;
        }

        private void UpdateSprayCan()
        {
            if (ClientVisualState.State == PlayerStates.Graffiti)
                Player.characterVisual.SetSpraycan(true);
        }

        private void UpdateLookAt()
        {
            Player.characterVisual.lookAtVel = Player.GetPracticalWorldVelocity();
            Player.characterVisual.lookAtSubject = null;
            Player.characterVisual.phoneActive = false;
        }

        private void UpdatePhone()
        {
            if (ClientVisualState.PhoneHeld)
            {
                Player.phoneLayerWeight += Player.grabPhoneSpeed * Core.dt;
                Player.characterVisual.SetPhone(true);
                if (Player.phoneLayerWeight >= 1f)
                {
                    Player.phoneLayerWeight = 1f;
                    Player.characterVisual.phoneActive = true;
                    Player.characterVisual.lookAtSubject = Player.characterVisual.VFX.phone;
                    Player.characterVisual.lookAtPos = Vector3.Lerp(Player.characterVisual.lookAtPos, Player.characterVisual.VFX.phone.transform.position + Vector3.up * 0.25f, Mathf.Clamp(Player.characterVisual.lookAtVel.magnitude * Core.dt * 3f, 6f * Core.dt, 1f));
                }
            }
            else
            {
                Player.phoneLayerWeight -= Player.grabPhoneSpeed * Core.dt;
                if (Player.phoneLayerWeight <= 0f)
                {
                    Player.phoneLayerWeight = 0f;
                    Player.characterVisual.SetPhone(false);
                }
            }
            Player.anim.SetLayerWeight(3, Player.phoneLayerWeight);
        }

        public void Delete()
        {
            var clientController = ClientController.Instance;
            var worldHandler = WorldHandler.instance;
            ClientState = null;
            ClientVisualState = null;
            if (Player != null)
            {
                clientController?.MultiplayerPlayerByPlayer?.Remove(Player);
                worldHandler?.SceneObjectsRegister?.players?.Remove(Player);
                GameObject.Destroy(Player.gameObject);
                Player = null;
            }
        }
    }
}
