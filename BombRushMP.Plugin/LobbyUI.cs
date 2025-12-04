using BombRushMP.Common.Networking;
using BombRushMP.Common.Packets;
using BombRushMP.Plugin.Gamemodes;
using Reptile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace BombRushMP.Plugin
{
    public class LobbyUI : MonoBehaviour
    {
        public static LobbyUI Instance { get; private set; }
        private GameObject _canvas;
        private TextMeshProUGUI _lobbyName;
        private ClientLobbyManager _lobbyManager;
        private GameObject _playerName;
        private float _playerNameHeight = 35f;
        private const int PlayerUIPoolSize = 32;
        private LobbyPlayerUI[] _playerUIs = new LobbyPlayerUI[PlayerUIPoolSize];
        private TextMeshProUGUI _lobbySettings;
        private bool _updateQueued = false;
        private bool _softUpdateQueued = false;
        private const float SoftUpdateRate = 0.5f;
        private float _softUpdateTimer = 0f;

        private void Awake()
        {
            Instance = this;
            _canvas = transform.Find("Canvas").gameObject;
            _lobbySettings = _canvas.transform.Find("Lobby Settings").GetComponent<TextMeshProUGUI>();
            _lobbyName = _canvas.transform.Find("Lobby Name").GetComponent<TextMeshProUGUI>();
            _lobbyManager = ClientController.Instance.ClientLobbyManager;
            ClientLobbyManager.LobbiesUpdated += OnLobbiesUpdated;
            ClientLobbyManager.LobbySoftUpdated += OnLobbySoftUpdated;
            ClientController.ClientStatesUpdate += OnLobbiesUpdated;
            _playerName = _canvas.transform.Find("Player Name").gameObject;
            _playerName.SetActive(false);
            InitializePlayerUIPool();
            Deactivate();
        }

        private void Update()
        {
            var clientController = ClientController.Instance;
            var currentLobby = _lobbyManager.CurrentLobby;
            _softUpdateTimer += Time.deltaTime;
            if (_updateQueued)
            {
                _softUpdateQueued = false;
                _updateQueued = false;
                if (currentLobby == null)
                    Deactivate();
                else
                    Activate();
            }
            if (_softUpdateQueued && _softUpdateTimer >= SoftUpdateRate)
            {
                _softUpdateQueued = false;
                UpdatePlayerListing();
            }
            if (_softUpdateTimer >= SoftUpdateRate)
                _softUpdateTimer = 0f;

            if (currentLobby != null && !currentLobby.InGame)
            {
                if (currentLobby.LobbyState.HostId != clientController.LocalID)
                {
                    var gameInput = Core.Instance.GameInput;
                    var enabledMaps = gameInput.GetAllCurrentEnabledControllerMapCategoryIDs(0);
                    if (enabledMaps.controllerMapCategoryIDs.Contains(0) && enabledMaps.controllerMapCategoryIDs.Contains(6) && !Core.Instance.IsCorePaused && !MPUtility.AnyMenusOpen())
                    {
                        if (Input.GetKeyDown(MPSettings.Instance.ReadyKey))
                        {
                            clientController.SendPacket(new ClientLobbySetReady(!currentLobby.LobbyState.Players[clientController.LocalID].Ready), IMessage.SendModes.ReliableUnordered, NetChannels.ClientAndLobbyUpdates);
                        }
                    }
                }
            }
        }

        private void OnDestroy()
        {
            ClientLobbyManager.LobbiesUpdated -= OnLobbiesUpdated;
            ClientLobbyManager.LobbySoftUpdated -= OnLobbySoftUpdated;
            ClientController.ClientStatesUpdate -= OnLobbiesUpdated;
        }

        private void InitializePlayerUIPool()
        {
            for(var i = 0; i < PlayerUIPoolSize; i++)
            {
                var playerui = LobbyPlayerUI.Create(_playerName);
                playerui.transform.SetParent(_canvas.transform, false);
                playerui.transform.localPosition -= new Vector3(0, (i + 1) * _playerNameHeight, 0);
                playerui.Position = i;
                _playerUIs[i] = playerui;
                playerui.gameObject.SetActive(false);
            }
        }

        private void OnLobbiesUpdated()
        {
            _updateQueued = true;
        }

        private void OnLobbySoftUpdated()
        {
            _softUpdateQueued = true;
        }

        private void Activate()
        {
            _canvas.SetActive(true);
            UpdateUI();
        }

        private void UpdatePlayerListing()
        {
            var lobby = _lobbyManager.CurrentLobby;
            if (lobby == null) return;
            var players = lobby.LobbyState.Players.Values.OrderByDescending(p => p.Score).ToArray();
            var gamemode = lobby.GetOrCreateGamemode();
            var teams = GamemodeFactory.GetTeams(lobby.LobbyState.Gamemode);
            if (gamemode.TeamBased)
            {
                var teamOrder = new byte[teams.Length];
                for (var i = 0; i < teams.Length; i++)
                {
                    teamOrder[i] = (byte)i;
                }
                teamOrder = teamOrder.OrderByDescending(t => lobby.LobbyState.GetScoreForTeam(t)).ToArray();
                players = players.OrderBy(p => Array.IndexOf(teamOrder, p.Team)).ToArray();
            }
            var teamStanding = 1;
            var lastTeam = -1;
            var playerIndex = 0;
            for (var i = 0; i < PlayerUIPoolSize; i++)
            {
                var playerui = _playerUIs[i];
                if (playerIndex >= players.Length)
                {
                    if (playerui.gameObject.activeSelf)
                        playerui.gameObject.SetActive(false);
                    continue;
                }
                var player = players[playerIndex];
                Team team = null;
                if (gamemode.TeamBased)
                {
                    team = teams[player.Team];
                }
                if (player.Team != lastTeam && gamemode.TeamBased)
                {
                    lastTeam = player.Team;
                    if (!playerui.gameObject.activeSelf)
                        playerui.gameObject.SetActive(true);
                    playerui.SetTeam(lobby, team, player.Team, teamStanding);
                    teamStanding++;
                    continue;
                }
                else
                {
                    playerIndex++;
                }
                if (!playerui.gameObject.activeSelf)
                    playerui.gameObject.SetActive(true);
                playerui.SetPlayer(player, team);
            }
        }

        public void UpdateUI()
        {
#if DEBUG
            if (!MPSettings.Instance.UpdateLobbyUI)
                return;
#endif
            var lobby = _lobbyManager.CurrentLobby;
            if (lobby == null) return;
            var gamemode = lobby.GetOrCreateGamemode();
            GamemodeSettings lobbySettings = null;
            if (lobby.InGame)
                lobbySettings = lobby.CurrentGamemode.Settings;
            else
                lobbySettings = GamemodeFactory.ParseGamemodeSettings(_lobbyManager.CurrentLobby.LobbyState.Gamemode, _lobbyManager.CurrentLobby.LobbyState.GamemodeSettings);
            _lobbySettings.text = lobbySettings.GetDisplayString(_lobbyManager.CurrentLobby.LobbyState.HostId == ClientController.Instance.LocalID, _lobbyManager.CurrentLobby.InGame);
            _lobbyName.text = _lobbyManager.GetLobbyName(_lobbyManager.CurrentLobby.LobbyState.Id);
            UpdatePlayerListing();
        }

        private void Deactivate()
        {
            _canvas.SetActive(false);
        }

        public static void Create()
        {
            if (Instance != null)
                Destroy(Instance.gameObject);
            var mpAssets = MPAssets.Instance;
            var prefab = mpAssets.Bundle.LoadAsset<GameObject>("Lobby UI");
            var lobbyUi = Instantiate(prefab);
            lobbyUi.transform.SetParent(Core.Instance.UIManager.transform, false);
            lobbyUi.AddComponent<LobbyUI>();
        }
    }
}
