using BombRushMP.BunchOfEmotes;
using BombRushMP.Common;
using BombRushMP.Common.Packets;
using BombRushMP.CrewBoom;
using BombRushMP.Plugin.Gamemodes;
using BombRushMP.Plugin.Patches;
using CommonAPI;
using Reptile;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.LowLevel;

namespace BombRushMP.Plugin
{
    public class MPPlayer
    {
        public bool Local => ClientId == ClientController.Instance.LocalID;
        public ushort ClientId = 0;
        public ClientState ClientState = null;
        public ClientVisualState ClientVisualState = null;
        public Player Player;
        public PlayerComponent PlayerComponent
        {
            get
            {
                if (_cachedPlayerComp == null)
                    _cachedPlayerComp = PlayerComponent.Get(Player);
                return _cachedPlayerComp;
            }
        }
        private PlayerComponent _cachedPlayerComp = null;
        public bool Teleporting = true;
        public int Outfit = 0;
        private PlayerStates _previousState = PlayerStates.Normal;
        public Nameplate NamePlate;
        private Lobby _lobby;
        private MapPin _mapPin = null;
        private GameObject _mapPinParticles = null;
        private Material _mapPinMaterial = null;
        private int _lastMoveStyleSkin = -1;
        private int _lastMPMoveStyleSkin = -1;

        private int _lastAnimation = 0;
        private float _timeSpentInWrongAnimation = 0f;
        private const float MaxTimeSpentInWrongAnimation = 0.5f;
        private PlayerInteractable _interactable = null;

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
            _mapPinParticles = pinInObj.transform.Find("Particle System").gameObject;
            _mapPinParticles.SetActive(false);

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

        private bool IsSpecialPlayer()
        {
            return ClientState.User != null && ClientState.User.HasTag(SpecialPlayerUtils.SpecialPlayerTag);
        }

        private bool InSameLobbyAsLocalPlayer()
        {
            var clientController = ClientController.Instance;
            var localLobby = clientController.ClientLobbyManager.CurrentLobby;
            if (localLobby == null) return false;
            if (_lobby == null) return false;
            return localLobby == _lobby;
        }

        private bool InOurTeamLobby(out bool rival)
        {
            rival = false;
            var clientController = ClientController.Instance;
            var localLobby = clientController.ClientLobbyManager.CurrentLobby;
            if (localLobby == null) return false;
            if (_lobby == null) return false;
            if (localLobby == _lobby)
            {
                var myPlayer = localLobby.LobbyState.Players[clientController.LocalID];
                var otherPlayer = localLobby.LobbyState.Players[ClientId];
                var gm = GamemodeFactory.GetGamemode(localLobby.LobbyState.Gamemode);
                rival = (otherPlayer.Team != myPlayer.Team);
                return gm.TeamBased;
            }
            return false;
        }

        private bool IsChallengeable()
        {
            var clientController = ClientController.Instance;
            var localLobby = clientController.ClientLobbyManager.CurrentLobby;
            if (localLobby != null) return false;
            if (_lobby == null) return false;
            return (_lobby.LobbyState.Challenge && !_lobby.LobbyState.InGame && _lobby.LobbyState.HostId == ClientId);
        }

        private IEnumerator ApplyCurrentAnimationToPlayerDelayed(Player player)
        {
            yield return null;
            var animation = ClientVisualState.CurrentAnimation;
            var time = ClientVisualState.CurrentAnimationTime;
            if (ClientVisualState.BoEAnimation)
            {
                if (BunchOfEmotesSupport.TryGetGameAnimationForCustomAnimationHash(animation, out var gameAnim))
                    animation = gameAnim;
                else
                    animation = ClientConstants.MissingAnimationHash;
            }
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
                if (clipInfo.Length > 0)
                {
                    time = time / clipInfo[0].clip.length;
                    player.PlayAnim(animation, true, true, time);
                }
            }
            finally
            {
                PlayerPatch.PlayAnimPatchEnabled = true;
            }
        }

        private bool CalculateChunkVisibility(List<StageChunk> chunks)
        {
            if (chunks == null || chunks.Count <= 0) return true;
            var inAChunk = false;
            foreach (var chunk in chunks)
            {
                foreach (var collider in chunk.colliders)
                {
                    if (StageChunk.PointInOBB(Player.transform.position, collider))
                    {
                        inAChunk = true;
                        if (chunk.isActive) return true;
                    }
                }
            }
            return !inAChunk;
        }

