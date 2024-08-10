using BombRushMP.Common;
using BombRushMP.Common.Packets;
using BombRushMP.CrewBoom;
using Reptile;
using Riptide;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BombRushMP.Plugin
{
    public class ClientController : MonoBehaviour
    {
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
        public string Address = "";
        public GraffitiGame CurrentGraffitiGame = null;
        public Action ServerDisconnect;
        public Action ServerConnect;
        public Action<ushort> PlayerDisconnected;
        public Action<Packets, Packet> PacketReceived;
        private Client _client;
        private float _tickTimer = 0f;
        private bool _handShook = false;
        private MPSettings _mpSettings = null;

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

        public void Connect()
        {
            ClientLogger.Log($"Connecting to {Address}");
            _client = new Client();
            _client.Connect(Address);
            _client.Connected += OnConnected;
            _client.Disconnected += OnDisconnect;
            _client.ConnectionFailed += OnConnectionFailed;
            _client.MessageReceived += OnMessageReceived;
            _client.ClientDisconnected += OnClientDisconnected;
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
                _client.ClientDisconnected -= OnClientDisconnected;
                _client.Disconnect();
            }
            _client = null;
            ServerDisconnect?.Invoke();
        }
        
        public void SendGenericEvent(GenericEvents ev, MessageSendMode sendMode)
        {
            SendPacket(new PlayerGenericEvent(ev), sendMode);
        }

        public void SendPacket(Packet packet, MessageSendMode sendMode)
        {
            var message = PacketFactory.MessageFromPacket(packet, sendMode);
            _client.Send(message);
        }

        public static ClientController Create(string address)
        {
            var clientControllerGO = new GameObject("Client Controller");
            var clientController = clientControllerGO.AddComponent<ClientController>();
            clientController.Address = address;
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
                packet.BoostpackEffectMode = (int)player.characterVisual.boostpackEffectMode;
                packet.FrictionEffectMode = (int)player.characterVisual.frictionEffectMode;
                if (player.characterVisual.dustParticles != null)
                    packet.DustEmissionRate = (int)player.characterVisual.dustParticles.emission.rateOverTime.constant;
                packet.CurrentAnimation = player.curAnim;
                packet.CurrentAnimationTime = player.curAnimActiveTime;
            }
            else if (state == PlayerStates.Graffiti){
                var grafRotation = Quaternion.LookRotation(-CurrentGraffitiGame.gSpot.transform.forward, Vector3.up);
                packet.MoveStyle = (int)player.moveStyleEquipped;
                packet.Position = (CurrentGraffitiGame.gSpot.transform.position + (CurrentGraffitiGame.gSpot.transform.forward * ClientConstants.PlayerGraffitiDistance) + (-CurrentGraffitiGame.gSpot.transform.up * ClientConstants.PlayerGraffitiDownDistance)).ToSystemVector3();
                packet.Rotation = grafRotation.ToSystemQuaternion();
                packet.BoostpackEffectMode = (int)CurrentGraffitiGame.characterPuppet.boostpackEffectMode;
            }
            return packet;
        }

        private void SendVisualState()
        {
            var player = WorldHandler.instance.GetCurrentPlayer();
            if (player == null) return;
            var packet = CreateVisualStatePacket(player);
            SendPacket(packet, MessageSendMode.Unreliable);
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
            if (_tickTimer >= Constants.NetworkingTickRate)
            {
                Tick();
                _tickTimer = 0f;
            }
            foreach(var player in Players)
            {
                player.Value.FrameUpdate();
            }
            ClientLobbyManager.OnUpdate();
        }

        private void OnClientDisconnected(object sender, ClientDisconnectedEventArgs e)
        {
            if (Players.TryGetValue(e.Id, out var player))
            {
                player.Delete();
                Players.Remove(e.Id);
            }
            PlayerDisconnected?.Invoke(e.Id);
        }

        private void ExecuteGenericEvent(GenericEvents ev, MPPlayer player)
        {
            switch (ev)
            {
                case GenericEvents.Spray:
                    if (player.Player == null) break;
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
                        _handShook = true;
                        ClientLogger.Log($"Received server handshake - our local ID is {connectionResponse.LocalClientId}.");
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
                            player.ClientState = clientState.Value;
                            player.ClientId = clientState.Key;
                        }
                    }
                    break;

                case Packets.ServerClientVisualStates:
                    {
                        var clientVisualStates = (ServerClientVisualStates)packet;
                        foreach (var clientVisualState in clientVisualStates.ClientVisualStates)
                        {
                            if (!Players.TryGetValue(clientVisualState.Key, out var player)) continue;
                            player.ClientVisualState = clientVisualState.Value;
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

        private void OnConnectionFailed(object sender, ConnectionFailedEventArgs e)
        {
            ClientLogger.Log($"Failed to connect to server. Reason: {e.Reason}");
            ClientLogger.Log("Will attempt to re-connect");
            Disconnect();
            Connect();
        }

        private void OnDisconnect(object sender, DisconnectedEventArgs e)
        {
            ClientLogger.Log($"Disconnected! Reason: {e.Reason}");
            ClientLogger.Log("Will attempt to re-connect");
            Disconnect();
            Connect();
        }

        public void SendClientState()
        {
            var player = WorldHandler.instance.GetCurrentPlayer();
            var statePacket = new ClientState()
            {
                Name = _mpSettings.PlayerName,
                Character = (int)player.character,
                Outfit = Core.Instance.SaveManager.CurrentSaveSlot.GetCharacterProgress(player.character).outfit,
                Stage = (int)Reptile.Utility.SceneNameToStage(SceneManager.GetActiveScene().name),
                ProtocolVersion = Constants.ProtocolVersion
            };
            if (CrewBoomSupport.Installed)
            {
                statePacket.CrewBoomCharacter = CrewBoomSupport.GetGuidForCharacter(player.character);
            }
            SendPacket(statePacket, MessageSendMode.Reliable);
        }

        private void OnConnected(object sender, EventArgs e)
        {
            ClientLogger.Log("Connected!");
            SendClientState();
        }

        private void OnDestroy()
        {
            Disconnect();
            ClientLobbyManager?.Dispose();
        }
    }
}
