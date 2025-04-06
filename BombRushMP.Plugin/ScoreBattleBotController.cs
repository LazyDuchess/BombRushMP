using Reptile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using BombRushMP.Common;
using BombRushMP.Plugin.Gamemodes;
using BombRushMP.Common.Packets;
using System.Globalization;

namespace BombRushMP.Plugin
{
    public class ScoreBattleBotController : MonoBehaviour
    {
        private const float HelpMessageInterval = 30f;
        private const float Timeout = 30f;
        private bool _sentStartPacket = false;
        private bool _sentCancelPacket = false;
        private int _cachedPlayerCount = 1;
        private float _timeoutTimer = 0f;
        private bool _lobbyCreated = false;
        private Vector3 _battlePosition = Vector3.zero;
        private Quaternion _battleRotation = Quaternion.identity;
        private Vector3 _returnPosition = Vector3.zero;
        private Quaternion _returnRotation = Quaternion.identity;
        private string _botPrefix = "!";
        private float _helpMessageTimer = 0f;

        private void MakeAvailable()
        {
            ClientController.Instance.ClientLobbyManager.SetChallenge(true);
            if (_helpMessageTimer <= 0f)
            {
                _helpMessageTimer = HelpMessageInterval;
                ClientController.Instance.SendPacket(new ClientChat($"I'm now Available. {_botPrefix}help to know more."), Common.Networking.IMessage.SendModes.ReliableUnordered, Common.Networking.NetChannels.Chat);
            }
        }

        private void SendHelp()
        {
            SendChat($"Hi! I'm score battle bot. Use me to practice score battles! Commands: {_botPrefix}help, {_botPrefix}seankingston, {_botPrefix}chibi, {_botPrefix}forklift, {_botPrefix}changechar");
        }

        private void RandomizeCharacter()
        {
            var player = WorldHandler.instance.GetCurrentPlayer();
            var possibleChars = new List<Characters>();
            var chars = Enum.GetValues(typeof(Characters));
            foreach(var ch in chars)
            {
                if ((Characters)ch == Characters.NONE || (Characters)ch == Characters.MAX) continue;
                possibleChars.Add((Characters)ch);
            }
            player.character = possibleChars[UnityEngine.Random.Range(0, possibleChars.Count)];
            SpecialSkinManager.Instance.RemoveSpecialSkinFromPlayer(player);
        }

        private void ParseChatMessage(string chatmessage)
        {
            var clientController = ClientController.Instance;
            var msg = chatmessage.Trim();
            if (!msg.StartsWith(_botPrefix)) return;
            var cmd = msg.Substring(_botPrefix.Length).ToLowerInvariant();
            switch (cmd)
            {
                case "help":
                    SendHelp();
                    break;

                case "seankingston":
                    SendChat($"/makeseankingston {clientController.LocalID}");
                    break;

                case "chibi":
                    SendChat($"/chibi");
                    break;

                case "forklift":
                    SendChat($"/makeforkliftcertified {clientController.LocalID}");
                    break;

                case "changechar":
                    RandomizeCharacter();
                    break;
            }
        }

        private void SendChat(string message)
        {
            ClientController.Instance.SendPacket(new ClientChat(message), Common.Networking.IMessage.SendModes.ReliableUnordered, Common.Networking.NetChannels.Chat);
        }

        private void Awake()
        {
            SetReturnPos(false);
            var taxiSpot = FindFirstObjectByType<Taxi>();
            ClientController.ServerConnect += OnConnected;
            ClientLobbyManager.LobbyChanged += OnLobbyChanged;
            ClientLobbyManager.LobbySoftUpdated += OnLobbyUpdate;
            ClientLobbyManager.LobbiesUpdated += OnLobbyUpdate;
            ClientController.PacketReceived += OnPacketReceived;
            //_taxiSpot.transform.position + (_taxiSpot.transform.forward * 1f), Quaternion.LookRotation(_taxiSpot.transform.forward));
            if (taxiSpot != null)
            {
                _battlePosition = taxiSpot.transform.position + (taxiSpot.transform.forward * 1f);
                _battleRotation = Quaternion.LookRotation(taxiSpot.transform.forward);

                _returnPosition = taxiSpot.transform.position + (taxiSpot.transform.forward * 2f) + (taxiSpot.transform.right * 3f);
                _returnRotation = Quaternion.LookRotation(taxiSpot.transform.forward);
            }
            else
            {
                SetBattlePos(false);
                SetReturnPos(false);
            }
        }

        private void SetBattlePos(bool message)
        {
            var worldHandler = WorldHandler.instance;
            var player = worldHandler.GetCurrentPlayer();
            _battlePosition = player.transform.position;
            _battleRotation = player.transform.rotation;
            if (message)
                ClientController.Instance.SendPacket(new ClientChat($"Updated my battle location!"), Common.Networking.IMessage.SendModes.ReliableUnordered, Common.Networking.NetChannels.Chat);
        }

