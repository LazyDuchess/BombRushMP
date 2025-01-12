using BombRushMP.Common;
using BombRushMP.Common.Networking;
using BombRushMP.Common.Packets;
using BombRushMP.CrewBoom;
using BombRushMP.Plugin.OfflineInterface;
using Reptile;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BombRushMP.Plugin
{
    public class ClientController : MonoBehaviour
    {
        public static float DeltaTime { get; private set; }
        public static ClientController Instance { get; private set; }
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
        public string AuthKey;
        private INetClient _client;
        private float _tickTimer = 0f;
        private bool _handShook = false;
        private MPSettings _mpSettings = null;
        private INetworkingInterface NetworkingInterface => NetworkingEnvironment.NetworkingInterface;

        private void Awake()
        {
            Instance = this;
            _mpSettings = MPSettings.Instance;
            ClientLobbyManager = new();
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
            var state = PlayerStates.None;
            if (CurrentGraffitiGame != null)
                state = PlayerStates.Graffiti;
            var packet = new ClientVisualState();
            packet.State = state;
            packet.AFK = PlayerComponent.Get(player).AFK;
            packet.MoveStyleSkin = (byte)Core.Instance.SaveManager.CurrentSaveSlot.GetCharacterProgress(player.character).moveStyleSkin;
            var charData = MPSaveData.Instance.GetCharacterData(player.character);
            if (charData != null)
                packet.MPMoveStyleSkin = charData.MPMoveStyleSkin;
            if (state == PlayerStates.None)
            {
                packet.MoveStyleEquipped = player.usingEquippedMovestyle;
                packet.MoveStyle = (int)player.moveStyleEquipped;
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
            }
            else if (state == PlayerStates.Graffiti){
                var grafRotation = Quaternion.LookRotation(-CurrentGraffitiGame.gSpot.transform.forward, Vector3.up);
                packet.MoveStyle = (int)player.moveStyleEquipped;
                packet.Position = (CurrentGraffitiGame.gSpot.transform.position + (CurrentGraffitiGame.gSpot.transform.forward * ClientConstants.PlayerGraffitiDistance) + (-CurrentGraffitiGame.gSpot.transform.up * ClientConstants.PlayerGraffitiDownDistance)).ToSystemVector3();
                packet.Rotation = grafRotation.ToSystemQuaternion();
                packet.BoostpackEffectMode = (byte)CurrentGraffitiGame.characterPuppet.boostpackEffectMode;
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
            _client?.Update();
            if (Connected)
            {
                SendVisualState();
            }
            ClientLobbyManager.OnTick();
        }

        private void Update()
        {
            _tickTimer += Time.deltaTime;
            if (_tickTimer >= TickRate)
            {
                DeltaTime = _tickTimer;
                Tick();
                _tickTimer = 0f;
            }
            foreach(var player in Players)
            {
                player.Value.FrameUpdate();
            }
            ClientLobbyManager.OnUpdate();
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
            }
        }

        private void OnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            var packetId = (Packets)e.MessageId;
            var packet = PacketFactory.PacketFromMessage(packetId, e.Message);
            if (packet == null) return;
            PacketReceived?.Invoke(packetId, packet);
            if (packet is PlayerPacket)
            {
                var playerPacket = packet as PlayerPacket;
                if (playerPacket.ClientId == LocalID && !_mpSettings.DebugLocalPlayer) return;
            }
            switch (packetId)
            {
                case Packets.ServerConnectionResponse:
                    {
                        var connectionResponse = (ServerConnectionResponse)packet;
                        LocalID = connectionResponse.LocalClientId;
                        TickRate = connectionResponse.TickRate;
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
                    }
                    break;

                case Packets.ServerClientVisualStates:
                    {
                        var clientVisualStates = (ServerClientVisualStates)packet;
                        foreach (var clientVisualState in clientVisualStates.ClientVisualStates)
                        {
                            if (!Players.TryGetValue(clientVisualState.Key, out var player)) continue;
                            player.UpdateVisualState(clientVisualState.Value);
                        }
                    }
                    break;

                case Packets.PlayerAnimation:
                    {
                        var playerPacket = (PlayerAnimation)packet;
                        if (Players.TryGetValue(playerPacket.ClientId, out var player))
                        {
                            if (player.Player != null)
                                MPUtility.PlayAnimationOnMultiplayerPlayer(player.Player, playerPacket.NewAnim, playerPacket.ForceOverwrite, playerPacket.Instant, playerPacket.AtTime);
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
            SendPacket(authPacket, IMessage.SendModes.Reliable, NetChannels.Default);
        }

        public ClientState CreateClientState()
        {
            var player = WorldHandler.instance.GetCurrentPlayer();
            var playerComp = PlayerComponent.Get(player);
            var statePacket = new ClientState()
            {
                Name = _mpSettings.PlayerName,
                Character = (int)player.character,
                Outfit = (byte)Core.Instance.SaveManager.CurrentSaveSlot.GetCharacterProgress(player.character).outfit,
                Stage = (int)Reptile.Utility.SceneNameToStage(SceneManager.GetActiveScene().name),
                ProtocolVersion = Constants.ProtocolVersion,
                SpecialSkin = playerComp.SpecialSkin,
                SpecialSkinVariant = playerComp.SpecialSkinVariant,
            };

            if (CrewBoomSupport.Installed)
            {
                statePacket.CrewBoomCharacter = CrewBoomSupport.GetGuidForCharacter(player.character);
            }
            return statePacket;
        }

        public void SendClientState()
        {
            SendPacket(CreateClientState(), IMessage.SendModes.Reliable, NetChannels.ClientAndLobbyUpdates);
        }

        private void OnConnected(object sender, EventArgs e)
        {
            ClientLogger.Log("Connected! Sending Auth request...");
            SendAuth();
        }

        private void OnDestroy()
        {
            Disconnect();
            ClientLobbyManager?.Dispose();
        }
    }
}
