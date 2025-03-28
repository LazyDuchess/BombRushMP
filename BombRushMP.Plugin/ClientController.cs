using BombRushMP.BunchOfEmotes;
using BombRushMP.Common;
using BombRushMP.Common.Networking;
using BombRushMP.Common.Packets;
using BombRushMP.CrewBoom;
using BombRushMP.Plugin.OfflineInterface;
using CommonAPI;
using Reptile;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BombRushMP.Plugin
{
    public class ClientController : MonoBehaviour
    {
        public static float DeltaTime { get; private set; }
        public static ClientController Instance { get; private set; }
        public static bool SeenMOTD = false;
        public Dictionary<int, int> PlayerCountByStage = new();
        public ClientLobbyManager ClientLobbyManager = null;
        public Dictionary<Player, MPPlayer> MultiplayerPlayerByPlayer = new();
        public Dictionary<ushort, MPPlayer> Players = new();
        /// <summary>
        /// Whether the client is connected to the server and performed the initial handshake.
        /// </summary>
        public bool Connected => _client != null && _client.IsConnected && _handShook;
        public ushort LocalID = 0;
        public float TickRate = Constants.DefaultNetworkingTickRate;
        public string Address = "";
        public int Port = 0;
        public GraffitiGame CurrentGraffitiGame = null;
        public static Action ServerDisconnect;
        public static Action ServerConnect;
        public static Action<ushort> PlayerDisconnected;
        public static Action<Packets, Packet> PacketReceived;
        public static Action ClientStatesUpdate;
        public string AuthKey;
        public bool InfrequentClientStateUpdateQueued = false;
        public ServerState ServerState = new();
        public PlayerComponent LocalPlayerComponent
        {
            get
            {
                if (_cachedPlayerComponent == null)
                    _cachedPlayerComponent = PlayerComponent.Get(WorldHandler.instance.GetCurrentPlayer());
                return _cachedPlayerComponent;
            }
        }
        private PlayerComponent _cachedPlayerComponent = null;
        private INetClient _client;
        private float _tickTimer = 0f;
        private float _infrequentUpdateTimer = 0f;
        private bool _handShook = false;
        private MPSettings _mpSettings = null;
        private INetworkingInterface NetworkingInterface => NetworkingEnvironment.NetworkingInterface;

        private void Awake()
        {
            Instance = this;
            _mpSettings = MPSettings.Instance;
            ClientLobbyManager = new();
            ClientLobbyManager.LobbiesUpdated += OnLobbiesUpdated;
            MPPlayer.OptimizationActions.Clear();
        }

        public int GetPlayerCountForStage(Stage stage)
        {
            if (PlayerCountByStage.TryGetValue((int)stage, out var playerCount))
                return playerCount;
            return 0;
        }

        public AuthUser GetUser(ushort playerId)
        {
            if (Players.TryGetValue(playerId, out var player))
            {
                if (player.ClientState != null)
                    return player.ClientState.User;
            }
            return null;
        }

        public AuthUser GetLocalUser()
        {
            return GetUser(LocalID);
        }

        public void Connect()
        {
            ClientLogger.Log($"Connecting to {Address}:{Port}");
            _client = NetworkingInterface.CreateClient();
            Task.Run(() => { _client.Connect(Address, Port); });
            _client.Connected += OnConnected;
            _client.Disconnected += OnDisconnect;
            _client.ConnectionFailed += OnConnectionFailed;
            _client.MessageReceived += OnMessageReceived;
            ServerConnect?.Invoke();
        }

        public void Disconnect()
        {
            foreach (var player in Players)
            {
                player.Value.Delete();
            }
            Players.Clear();
            _handShook = false;
            LocalID = 0;
            if (_client != null)
            {
                _client.Connected -= OnConnected;
                _client.Disconnected -= OnDisconnect;
                _client.ConnectionFailed -= OnConnectionFailed;
                _client.MessageReceived -= OnMessageReceived;
                _client.Disconnect();
            }
            _client = null;
            ServerDisconnect?.Invoke();
        }
        
        public void SendGenericEvent(GenericEvents ev, IMessage.SendModes sendMode)
        {
            SendPacket(new PlayerGenericEvent(ev), sendMode, NetChannels.Default);
        }

        public void SendPacket(Packet packet, IMessage.SendModes sendMode, NetChannels channel)
        {
            var message = PacketFactory.MessageFromPacket(packet, sendMode, channel);
            _client.Send(message);
        }

        public static ClientController Create(string address, int port)
        {
            var clientControllerGO = new GameObject("Client Controller");
            var clientController = clientControllerGO.AddComponent<ClientController>();
            clientController.Address = address;
            clientController.Port = port;
            clientController.Connect();
            return clientController;
        }

        private ClientVisualState CreateVisualStatePacket(Player player)
        {
            var state = PlayerStates.Normal;
            if (CurrentGraffitiGame != null)
                state = PlayerStates.Graffiti;

            var sequenceObject = MPUtility.GetCurrentToilet();

            if (sequenceObject != null)
                state = PlayerStates.Toilet;

            if (player.IsDead())
                state = PlayerStates.Dead;

            var playerComp = LocalPlayerComponent;

            var packet = new ClientVisualState();
            packet.State = state;
            packet.AFK = playerComp.AFK;
            packet.MoveStyleSkin = (byte)Core.Instance.SaveManager.CurrentSaveSlot.GetCharacterProgress(player.character).moveStyleSkin;
            packet.MoveStyle = (int)player.moveStyleEquipped;
            packet.Chibi = playerComp.Chibi;
            var charData = MPSaveData.Instance.GetCharacterData(player.character);
            if (charData != null)
                packet.MPMoveStyleSkin = charData.MPMoveStyleSkin;
            switch (state) {
                case PlayerStates.Dead:
                case PlayerStates.Normal:
                    {
                        packet.MoveStyleEquipped = player.usingEquippedMovestyle;
                        packet.Position = player.gameObject.transform.position.ToSystemVector3();
                        packet.VisualPosition = player.visualTf.localPosition.ToSystemVector3();
                        packet.Rotation = player.gameObject.transform.rotation.ToSystemQuaternion();
                        packet.VisualRotation = player.visualTf.localRotation.ToSystemQuaternion();
                        packet.Velocity = player.motor._rigidbody.velocity.ToSystemVector3();
                        packet.GrindDirection = player.anim.GetFloat(ClientConstants.GrindDirectionHash);
                        packet.SprayCanHeld = player.spraycanState == Player.SpraycanState.START || player.spraycanState == Player.SpraycanState.SHAKE;
                        packet.PhoneHeld = player.characterVisual.phoneActive;
                        packet.PhoneDirectionX = player.anim.GetFloat(ClientConstants.PhoneDirectionXHash);
                        packet.PhoneDirectionY = player.anim.GetFloat(ClientConstants.PhoneDirectionYHash);
                        packet.TurnDirection1 = player.anim.GetFloat(ClientConstants.TurnDirection1Hash);
                        packet.TurnDirection2 = player.anim.GetFloat(ClientConstants.TurnDirection2Hash);
                        packet.TurnDirection3 = player.anim.GetFloat(ClientConstants.TurnDirection3Hash);
                        packet.TurnDirectionSkateboard = player.anim.GetFloat(ClientConstants.TurnDirectionSkateboardHash);
                        packet.BoostpackEffectMode = (byte)player.characterVisual.boostpackEffectMode;
                        packet.FrictionEffectMode = (byte)player.characterVisual.frictionEffectMode;
                        if (player.characterVisual.dustParticles != null)
                            packet.DustEmissionRate = (byte)player.characterVisual.dustParticles.emission.rateOverTime.constant;
                        packet.CurrentAnimation = player.curAnim;
                        packet.CurrentAnimationTime = player.curAnimActiveTime;
                        packet.Hitbox = player.hitbox.activeSelf;
                        packet.HitboxLeftLeg = player.hitboxLeftLeg.activeSelf;
                        packet.HitboxRightLeg = player.hitboxRightLeg.activeSelf;
                        packet.HitboxUpperBody = player.hitboxUpperBody.activeSelf;
                        packet.HitboxAerial = player.airialHitbox.activeSelf;
                        packet.HitboxRadial = player.radialHitbox.activeSelf;
                        packet.HitboxSpray = player.sprayHitbox.activeSelf;

                        if (BunchOfEmotesSupport.TryGetCustomAnimationHashByGameAnimation(packet.CurrentAnimation, out var hash))
                        {
                            packet.BoEAnimation = true;
                            packet.CurrentAnimation = hash;
                        }
                    }
                    break;

                case PlayerStates.Graffiti:
                    {
                        var grafRotation = Quaternion.LookRotation(-CurrentGraffitiGame.gSpot.transform.forward, Vector3.up);
                        packet.Position = (CurrentGraffitiGame.gSpot.transform.position + (CurrentGraffitiGame.gSpot.transform.forward * ClientConstants.PlayerGraffitiDistance) + (-CurrentGraffitiGame.gSpot.transform.up * ClientConstants.PlayerGraffitiDownDistance)).ToSystemVector3();
                        packet.Rotation = grafRotation.ToSystemQuaternion();
                        packet.BoostpackEffectMode = (byte)CurrentGraffitiGame.characterPuppet.boostpackEffectMode;
                    }
                    break;

                case PlayerStates.Toilet:
                    {
                        packet.Position = (sequenceObject.transform.position + (Vector3.up * 0.2f) - (sequenceObject.transform.forward * 0.2f)).ToSystemVector3();
                        packet.Rotation = (sequenceObject.transform.rotation * Quaternion.Euler(0f, 180f, 0f)).ToSystemQuaternion();
                        packet.CurrentAnimation = Animator.StringToHash("idle");
                    }
                    break;
            }
            return packet;
        }

        private void SendVisualState()
        {
            var player = WorldHandler.instance.GetCurrentPlayer();
            if (player == null) return;
            var packet = CreateVisualStatePacket(player);
            SendPacket(packet, IMessage.SendModes.Unreliable, NetChannels.VisualUpdates);
        }

        private void Tick()
        {
#if DEBUG
            if (MPSettings.Instance.UpdateNetworkClient)
#endif
            _client?.Update();
            if (Connected)
            {
                SendVisualState();
            }
#if DEBUG
            if (MPSettings.Instance.UpdatePlayers)
            {
#endif
                foreach (var player in Players)
                {
                    player.Value.TickUpdate();
                }
#if DEBUG
            }
#endif
            ClientLobbyManager.OnTick();
        }

        private void OnLobbiesUpdated()
        {
            foreach (var player in Players)
            {
                player.Value.UpdateLobby();
                player.Value.UpdateNameplate();
            }
        }

        private List<Plane[]> _frustumList = new();

        private void Update()
        {
            var mpSettings = MPSettings.Instance;
#if DEBUG
            if (!mpSettings.UpdateClientController) return;
#endif
            var playersHidden = MPUtility.GetCurrentToilet() != null;
            _tickTimer += Time.deltaTime;
            _infrequentUpdateTimer += Time.deltaTime;
            if (_tickTimer >= TickRate)
            {
                DeltaTime = _tickTimer;
                Tick();
                _tickTimer = 0f;
            }
            if (_infrequentUpdateTimer >= ClientConstants.InfrequentUpdateRate)
            {
                InfrequentUpdate();
                _infrequentUpdateTimer = 0f;
            }
#if DEBUG
            RenderStats.Reset();
            RenderStats.Players = Players.Count;
            if (mpSettings.UpdatePlayers)
            {
#endif
                _frustumList.Clear();
                var worldHandler = WorldHandler.instance;
                var hideOutOfView = mpSettings.HidePlayersOutOfView;
                var hideDist = mpSettings.PlayerDrawDistance;
                if (hideOutOfView)
                {
                    var mainCam = worldHandler.CurrentCamera;
                    _frustumList.Add(GeometryUtility.CalculateFrustumPlanes(mainCam));
                    var phoneCameras = PlayerPhoneCameras.Instance;
                    if (phoneCameras != null && phoneCameras.isActiveAndEnabled)
                    {
                        foreach(var cam in phoneCameras.Cameras)
                        {
                            if (cam.Value.enabled)
                                _frustumList.Add(GeometryUtility.CalculateFrustumPlanes(cam.Value));
                        }
                    }
                }
 
                var localCamPos = worldHandler.currentCamera.transform.position;
                List<StageChunk> stageChunkList = null;
                if (mpSettings.HidePlayersInInactiveChunks)
                    stageChunkList = worldHandler.SceneObjectsRegister.stageChunks;
                foreach (var player in Players)
                {
                    var hidden = playersHidden;
                    if (hideOutOfView && !playersHidden)
                    {
                        if (player.Value.Player != null && (player.Value.Player.transform.position - localCamPos).sqrMagnitude >= mpSettings.PlayerDrawDistance)
                        {
                            hidden = true;
                        }
                        else
                        {
                            if (!player.Value.CalculateVisibility(_frustumList, stageChunkList))
                                hidden = true;
                        }
                    }
                    var lod = false;
                    if (!hidden && player.Value.Player != null && mpSettings.PlayerLodEnabled && (player.Value.Player.transform.position - localCamPos).sqrMagnitude >= mpSettings.PlayerLodDistance)
                        lod = true;
                    player.Value.FrameUpdate(hidden, lod);
                }
                if (MPPlayer.OptimizationActions.Count > 0)
                {
                    var act = MPPlayer.OptimizationActions[0];
                    MPPlayer.OptimizationActions.RemoveAt(0);
                    act?.Invoke();
                }

#if DEBUG
            }
#endif
            ClientLobbyManager.OnUpdate();
        }

        private void InfrequentUpdate()
        {
            if (InfrequentClientStateUpdateQueued)
                SendClientState();
        }

        private void OnClientDisconnected(object sender, ushort id)
        {
            if (Players.TryGetValue(id, out var player))
            {
                player.Delete();
                Players.Remove(id);
            }
            PlayerDisconnected?.Invoke(id);
        }

        private void ExecuteGenericEvent(GenericEvents ev, MPPlayer player)
        {
            switch (ev)
            {
                case GenericEvents.Spray:
                    if (player.Player == null) break;
                    player.Player.characterVisual.SetSpraycan(true, player.Player.character);
                    player.Player.SetSpraycanState(Player.SpraycanState.SPRAY);
                    break;

                case GenericEvents.Teleport:
                    player.Teleporting = true;
                    break;

                case GenericEvents.GraffitiGameOver:
                    if (player.Player == null) break;
                    player.Player.RemoveGraffitiSlash();
                    break;

                case GenericEvents.Death:
                    if (player.ClientState == null) break;
                    if (!MPSettings.Instance.DeathMessages) break;
                    ChatUI.Instance.AddMessage(string.Format(ClientConstants.DeathMessage, MPUtility.GetPlayerDisplayName(player.ClientState)));
                    break;

                case GenericEvents.Land:
                    if (player.Player == null) break;
                    player.Player.AudioManager.PlaySfxGameplay(player.Player.moveStyle, AudioClipID.land, player.Player.playerOneShotAudioSource);
                    player.Player.CreateCircleDustEffect(-player.Player.transform.up);
                    break;
            }
        }

        private void OnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            var packetId = (Packets)e.MessageId;
            var packet = PacketFactory.PacketFromMessage(packetId, e.Message);
            if (packet == null) return;
            PacketReceived?.Invoke(packetId, packet);
            switch (packetId)
            {
                case Packets.ServerServerStateUpdate:
                    {
                        var statePacket = packet as ServerServerStateUpdate;
                        ServerState = statePacket.State;
                    }
                    break;
                case Packets.ServerSetSpecialSkin:
                    {
                        var skinPacket = packet as ServerSetSpecialSkin;
                        var player = WorldHandler.instance.GetCurrentPlayer();
                        if (skinPacket.SpecialSkin == SpecialSkins.None)
                        {
                            SpecialSkinManager.Instance.RemoveSpecialSkinFromPlayer(player);
                        }
                        else
                        {
                            SpecialSkinManager.Instance.ApplySpecialSkinToPlayer(player, skinPacket.SpecialSkin);
                            SpecialSkinManager.Instance.ApplyRandomVariantToPlayer(player);
                        }
                    }
                    break;
                case Packets.ServerSetChibi:
                    {
                        LocalPlayerComponent.Chibi = (packet as ServerSetChibi).Set;
                    }
                    break;

                case Packets.ClientHitByPlayer:
                    {
                        var hitPacket = packet as ClientHitByPlayer;
                        if (hitPacket.ClientId == LocalID) break;
                        if (hitPacket.Attacker != LocalID) break;
                        MPSaveData.Instance.Stats.PlayersHit++;
                        Core.Instance.SaveManager.SaveCurrentSaveSlot();
                    }
                    break;

                case Packets.ServerConnectionResponse:
                    {
                        var connectionResponse = (ServerConnectionResponse)packet;
                        LocalID = connectionResponse.LocalClientId;
                        TickRate = connectionResponse.TickRate;
                        ServerState = connectionResponse.ServerState;
                        PlayerAnimation.ClientSendMode = connectionResponse.ClientAnimationSendMode;
                        _handShook = true;
                        ClientLogger.Log($"Received server handshake - our local ID is {connectionResponse.LocalClientId}, our UserKind is {connectionResponse.User.UserKind}.");
                        if (connectionResponse.User.HasTag(SpecialPlayerUtils.SpecialPlayerTag))
                        {
                            var player = WorldHandler.instance.GetCurrentPlayer();
                            player.SetCurrentMoveStyleEquipped(MoveStyle.SKATEBOARD, true, true);
                            var saveManager = Core.Instance.SaveManager;
                            var skin = MPUnlockManager.Instance.UnlockByID[Animator.StringToHash(SpecialPlayerUtils.SpecialPlayerUnlockID)] as MPSkateboardSkin;
                            saveManager.CurrentSaveSlot.GetCharacterProgress(player.character).moveStyleSkin = 0;
                            saveManager.CurrentSaveSlot.GetCharacterProgress(player.character).moveStyle = MoveStyle.SKATEBOARD;
                            MPSaveData.Instance.GetCharacterData(player.character).MPMoveStyleSkin = skin.Identifier;
                            skin.ApplyToPlayer(player);
                            saveManager.SaveCurrentSaveSlot();
                            SpecialSkinManager.Instance.ApplySpecialSkinToPlayer(player, SpecialSkins.SpecialPlayer);
                        }
                        if (!SeenMOTD)
                        {
                            SeenMOTD = true;
                            if (!string.IsNullOrEmpty(connectionResponse.MOTD))
                            {
                                ChatUI.Instance.AddMessage(connectionResponse.MOTD);
                            }
                        }
                    }
                    break;

                case Packets.ServerClientStates:
                    {
                        var clientStates = (ServerClientStates)packet;
                        foreach (var clientState in clientStates.ClientStates)
                        {
                            if (!Players.TryGetValue(clientState.Key, out var player))
                            {
                                player = new MPPlayer();
                                Players[clientState.Key] = player;
                            }
                            player.SetClientState(clientState.Value);
                            player.ClientId = clientState.Key;
                            player.UpdateClientStateVisuals();
                            player.UpdateNameplate();
                        }
                        if (clientStates.Full)
                        {
                            var playerValues = new List<MPPlayer>(Players.Values);
                            foreach(var player in playerValues)
                            {
                                if (!clientStates.ClientStates.ContainsKey(player.ClientId))
                                {
                                    OnClientDisconnected(this, player.ClientId);
                                }
                            }
                        }
                        ClientStatesUpdate?.Invoke();
                    }
                    break;

                case Packets.ServerClientVisualStates:
                    {
                        var clientVisualStates = (ServerClientVisualStates)packet;
                        foreach (var clientVisualState in clientVisualStates.ClientVisualStates)
                        {
                            if (!Players.TryGetValue(clientVisualState.Key, out var player)) continue;
                            var oldVisualState = player.ClientVisualState;
                            player.UpdateVisualState(clientVisualState.Value);
                            if (oldVisualState == null)
                            {
                                player.UpdateClientStateVisuals();
                            }
                        }
                    }
                    break;

                case Packets.PlayerAnimation:
                    {
                        var playerPacket = (PlayerAnimation)packet;
                        if (Players.TryGetValue(playerPacket.ClientId, out var player))
                        {
                            if (player.Player != null)
                            {
                                var newAnim = playerPacket.NewAnim;
                                if (playerPacket.BoE)
                                {
                                    if (BunchOfEmotesSupport.TryGetGameAnimationForCustomAnimationHash(newAnim, out var gameAnim))
                                        newAnim = gameAnim;
                                    else
                                        newAnim = ClientConstants.MissingAnimationHash;
                                }
                                MPUtility.PlayAnimationOnMultiplayerPlayer(player.Player, newAnim, playerPacket.ForceOverwrite, playerPacket.Instant, playerPacket.AtTime);
                            }
                        }
                    }
                    break;

                case Packets.PlayerVoice:
                    {
                        if (MPSettings.Instance.PlayerAudioEnabled)
                        {
                            var playerPacket = (PlayerVoice)packet;
                            if (Players.TryGetValue(playerPacket.ClientId, out var player))
                            {
                                if (player.Player != null)
                                    player.Player.PlayVoice((AudioClipID)playerPacket.AudioClipId, (VoicePriority)playerPacket.VoicePriority, true);
                            }
                        }
                    }
                    break;

                case Packets.PlayerGraffitiSlash:
                    {
                        var playerPacket = (PlayerGraffitiSlash)packet;
                        if (Players.TryGetValue(playerPacket.ClientId, out var player))
                        {
                            if (player.Player != null)
                            {
                                player.Player.RemoveGraffitiSlash();
                                player.Player.CreateGraffitiSlashEffect(player.Player.transform, playerPacket.Direction.ToUnityVector3());
                                player.Player.AudioManager.PlaySfxGameplay(SfxCollectionID.GraffitiSfx, AudioClipID.graffitiSlash, player.Player.playerOneShotAudioSource, 0f);
                            }
                        }
                    }
                    break;

                case Packets.PlayerGraffitiFinisher:
                    {
                        var playerPacket = (PlayerGraffitiFinisher)packet;
                        if (Players.TryGetValue(playerPacket.ClientId, out var player))
                        {
                            if (player.Player != null)
                            {
                                player.Player.RemoveGraffitiSlash();
                                player.Player.CreateGraffitiFinishEffect(player.Player.transform, (GraffitiSize)playerPacket.GraffitiSize);
                                player.Player.AudioManager.PlaySfxGameplay(SfxCollectionID.GraffitiSfx, AudioClipID.graffitiComplete, player.Player.playerOneShotAudioSource, 0f);
                            }
                        }
                    }
                    break;

                case Packets.PlayerGenericEvent:
                    {
                        var evPacket = (PlayerGenericEvent)packet;
                        if (Players.TryGetValue(evPacket.ClientId, out var player))
                        {
                            ExecuteGenericEvent(evPacket.Event, player);
                        }
                    }
                    break;

                case Packets.ServerPlayerCount:
                    {
                        var playerCountPacket = (ServerPlayerCount)packet;
                        PlayerCountByStage = playerCountPacket.PlayerCountByStage;
                    }
                    break;

                case Packets.ServerBanList:
                    {
                        var user = GetLocalUser();
                        if (!user.IsModerator) break;
                        var banListPacket = (ServerBanList)packet;
                        var fileDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ACN");
                        var filePath = Path.Combine(fileDirectory, "banned_users.json");
                        var chatUi = ChatUI.Instance;
                        ClientLogger.Log($"Received ban list from server. Will write to {filePath}.");
                        try
                        {
                            Directory.CreateDirectory(fileDirectory);
                            File.WriteAllText(filePath, banListPacket.JsonData);
                            chatUi.AddMessage($"Ban list written to {filePath}");
                        }
                        catch(Exception ex)
                        {
                            chatUi.AddMessage("Failed to write ban list.");
                            Debug.LogError(ex);
                        }
                    }
                    break;

                case Packets.ServerClientDisconnected:
                    {
                        var discPacket = (ServerClientDisconnected)packet;
                        if (Players.ContainsKey(discPacket.ClientId))
                        {
                            OnClientDisconnected(this, discPacket.ClientId);
                            ClientStatesUpdate?.Invoke();
                        }
                    }
                    break;
            }
        }

        private IEnumerator AttemptReconnect()
        {
            yield return new WaitForSecondsRealtime(1f);
            Disconnect();
            Connect();
        }

        private void OnConnectionFailed(object sender, ConnectionFailedEventArgs e)
        {
            ClientLogger.Log($"Failed to connect to server. Reason: {e.Reason}");
            ClientLogger.Log("Will attempt to re-connect");
            StopAllCoroutines();
            StartCoroutine(AttemptReconnect());
        }

        private void OnDisconnect(object sender, DisconnectedEventArgs e)
        {
            ClientLogger.Log($"Disconnected! Reason: {e.Reason}");
            ClientLogger.Log("Will attempt to re-connect");
            StopAllCoroutines();
            StartCoroutine(AttemptReconnect());
        }

        public void SendAuth()
        {
            var authKey = AuthKey;
            var clientState = CreateClientState();
            var authPacket = new ClientAuth(authKey, clientState);
            authPacket.Invisible = MPSettings.Instance.Invisible;
            SendPacket(authPacket, IMessage.SendModes.Reliable, NetChannels.Default);
        }

        public ClientState CreateClientState()
        {
            var player = WorldHandler.instance.GetCurrentPlayer();
            var playerComp = LocalPlayerComponent;
            var statePacket = new ClientState()
            {
                Name = _mpSettings.PlayerName,
                CrewName = _mpSettings.CrewName,
                Character = (sbyte)player.character,
                FallbackCharacter = (sbyte)MPSettings.Instance.FallbackCharacter,
                FallbackOutfit = (byte)MPSettings.Instance.FallbackOutfit,
                Outfit = (byte)Core.Instance.SaveManager.CurrentSaveSlot.GetCharacterProgress(player.character).outfit,
                Stage = (int)Utility.SceneNameToStage(SceneManager.GetActiveScene().name),
                SpecialSkin = playerComp.SpecialSkin,
                SpecialSkinVariant = playerComp.SpecialSkinVariant,
            };

            if (CrewBoomSupport.Installed)
            {
                statePacket.CrewBoomCharacter = CrewBoomSupport.GetGuidForCharacter(player.character);
                if (playerComp.StreamedCharacterHandle != null)
                {
                    statePacket.CrewBoomCharacter = playerComp.StreamedCharacterHandle.GUID;
                    statePacket.Outfit = (byte)playerComp.StreamedOutfit;
                }
            }
            return statePacket;
        }

        public void SendClientState()
        {
            SendPacket(CreateClientState(), IMessage.SendModes.Reliable, NetChannels.ClientAndLobbyUpdates);
            InfrequentClientStateUpdateQueued = false;
        }

        private void OnConnected(object sender, EventArgs e)
        {
            ClientLogger.Log("Connected! Sending Auth request...");
            SendAuth();
        }

        private void OnDestroy()
        {
            Disconnect();
            ClientLobbyManager.LobbiesUpdated -= OnLobbiesUpdated;
            ClientLobbyManager?.Dispose();
        }
    }
}