        private void SetReturnPos(bool message)
        {
            var worldHandler = WorldHandler.instance;
            var player = worldHandler.GetCurrentPlayer();
            _returnPosition = player.transform.position;
            _returnRotation = player.transform.rotation;
            if (message)
                ClientController.Instance.SendPacket(new ClientChat($"Updated my return location!"), Common.Networking.IMessage.SendModes.ReliableUnordered, Common.Networking.NetChannels.Chat);
        }

        private void PlaceAtReturn()
        {
            MPUtility.PlaceCurrentPlayer(_returnPosition, _returnRotation);
        }

        private void PlaceAtBattle()
        {
            MPUtility.PlaceCurrentPlayer(_battlePosition, _battleRotation);
        }

        private void OnDestroy()
        {
            ClientController.ServerConnect -= OnConnected;
            ClientLobbyManager.LobbyChanged -= OnLobbyChanged;
            ClientLobbyManager.LobbySoftUpdated -= OnLobbyUpdate;
            ClientLobbyManager.LobbiesUpdated -= OnLobbyUpdate;
            ClientController.PacketReceived -= OnPacketReceived;
        }

        private void Update()
        {
            _helpMessageTimer -= Time.unscaledDeltaTime;
            if (_helpMessageTimer <= 0f)
                _helpMessageTimer = 0f;
            if (Input.GetKeyDown(KeyCode.F3))
                SetReturnPos(true);
            if (Input.GetKeyDown(KeyCode.F4))
                SetBattlePos(true);
            var clientController = ClientController.Instance;
            var currentLobby = clientController.ClientLobbyManager.CurrentLobby;
            if (currentLobby != null)
            {
                if (currentLobby.InGame && currentLobby.LobbyState.Players.Count <= 1 && !_sentCancelPacket)
                {
                    _sentCancelPacket = true;
                    clientController.ClientLobbyManager.EndGame();
                }
                if (currentLobby.InGame || currentLobby.LobbyState.Players.Count <= 1)
                {
                    _timeoutTimer = 0f;
                }
                else
                {
                    _timeoutTimer += Time.unscaledDeltaTime;
                    if (_timeoutTimer > Timeout)
                    {
                        _timeoutTimer = 0f;
                        KickEveryone();
                    }
                }
            }
            else
                _timeoutTimer = 0f;
            
        }

        private void CachePlayerCount()
        {
            var clientController = ClientController.Instance;
            var currentLobby = clientController.ClientLobbyManager.CurrentLobby;
            if (currentLobby == null)
                _cachedPlayerCount = 1;
            else
            {
                _cachedPlayerCount = currentLobby.LobbyState.Players.Count;
            }
        }

        private void KickEveryone()
        {
            var clientController = ClientController.Instance;
            var currentLobby = clientController.ClientLobbyManager.CurrentLobby;
            if (currentLobby == null) return;
            foreach (var player in currentLobby.LobbyState.Players)
            {
                if (player.Key == clientController.LocalID) continue;
                clientController.SendPacket(new ClientLobbyKick(player.Key), Common.Networking.IMessage.SendModes.Reliable, Common.Networking.NetChannels.ClientAndLobbyUpdates);
            }
            clientController.SendPacket(new ClientChat($"Kicked due to inactivity."), Common.Networking.IMessage.SendModes.ReliableUnordered, Common.Networking.NetChannels.Chat);
        }

        private void KickExtraPlayers()
        {
            if (_cachedPlayerCount <= 2) return;
            var clientController = ClientController.Instance;
            var currentLobby = clientController.ClientLobbyManager.CurrentLobby;
            if (currentLobby == null) return;
            var gotFirstPlayer = false;
            foreach(var player in currentLobby.LobbyState.Players)
            {
                if (player.Key == clientController.LocalID) continue;
                if (gotFirstPlayer)
                {
                    clientController.SendPacket(new ClientLobbyKick(player.Key), Common.Networking.IMessage.SendModes.Reliable, Common.Networking.NetChannels.ClientAndLobbyUpdates);
                }
                else
                    gotFirstPlayer = true;
            }
        }

        private void OnPacketReceived(Packets packets, Packet packet)
        {
            var clientController = ClientController.Instance;
            switch (packets)
            {
                case Packets.ServerLobbyEnd:
                    CachePlayerCount();
                    var lobbyPacket = packet as ServerLobbyEnd;
                    _sentStartPacket = false;
                    _sentCancelPacket = false;
                    if (!lobbyPacket.Cancelled && _cachedPlayerCount > 1)
                    {
                        var play = clientController.ClientLobbyManager.CurrentLobby.GetHighestScoringPlayer();
                        var hiScore = play.Score;
                        clientController.SendPacket(new ClientChat($"Score for {MPUtility.GetPlayerDisplayName(clientController.Players[play.Id].ClientState.Name)}: {FormatScore(hiScore)}"), Common.Networking.IMessage.SendModes.ReliableUnordered, Common.Networking.NetChannels.Chat);
                    }
                    if (_cachedPlayerCount <= 1)
                    {
                        PlaceAtReturn();
                        MakeAvailable();
                    }
                    break;

                case Packets.ServerConnectionResponse:
                    clientController.SendPacket(new ClientChat($"Hi! It's me, Score Battle Bot! Interact with me to practice!"), Common.Networking.IMessage.SendModes.ReliableUnordered, Common.Networking.NetChannels.Chat);
                    break;

                case Packets.ServerChat:
                    var chatPacket = packet as ServerChat;
                    if (chatPacket.MessageType == ChatMessageTypes.Chat)
                        ParseChatMessage(chatPacket.Message);
                    break;
            }
        }