        public bool CalculateVisibility(List<Plane[]> frustumPlanes, List<StageChunk> stageChunks)
        {
            if (Player == null) return false;
            if (CalculateChunkVisibility(stageChunks))
            {
                foreach (var planes in frustumPlanes)
                {
                    if (GeometryUtility.TestPlanesAABB(planes, Player.characterVisual.mainRenderer.bounds)) return true;
                }
            }
            return false;
        }

        private void Hide()
        {
            Player.characterVisual.SetSpraycan(false);
            if (Player != null && Player.characterVisual != null)
                Player.characterVisual.gameObject.SetActive(false);
            if (NamePlate != null)
                NamePlate.gameObject.SetActive(false);
        }

        private void Unhide()
        {
            if (Player != null && Player.characterVisual != null)
            {
                if (!Player.characterVisual.gameObject.activeSelf)
                {
                    Player.characterVisual.SetSpraycan(false);
                    Player.StartCoroutine(ApplyCurrentAnimationToPlayerDelayed(Player));
                }
                Player.characterVisual.gameObject.SetActive(true);

                if (_targetLod != PlayerComponent.LOD)
                {
                    RefreshCharacterVisuals();
                    PlayerComponent.LOD = _targetLod;
                    if (_targetLod)
                        PlayerComponent.MakeLOD();
                }
            }
        }

        public static List<Action> OptimizationActions = new();

        private void ClearOptimizationActions()
        {
            OptimizationActions.Remove(Hide);
            OptimizationActions.Remove(Unhide);
        }

        private bool _targetLod = false;
        private bool _targetHidden = false;

        public void FrameUpdate(bool hidden, bool lod)
        {
            var mpSettings = MPSettings.Instance;
            if (_interactable != null)
            {
                var player = WorldHandler.instance.GetCurrentPlayer();
                var sequenceHandler = CustomSequenceHandler.instance;
                if (sequenceHandler.sequence == _interactable.Sequence && (player.sequenceState == SequenceState.IN_SEQUENCE || player.sequenceState == SequenceState.EXITING))
                    hidden = true;
            }

            if (ClientState == null || ClientVisualState == null) return;

            if (Player == null) return;

            if (hidden || lod)
                Player.RemoveGraffitiSlash();

            _targetLod = lod;
            _targetHidden = hidden;

#if DEBUG
            if (!Player.characterVisual.gameObject.activeSelf)
                RenderStats.PlayersCulled++;
            else
            {
                if (PlayerComponent.LOD)
                    RenderStats.PlayersRenderedLOD++;
                else
                    RenderStats.PlayersRendered++;
            }
#endif

            if (ClientVisualState.State == PlayerStates.Toilet)
                Teleporting = true;

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

            if (ClientVisualState.State == PlayerStates.Toilet)
                Teleporting = true;

            Player.motor._rigidbody.velocity = clientVisualStateVelocity;
            UpdateLookAt();
            UpdatePhone();
            UpdateSprayCan();
            if (NamePlate != null)
            {
                NamePlate.transform.position = Player.transform.position + (Vector3.up * 2f);
                if (ClientVisualState.State == PlayerStates.Toilet)
                    NamePlate.transform.position += Vector3.up * 2f;
            }
        }

        public void UpdateLobby()
        {
            var clientController = ClientController.Instance;
            _lobby = null;
            foreach (var lobby in clientController.ClientLobbyManager.Lobbies)
            {
                if (lobby.Value.LobbyState.Players.ContainsKey(ClientId))
                {
                    _lobby = lobby.Value;
                    break;
                }
            }
        }

