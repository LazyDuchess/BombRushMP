using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using BombRushMP.Common;
using Reptile;
using System.Globalization;

namespace BombRushMP.Plugin
{
    public class LobbyPlayerUI : MonoBehaviour
    {
        private TextMeshProUGUI _playerName;
        private TextMeshProUGUI _score;
        private ClientController _clientController;
        private LobbyPlayer _lobbyPlayer = null;
        private GameObject _readySprite;
        private GameObject _notReadySprite;
        private GameObject _afkSprite;
        public int Position = -1;
        private void Awake()
        {
            _playerName = transform.Find("Label").GetComponent<TextMeshProUGUI>();
            _score = transform.Find("Score").GetComponent<TextMeshProUGUI>();
            _readySprite = transform.Find("Ready").gameObject;
            _notReadySprite = transform.Find("Not Ready").gameObject;
            _afkSprite = transform.Find("AFK").gameObject;
            _clientController = ClientController.Instance;
        }

        private void Update()
        {
            if (_lobbyPlayer == null) return;
            var player = _clientController.Players[_lobbyPlayer.Id];
            var playerVisualState = player.ClientVisualState;
            if (playerVisualState == null) return;
            _afkSprite.SetActive(playerVisualState.AFK);
        }

        private string FormatScore(float score)
        {
            if (score <= 0f)
                return "0";
            return FormattingUtility.FormatPlayerScore(CultureInfo.CurrentCulture, _lobbyPlayer.Score);
        }

        public void SetPlayer(LobbyPlayer player)
        {
            var lobby = _clientController.ClientLobbyManager.Lobbies[player.LobbyId];

            var playername = _clientController.Players[player.Id].ClientState.Name;

            if (!lobby.InGame)
            {
                _readySprite.SetActive(player.Ready);
                _notReadySprite.SetActive(!player.Ready);
            }
            else
            {
                _readySprite.SetActive(false);
                _notReadySprite.SetActive(false);
            }

            if (lobby.LobbyState.HostId == player.Id)
            {
                playername = $"<color=yellow>[Host]</color> {playername}";
                _readySprite.SetActive(false);
                _notReadySprite.SetActive(false);
            }

            _lobbyPlayer = player;
            _playerName.text = playername;
            _score.text = FormatScore(_lobbyPlayer.Score);
            if (Position != -1)
            {
                _playerName.text = $"{Position + 1}. {playername}";
            }
        }

        public static LobbyPlayerUI Create(GameObject reference)
        {
            var go = Instantiate(reference);
            go.SetActive(true);
            var playerui = go.AddComponent<LobbyPlayerUI>();
            return playerui;
        }
    }
}