        private string FormatScore(float score)
        {
            if (score <= 0f)
                return "0";
            return FormattingUtility.FormatPlayerScore(CultureInfo.CurrentCulture, score);
        }

        private void OnConnected()
        {
            _lobbyCreated = false;
            CreateLobbyIfIShould();
        }

        private void OnLobbyChanged()
        {
            if (_lobbyCreated) return;
            CachePlayerCount();
            CreateLobbyIfIShould();
            var clientController = ClientController.Instance;
            var currentLobby = clientController.ClientLobbyManager.CurrentLobby;
            Debug.Log("SETTING CHALLENGE TO TRUE (OnLobbyChanged)");
            MakeAvailable();
            _lobbyCreated = true;
            _sentCancelPacket = false;
        }

        private void OnLobbyUpdate()
        {
            var clientController = ClientController.Instance;
            var prevPlayerCount = _cachedPlayerCount;
            CachePlayerCount();
            var currentLobby = clientController.ClientLobbyManager.CurrentLobby;
            if (currentLobby != null)
            {
                if (_sentStartPacket && _cachedPlayerCount <= 1 && currentLobby.InGame)
                {
                    _sentStartPacket = false;
                    _sentCancelPacket = true;
                    clientController.ClientLobbyManager.EndGame();
                    return;
                }
                if (prevPlayerCount != _cachedPlayerCount)
                {
                    Debug.Log($"PrevPlayerCount: {prevPlayerCount}, _cachedPlayerCount: {_cachedPlayerCount}");
                    if (currentLobby.LobbyState.Players.Count <= 1)
                    {
                        Debug.Log("SETTING CHALLENGE TO TRUE (LOBBYUPDATE)");
                        MakeAvailable();
                    }
                    else if (currentLobby.LobbyState.Players.Count > 1)
                    {
                        Debug.Log("SETTING CHALLENGE TO FALSE (LOBBYUPDATE)");
                        clientController.ClientLobbyManager.SetChallenge(false);
                    }
                }
                KickExtraPlayers();
            }
            if (_cachedPlayerCount < prevPlayerCount && _cachedPlayerCount <= 1)
            {
                PlaceAtReturn();
            }
            if (_sentStartPacket) return;
            CreateLobbyIfIShould();
            if (_cachedPlayerCount > prevPlayerCount)
            {
                clientController.SendPacket(new ClientChat("Whenever you're ready, toggle Ready on your phone or press R to start!"), Common.Networking.IMessage.SendModes.ReliableUnordered, Common.Networking.NetChannels.Chat);
                _timeoutTimer = 0f;
                PlaceAtBattle();
            }
            if (currentLobby != null && !currentLobby.InGame)
            {
                if (currentLobby.LobbyState.Players.Count <= 1)
                    return;
                foreach(var player in currentLobby.LobbyState.Players)
                {
                    if (player.Key == clientController.LocalID) continue;
                    if (!player.Value.Ready)
                        return;
                }
                OnEveryoneReady();
            }
        }

        private void OnEveryoneReady()
        {
            _timeoutTimer = 0f;
            _sentCancelPacket = false;
            _sentStartPacket = true;
            var clientController = ClientController.Instance;
            clientController.ClientLobbyManager.StartGame();
        }

        private void CreateLobbyIfIShould()
        {
            if (_lobbyCreated) return;
            var clientController = ClientController.Instance;
            if (clientController.ClientLobbyManager.CurrentLobby != null) return;
            var settings = GamemodeFactory.GetGamemodeSettings(GamemodeIDs.ScoreBattle);
            settings.SettingByID[Animator.StringToHash("SpawnMode")].Value = (int)ScoreBattle.SpawnMode.At_Host;
            (settings.SettingByID[Animator.StringToHash("TPToSpawnOnEnd")] as ToggleGamemodeSetting).Value = (int)ToggleGamemodeSetting.Toggle.ON;
            clientController.ClientLobbyManager.CreateLobby(GamemodeIDs.ScoreBattle, settings);
            _timeoutTimer = 0f;
        }

        public static void Create()
        {
            var go = new GameObject("Score Battle Bot Controller");
            go.AddComponent<ScoreBattleBotController>();
        }
    }
}