        private void RefreshCharacterVisuals()
        {
            if (ClientId == ClientController.Instance.LocalID && !MPSettings.Instance.DebugLocalPlayer) return;
            Player.characterVisual.SetSpraycan(false);
            Player.RemoveGraffitiSlash();
            PlayerComponent.Chibi = ClientVisualState.Chibi;
            Player.character = Characters.NONE;
            var chara = (Characters)ClientState.Character;
            var fallbackChara = (Characters)ClientState.FallbackCharacter;

            if (chara >= Characters.MAX || chara <= Characters.NONE)
                chara = Characters.metalHead;

            if (fallbackChara >= Characters.MAX || fallbackChara <= Characters.NONE)
                fallbackChara = Characters.metalHead;

            var fit = ClientState.Outfit;
            var useStreamedCharacter = false;

            if (ClientState.CrewBoomCharacter != Guid.Empty)
            {
                chara = fallbackChara;
                if (CrewBoomSupport.Installed)
                {
                    chara = CrewBoomSupport.GetCharacterForGuid(ClientState.CrewBoomCharacter, Characters.NONE);
                    if (chara == Characters.NONE)
                    {
                        if (CrewBoomStreamer.HasCharacter(ClientState.CrewBoomCharacter))
                            useStreamedCharacter = true;
                        else
                        {
                            chara = fallbackChara;
                            fit = ClientState.FallbackOutfit;
                        }
                    }
                }
                else
                {
                    fit = ClientState.FallbackOutfit;
                }
            }

            if (fit < 0 || fit > 3)
                fit = 0;

            if (useStreamedCharacter)
            {
                PlayerComponent.SetStreamedCharacter(ClientState.CrewBoomCharacter, fit);
                Outfit = fit;
            }
            else
            {
                if (ClientState.SpecialSkin == SpecialSkins.None)
                {
                    Player.SetCharacter(chara, fit);
                    Player.InitVisual();
                    Outfit = fit;
                }
                SpecialSkinManager.Instance.ApplySpecialSkinToPlayer(Player, ClientState.SpecialSkin);
                SpecialSkinManager.Instance.ApplySpecialSkinVariantToPlayer(Player, ClientState.SpecialSkinVariant);
            }
            UpdateNameplate();
            Teleporting = true;
            if (!Player.anim.GetComponent<InverseKinematicsRelay>())
                Player.anim.gameObject.AddComponent<InverseKinematicsRelay>();

            if (ClientVisualState.CurrentAnimation != 0)
            {
                _timeSpentInWrongAnimation = 0f;
                Player.StartCoroutine(ApplyCurrentAnimationToPlayerDelayed(Player));
            }
            UpdateMoveStyleSkin();
            PlayerComponent.UpdateChibi();
        }

        public void UpdateClientStateVisuals()
        {
            if (ClientState == null || ClientVisualState == null) return;
            if (ClientId == ClientController.Instance.LocalID && !MPSettings.Instance.DebugLocalPlayer) return;

            var chara = (Characters)ClientState.Character;
            var fallbackChara = (Characters)ClientState.FallbackCharacter;

            if (chara >= Characters.MAX || chara <= Characters.NONE)
                chara = Characters.metalHead;

            if (fallbackChara >= Characters.MAX || fallbackChara <= Characters.NONE)
                fallbackChara = Characters.metalHead;

            var fit = ClientState.Outfit;
            var useStreamedCharacter = false;

            if (ClientState.CrewBoomCharacter != Guid.Empty)
            {
                chara = fallbackChara;
                if (CrewBoomSupport.Installed)
                {
                    chara = CrewBoomSupport.GetCharacterForGuid(ClientState.CrewBoomCharacter, Characters.NONE);
                    if (chara == Characters.NONE)
                    {
                        if (CrewBoomStreamer.HasCharacter(ClientState.CrewBoomCharacter))
                            useStreamedCharacter = true;
                        else
                        {
                            chara = fallbackChara;
                            fit = ClientState.FallbackOutfit;
                        }
                    }
                }
                else
                {
                    fit = ClientState.FallbackOutfit;
                }
            }

            if (fit < 0 || fit > 3)
                fit = 0;

            var justCreated = false;

            if (Player == null)
            {
                if (useStreamedCharacter)
                    Player = MPUtility.CreateMultiplayerPlayer(fallbackChara, ClientState.FallbackOutfit, this);
                else
                    Player = MPUtility.CreateMultiplayerPlayer(chara, fit, this);
                Outfit = fit;
                justCreated = true;
            }

            PlayerComponent.Chibi = ClientVisualState.Chibi;

            if (Player.character != chara && !useStreamedCharacter && ClientState.SpecialSkin == SpecialSkins.None)
            {
                Player.RemoveGraffitiSlash();
                Player.SetCharacter(chara, fit);
                Player.InitVisual();
                Outfit = fit;
                justCreated = true;
            }

            if (Outfit != fit && !useStreamedCharacter && ClientState.SpecialSkin == SpecialSkins.None)
            {
                Player.SetOutfit(fit);
                Outfit = fit;
            }

            if (useStreamedCharacter)
            {
                if (PlayerComponent.StreamedCharacter == null || PlayerComponent.StreamedCharacter.Handle.GUID != ClientState.CrewBoomCharacter)
                {
                    PlayerComponent.SetStreamedCharacter(ClientState.CrewBoomCharacter, fit);
                }
                if (PlayerComponent.StreamedCharacter != null && PlayerComponent.StreamedCharacter.Outfit != fit)
                {
                    PlayerComponent.StreamedCharacter.ApplyOutfit(fit);
                }
                PlayerComponent.StreamedOutfit = fit;
                Outfit = fit;
            }

            if (ClientState.SpecialSkin != PlayerComponent.SpecialSkin)
            {
                SpecialSkinManager.Instance.ApplySpecialSkinToPlayer(Player, ClientState.SpecialSkin);
                justCreated = true;
            }
            if (ClientState.SpecialSkinVariant != PlayerComponent.SpecialSkinVariant)
            {
                SpecialSkinManager.Instance.ApplySpecialSkinVariantToPlayer(Player, ClientState.SpecialSkinVariant);
            }

            if (justCreated)
            {
                UpdateNameplate();
                Teleporting = true;
                if (!Player.anim.GetComponent<InverseKinematicsRelay>())
                    Player.anim.gameObject.AddComponent<InverseKinematicsRelay>();

                if (ClientVisualState.CurrentAnimation != 0)
                {
                    _timeSpentInWrongAnimation = 0f;
                    Player.StartCoroutine(ApplyCurrentAnimationToPlayerDelayed(Player));
                }
                UpdateMoveStyleSkin();
            }
        }

