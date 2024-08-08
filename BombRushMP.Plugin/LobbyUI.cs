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
        private GameObject _canvas;
        private TextMeshProUGUI _lobbyName;
        private ClientLobbyManager _lobbyManager;
        private GameObject _playerName;
        private float _playerNameHeight = 40f;
        private List<LobbyPlayerUI> _playerUIs = new();
        private void Awake()
        {
            _canvas = transform.Find("Canvas").gameObject;
            _lobbyName = _canvas.transform.Find("Lobby Name").GetComponent<TextMeshProUGUI>();
            _lobbyManager = ClientController.Instance.ClientLobbyManager;
            _lobbyManager.LobbiesUpdated += OnLobbiesUpdated;
            _playerName = _canvas.transform.Find("Player Name").gameObject;
            _playerName.SetActive(false);
            Deactivate();
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
            _lobbyName.text = _lobbyManager.GetLobbyName(_lobbyManager.CurrentLobby.Id);
            foreach (var playerui in _playerUIs)
            {
                Destroy(playerui.gameObject);
            }
            _playerUIs.Clear();
            var players = _lobbyManager.CurrentLobby.Players.Values.OrderByDescending(p => p.Score);
            foreach (var player in players)
            {
                var playerui = LobbyPlayerUI.Create(_playerName);
                playerui.transform.SetParent(_canvas.transform, false);
                playerui.transform.localPosition -= new Vector3(0, _playerUIs.Count + 1 * _playerNameHeight, 0);
                playerui.SetPlayer(player);
                _playerUIs.Add(playerui);
            }
        }

        private void Deactivate()
        {
            _canvas.SetActive(false);
        }

        public static void Create()
        {
            var mpAssets = MPAssets.Instance;
            var prefab = mpAssets.Bundle.LoadAsset<GameObject>("Lobby UI");
            var lobbyUi = Instantiate(prefab);
            lobbyUi.transform.SetParent(Core.Instance.UIManager.transform, false);
            lobbyUi.AddComponent<LobbyUI>();
        }
    }
}
