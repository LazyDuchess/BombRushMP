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

        private void Awake()
        {
            Instance = this;
            _canvas = transform.Find("Canvas").gameObject;
            _lobbySettings = _canvas.transform.Find("Lobby Settings").GetComponent<TextMeshProUGUI>();
            _lobbyName = _canvas.transform.Find("Lobby Name").GetComponent<TextMeshProUGUI>();
            _lobbyManager = ClientController.Instance.ClientLobbyManager;
            ClientLobbyManager.LobbiesUpdated += OnLobbiesUpdated;
            ClientLobbyManager.LobbySoftUpdated += OnLobbiesUpdated;
            ClientController.ClientStatesUpdate += OnLobbiesUpdated;
            _playerName = _canvas.transform.Find("Player Name").gameObject;
            _playerName.SetActive(false);
            InitializePlayerUIPool();
            Deactivate();
        }

        private void Update()
        {
            if (_updateQueued)
            {
                _updateQueued = false;
                var currentLobby = _lobbyManager.CurrentLobby;
                if (currentLobby == null)
                    Deactivate();
                else
                    Activate();
            }
        }

        private void OnDestroy()
        {
            ClientLobbyManager.LobbiesUpdated -= OnLobbiesUpdated;
            ClientLobbyManager.LobbySoftUpdated -= OnLobbiesUpdated;
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

        private void Activate()
        {
            _canvas.SetActive(true);
            UpdateUI();
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
            var players = _lobbyManager.CurrentLobby.LobbyState.Players.Values.OrderByDescending(p => p.Score).ToArray();
            if (gamemode.TeamBased)
            {
                var teamOrder = new byte[TeamManager.Teams.Length];
                for(var i=0;i<TeamManager.Teams.Length;i++)
                {
                    teamOrder[i] = (byte)i;
                }
                teamOrder = teamOrder.OrderByDescending(t => lobby.LobbyState.GetScoreForTeam(t)).ToArray();
                players = players.OrderBy(p => Array.IndexOf(teamOrder, p.Team)).ToArray();
            }
            var teamStanding = 1;
            var lastTeam = -1;
            var playerIndex = 0;
            for(var i = 0; i < PlayerUIPoolSize; i++)
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
                    team = TeamManager.Teams[player.Team];
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
