using BombRushMP.Common;
using BombRushMP.Common.Packets;
using BombRushMP.CrewBoom;
using BombRushMP.Plugin.Patches;
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
    public class MPPlayer
    {
        public bool Local => ClientId == ClientController.Instance.LocalID;
        public ushort ClientId = 0;
        public ClientState ClientState = null;
        public ClientVisualState ClientVisualState = null;
        public Player Player;
        public bool Teleporting = true;
        public int Outfit = 0;
        private PlayerStates _previousState = PlayerStates.None;
        public Nameplate NamePlate;
        private MapPin _mapPin = null;
        private Material _mapPinMaterial = null;
        private int _lastMoveStyleSkin = -1;
        private int _lastMPMoveStyleSkin = -1;

        public void UpdateVisualState(ClientVisualState newVisualState)
        {
            ClientVisualState = newVisualState;
            if (ClientVisualState.MoveStyle >= (int)MoveStyle.MAX || ClientVisualState.MoveStyle < 0)
                ClientVisualState.MoveStyle = Mathf.Clamp(ClientVisualState.MoveStyle, 0, (int)MoveStyle.MAX - 1);
        }

        private void MakeMapPin()
        {
            var mapController = Mapcontroller.Instance;
            _mapPin = mapController.CreatePin(MapPin.PinType.StoryObjectivePin);

            _mapPin.AssignGameplayEvent(Player.gameObject);
            _mapPin.InitMapPin(MapPin.PinType.StoryObjectivePin);
            _mapPin.OnPinEnable();

            // THX SLOPCREW! For code below.
            var pinInObj = _mapPin.transform.Find("InViewVisualization").gameObject;

            // Particles. Get rid of them.
            var pinInPartObj = pinInObj.transform.Find("Particle System").gameObject;
            GameObject.Destroy(pinInPartObj);

            var pinOutObj = _mapPin.transform.Find("OutOfViewVisualization").gameObject;
            var pinOutPartS = pinOutObj.GetComponent<ParticleSystem>();
            var pinOutPartR = pinOutObj.GetComponent<ParticleSystemRenderer>();
            GameObject.Destroy(pinOutPartS);
            GameObject.Destroy(pinOutPartR);

            // Color
            var pinInMeshR = pinInObj.GetComponent<MeshRenderer>();
            if (_mapPinMaterial == null)
            {
                _mapPinMaterial = new Material(pinInMeshR.sharedMaterial);
                _mapPinMaterial.color = new Color(1f, 1f, 1f);
            }

            pinInMeshR.sharedMaterial = _mapPinMaterial;
        }

        private bool InSameLobbyAsLocalPlayer()
        {
            var clientController = ClientController.Instance;
            var localLobby = clientController.ClientLobbyManager.CurrentLobby;
            if (localLobby == null) return false;
            if (localLobby.LobbyState.Players.ContainsKey(ClientId) && localLobby.LobbyState.Players.ContainsKey(clientController.LocalID)) return true;
            return false;
        }

        private IEnumerator ApplyAnimationToPlayerDelayed(Player player, int animation, float time)
        {
            yield return null;
            switch (player.moveStyle)
            {
                case MoveStyle.BMX:
                    player.anim.runtimeAnimatorController = player.animatorControllerBMX;
                    break;
                case MoveStyle.SPECIAL_SKATEBOARD:
                case MoveStyle.SKATEBOARD:
                    player.anim.runtimeAnimatorController = player.animatorControllerSkateboard;
                    break;
                case MoveStyle.INLINE:
                    player.anim.runtimeAnimatorController = player.animatorControllerSkates;
                    break;
            }
            PlayerPatch.PlayAnimPatchEnabled = false;
            try
            {
                player.PlayAnim(animation, true, true, time);
                PlayerPatch.PlayAnimPatchEnabled = true;
                yield return null;
                PlayerPatch.PlayAnimPatchEnabled = false;
                //hax
                var clipInfo = player.anim.GetCurrentAnimatorClipInfo(0);
                time = time / clipInfo[0].clip.length;
                player.PlayAnim(animation, true, true, time);
            }
            finally
            {
                PlayerPatch.PlayAnimPatchEnabled = true;
            }
        }

        public void FrameUpdate()
        {
            if (ClientState == null || ClientVisualState == null) return;

            if (Player == null) return;

            var clientVisualStatePosition = ClientVisualState.Position.ToUnityVector3();
            var clientVisualStateVisualPosition = ClientVisualState.VisualPosition.ToUnityVector3();
            var clientVisualStateRotation = ClientVisualState.Rotation.ToUnityQuaternion();
            var clientVisualStateVisualRotation = ClientVisualState.VisualRotation.ToUnityQuaternion();
            var clientVisualStateVelocity = ClientVisualState.Velocity.ToUnityVector3();

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

            Player.motor._rigidbody.velocity = clientVisualStateVelocity;

            UpdatePhone();
            NamePlate.transform.position = Player.transform.position + (Vector3.up * 2f);
        }

        public void TickUpdate()
        {
            var mpSettings = MPSettings.Instance;

            var clientController = ClientController.Instance;

            if (!mpSettings.DebugLocalPlayer)
            {
                if (ClientId == clientController.LocalID)
                {
                    if (NamePlate != null)
                    {
                        GameObject.Destroy(NamePlate.gameObject);
                        NamePlate = null;
                    }
                    if (Player != null)
                        DeletePlayer();
                    return;
                }
            }

            var worldHandler = WorldHandler.instance;
            if (ClientState == null || ClientVisualState == null)
            {
                if (Player != null)
                {
                    DeletePlayer();
                }
                return;
            }
            var chara = (Characters)ClientState.Character;
            var fallbackChara = (Characters)ClientState.FallbackCharacter;

            if (chara >= Characters.MAX || chara <= Characters.NONE)
                chara = Characters.metalHead;

            if (fallbackChara >= Characters.MAX || fallbackChara <= Characters.NONE)
                fallbackChara = Characters.metalHead;

            if (ClientState.CrewBoomCharacter != Guid.Empty)
            {
                chara = fallbackChara;
                if (CrewBoomSupport.Installed)
                {
                    chara = CrewBoomSupport.GetCharacterForGuid(ClientState.CrewBoomCharacter, fallbackChara);
                }
            }

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

            var playerComp = PlayerComponent.Get(Player);
            playerComp.AFK = ClientVisualState.AFK;
            if (ClientState.SpecialSkin != playerComp.SpecialSkin)
            {
                SpecialSkinManager.Instance.ApplySpecialSkinToPlayer(Player, ClientState.SpecialSkin);
                justCreated = true;
            }
            if (ClientState.SpecialSkinVariant != playerComp.SpecialSkinVariant)
            {
                SpecialSkinManager.Instance.ApplySpecialSkinVariantToPlayer(Player, ClientState.SpecialSkinVariant);
            }

            if (justCreated)
            {
                Teleporting = true;
                if (ClientVisualState.CurrentAnimation != 0)
                {
                    if (mpSettings.DebugInfo)
                            Debug.Log($"BRCMP: Applying animation {ClientVisualState.CurrentAnimation} at time {ClientVisualState.CurrentAnimationTime} to player {ClientState.Name} (justCreated == true)");
                    Player.StartCoroutine(ApplyAnimationToPlayerDelayed(Player, ClientVisualState.CurrentAnimation, ClientVisualState.CurrentAnimationTime));
                }
            }

            if (!Player.anim.GetComponent<InverseKinematicsRelay>())
                Player.anim.gameObject.AddComponent<InverseKinematicsRelay>();

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
            UpdateSprayCan();
            UpdateNameplate();
            if (_mapPin == null)
                MakeMapPin();
            _mapPin.SetLocation();

            if (InSameLobbyAsLocalPlayer())
                _mapPinMaterial.color = new Color(0f, 0.9f, 0f);
            else
                _mapPinMaterial.color = new Color(0.9f, 0.9f, 0.9f);

            if (Player.characterVisual.boostpackEffectMode != (BoostpackEffectMode)ClientVisualState.BoostpackEffectMode)
                Player.characterVisual.SetBoostpackEffect((BoostpackEffectMode)ClientVisualState.BoostpackEffectMode);

            if (Player.characterVisual.frictionEffectMode != (FrictionEffectMode)ClientVisualState.FrictionEffectMode)
                Player.characterVisual.SetFrictionEffect((FrictionEffectMode)ClientVisualState.FrictionEffectMode);

            Player.SetDustEmission(ClientVisualState.DustEmissionRate);

            if (ClientVisualState.State != PlayerStates.Graffiti)
                Player.RemoveGraffitiSlash();

            if (ClientVisualState.MoveStyleSkin != _lastMoveStyleSkin || ClientVisualState.MPMoveStyleSkin != _lastMPMoveStyleSkin || justCreated)
            {
                _lastMoveStyleSkin = ClientVisualState.MoveStyleSkin;
                _lastMPMoveStyleSkin = ClientVisualState.MPMoveStyleSkin;
                if (ClientVisualState.MPMoveStyleSkin != -1)
                {
                    if (MPUnlockManager.Instance.UnlockByID.ContainsKey(ClientVisualState.MPMoveStyleSkin))
                    {
                        (MPUnlockManager.Instance.UnlockByID[ClientVisualState.MPMoveStyleSkin] as MPMoveStyleSkin).ApplyToPlayer(Player);
                    }
                }
                else
                    playerComp.ApplyMoveStyleSkin(ClientVisualState.MoveStyleSkin);
            }

            if (ClientVisualState.Hitbox)
                Player.hitbox.SetActive(true);
            else
                Player.hitbox.SetActive(false);

            if (ClientVisualState.HitboxLeftLeg)
                Player.hitboxLeftLeg.SetActive(true);
            else
                Player.hitboxLeftLeg.SetActive(false);

            if (ClientVisualState.HitboxRightLeg)
                Player.hitboxRightLeg.SetActive(true);
            else
                Player.hitboxRightLeg.SetActive(false);

            if (ClientVisualState.HitboxUpperBody)
                Player.hitboxUpperBody.SetActive(true);
            else
                Player.hitboxUpperBody.SetActive(false);

            if (ClientVisualState.HitboxAerial)
                Player.airialHitbox.SetActive(true);
            else
                Player.airialHitbox.SetActive(false);

            if (ClientVisualState.HitboxRadial)
                Player.radialHitbox.SetActive(true);
            else
                Player.radialHitbox.SetActive(false);

            if (ClientVisualState.HitboxSpray)
                Player.sprayHitbox.SetActive(true);
            else
                Player.sprayHitbox.SetActive(false);

            _previousState = ClientVisualState.State;
        }

        public void SetClientState(ClientState newClientState)
        {
            ClientState = newClientState;
            ClientState.Name = MPUtility.GetPlayerDisplayName(ClientState.Name);
        }

        private void UpdateNameplate()
        {
            var settings = MPSettings.Instance;

            if (!settings.ShowNamePlates)
            {
                if (NamePlate != null)
                {
                    GameObject.Destroy(NamePlate.gameObject);
                    NamePlate = null;
                }
                return;
            }

            if (NamePlate == null)
            {
                NamePlate = Nameplate.Create();
            }

            var name = MPUtility.GetPlayerDisplayName(ClientState);
            if (NamePlate.Label.text != name)
                NamePlate.Label.text = name;
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

        private void DeletePlayer()
        {
            var clientController = ClientController.Instance;
            var worldHandler = WorldHandler.instance;
            var mapController = Mapcontroller.Instance;
            if (_mapPin != null)
            {
                mapController.m_MapPins.Remove(_mapPin);
                _mapPin.isMapPinValid = false;
                _mapPin.DisableMapPinGameObject();
                GameObject.Destroy(_mapPin.gameObject);
                UnityEngine.Object.Destroy(_mapPinMaterial);
            }
            if (Player != null)
            {
                clientController?.MultiplayerPlayerByPlayer?.Remove(Player);
                worldHandler?.SceneObjectsRegister?.players?.Remove(Player);
                GameObject.Destroy(Player.gameObject);
                Player = null;
            }
        }

        public void Delete()
        {
            ClientState = null;
            ClientVisualState = null;
            if (NamePlate != null)
                GameObject.Destroy(NamePlate.gameObject);
            NamePlate = null;
            DeletePlayer();
        }
    }
}