        public void OnSkinLoaded()
        {
            if (ClientVisualState.CurrentAnimation != 0)
            {
                _timeSpentInWrongAnimation = 0f;
                Player.StartCoroutine(ApplyCurrentAnimationToPlayerDelayed(Player));
            }
        }

        public void TickUpdate()
        {
            var clientController = ClientController.Instance;
            var mpSettings = MPSettings.Instance;

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

            if (_targetHidden)
            {
                if (mpSettings.OptimizeOnePlayerAtATime)
                {
                    if (Player.characterVisual.gameObject.activeSelf)
                    {
                        ClearOptimizationActions();
                        OptimizationActions.Add(Hide);
                    }
                }
                else
                    Hide();
                Player.characterVisual.SetSpraycan(false);
            }
            else
            {
                if (mpSettings.OptimizeOnePlayerAtATime)
                {
                    if (!Player.characterVisual.gameObject.activeSelf || _targetLod != PlayerComponent.LOD)
                    {
                        ClearOptimizationActions();
                        OptimizationActions.Add(Unhide);
                    }
                }
                else
                    Unhide();

                if (NamePlate != null && mpSettings.ShowNamePlates)
                    NamePlate.gameObject.SetActive(true);
                else if (NamePlate != null && !mpSettings.ShowNamePlates)
                    NamePlate.gameObject.SetActive(false);
            }

            var snapAnim = false;

            if (ClientVisualState.Chibi != PlayerComponent.Chibi)
            {
                UpdateClientStateVisuals();
                RefreshCharacterVisuals();
            }

            PlayerComponent.AFK = ClientVisualState.AFK;
            PlayerComponent.Chibi = ClientVisualState.Chibi;

            if (ClientVisualState.State == PlayerStates.Toilet)
            {
                Player.characterVisual.feetIK = false;
            }

            var targetAnim = ClientVisualState.CurrentAnimation;
            if (ClientVisualState.BoEAnimation)
            {
                if (BunchOfEmotesSupport.TryGetGameAnimationForCustomAnimationHash(targetAnim, out var gameAnim))
                    targetAnim = gameAnim;
                else
                    targetAnim = ClientConstants.MissingAnimationHash;
            }    
            if (Player.curAnim != targetAnim)
            {
                if (Player.curAnim != _lastAnimation)
                    _timeSpentInWrongAnimation = 0f;
                _timeSpentInWrongAnimation += ClientController.DeltaTime;
            }
            else
                _timeSpentInWrongAnimation = 0f;
            _lastAnimation = Player.curAnim;

            if (_timeSpentInWrongAnimation >= MaxTimeSpentInWrongAnimation)
                snapAnim = true;

            if (snapAnim)
            {
                if (ClientVisualState.CurrentAnimation != 0)
                {
                    _timeSpentInWrongAnimation = 0f;
                    Player.StartCoroutine(ApplyCurrentAnimationToPlayerDelayed(Player));
                }
            }

            if (Player.moveStyleEquipped != (MoveStyle)ClientVisualState.MoveStyle || Player.usingEquippedMovestyle != ClientVisualState.MoveStyleEquipped)
            {
                Player.StartCoroutine(UpdateMoveStyleDelayed());
            }

            var inTeamLobby = InOurTeamLobby(out var rival);

            TickUpdateNameplate();
            if (_mapPin == null)
                MakeMapPin();
            _mapPin.SetLocation();

            if (rival && inTeamLobby)
                _mapPinMaterial.color = new Color(0.9f, 0.9f, 0f);
            else if (InSameLobbyAsLocalPlayer())
                _mapPinMaterial.color = new Color(0f, 0.9f, 0f);
            else 
                _mapPinMaterial.color = new Color(0.9f, 0.9f, 0.9f);

            if (_interactable == null)
            {
                _interactable = PlayerInteractable.Create(this);
            }

            if (_lobby == null)
                _interactable.AlreadyTalked = false;

            if (IsChallengeable())
            {
                _mapPinMaterial.color = new Color(0.0f, 0.8f, 0.9f);
                _mapPinParticles.SetActive(true);
                _interactable.gameObject.SetActive(true);
                PlayerComponent.ChallengeIndicatorVisible = true;
            }
            else
            {
                _mapPinParticles.SetActive(false);
                _interactable.gameObject.SetActive(false);
                PlayerComponent.ChallengeIndicatorVisible = false;
            }

            if (Player.characterVisual.boostpackEffectMode != (BoostpackEffectMode)ClientVisualState.BoostpackEffectMode)
                Player.characterVisual.SetBoostpackEffect((BoostpackEffectMode)ClientVisualState.BoostpackEffectMode);

            if (Player.characterVisual.frictionEffectMode != (FrictionEffectMode)ClientVisualState.FrictionEffectMode)
                Player.characterVisual.SetFrictionEffect((FrictionEffectMode)ClientVisualState.FrictionEffectMode);

            Player.SetDustEmission(ClientVisualState.DustEmissionRate);

            if (ClientVisualState.State != PlayerStates.Graffiti)
                Player.RemoveGraffitiSlash();

            if (ClientVisualState.MoveStyleSkin != _lastMoveStyleSkin || ClientVisualState.MPMoveStyleSkin != _lastMPMoveStyleSkin)
            {
                UpdateMoveStyleSkin();
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

            Player.characterVisual.VFX.boostpackTrail.SetActive(ClientVisualState.BoostpackTrail);

            _previousState = ClientVisualState.State;
        }

        private void UpdateMoveStyleSkin()
        {
            if (ClientVisualState == null) return;
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
                PlayerComponent.ApplyMoveStyleSkin(ClientVisualState.MoveStyleSkin);
        }

        private void UpdateMoveStyle()
        {
            PlayerPatch.PlayAnimPatchEnabled = false;
            try
            {
                Player.SetCurrentMoveStyleEquipped((MoveStyle)ClientVisualState.MoveStyle);
                Player.SwitchToEquippedMovestyle(ClientVisualState.MoveStyleEquipped, false, true, false);
            }
            finally
            {
                PlayerPatch.PlayAnimPatchEnabled = true;
            }
        }

        private IEnumerator UpdateMoveStyleDelayed()
        {
            yield return null;
            UpdateMoveStyle();   
        }

        public void SetClientState(ClientState newClientState)
        {
            ClientState = newClientState;
        }

        public void UpdateNameplate()
        {
            var clientController = ClientController.Instance;
            var inOurTeamLobby = InOurTeamLobby(out var rival);

            CreateNameplateIfNecessary();

            var name = MPUtility.GetPlayerDisplayName(ClientState);

            if (inOurTeamLobby)
            {
                name = MPUtility.GetPlayerDisplayNameWithoutTags(ClientState);
                if (rival)
                {
                    NamePlate.Label.color = Color.red;
                }
                else
                {
                    NamePlate.Label.color = Color.green;
                }
            }
            else
                NamePlate.Label.color = Color.white;

            if (NamePlate.Label.text != name)
                NamePlate.Label.text = name;
        }

        private void CreateNameplateIfNecessary()
        {
            if (NamePlate == null)
            {
                NamePlate = Nameplate.Create();
            }
        }

        private void TickUpdateNameplate()
        {
            var settings = MPSettings.Instance;

            CreateNameplateIfNecessary();
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
            ClearOptimizationActions();
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
