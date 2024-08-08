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
        private float _playerNameHeight = 40f;
        private const int PlayerUIPoolSize = 32;
        private LobbyPlayerUI[] _playerUIs = new LobbyPlayerUI[PlayerUIPoolSize];
        private void Awake()
        {
            Instance = this;
            _canvas = transform.Find("Canvas").gameObject;
            _lobbyName = _canvas.transform.Find("Lobby Name").GetComponent<TextMeshProUGUI>();
            _lobbyManager = ClientController.Instance.ClientLobbyManager;
            _lobbyManager.LobbiesUpdated += OnLobbiesUpdated;
            _playerName = _canvas.transform.Find("Player Name").gameObject;
            _playerName.SetActive(false);
            InitializePlayerUIPool();
            Deactivate();
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
            var currentLobby = _lobbyManager.CurrentLobby;
            if (currentLobby == null)
                Deactivate();
            else
                Activate();
        }

        private void Activate()
        {
            _canvas.SetActive(true);
            UpdateUI();
        }

        private void UpdateUI()
        {
            _lobbyName.text = _lobbyManager.GetLobbyName(_lobbyManager.CurrentLobby.LobbyState.Id);
            var players = _lobbyManager.CurrentLobby.LobbyState.Players.Values.OrderByDescending(p => p.Score).ToList();
            for(var i = 0; i < PlayerUIPoolSize; i++)
            {
                var playerui = _playerUIs[i];
                if (i >= players.Count)
                {
                    if (playerui.gameObject.activeSelf)
                        playerui.gameObject.SetActive(false);
                    continue;
                }
                if (!playerui.gameObject.activeSelf)
                    playerui.gameObject.SetActive(true);
                var player = players[i];
                playerui.SetPlayer(player);
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
