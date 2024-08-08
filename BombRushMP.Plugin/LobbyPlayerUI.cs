using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using BombRushMP.Common;

namespace BombRushMP.Plugin
{
    public class LobbyPlayerUI : MonoBehaviour
    {
        private TextMeshProUGUI _playerName;
        private TextMeshProUGUI _score;
        private ClientController _clientController;
        private LobbyPlayer _lobbyPlayer = null;
        private void Awake()
        {
            _playerName = transform.Find("Label").GetComponent<TextMeshProUGUI>();
            _score = transform.Find("Score").GetComponent<TextMeshProUGUI>();
            _clientController = ClientController.Instance;
        }

        private void Update()
        {
            if (_lobbyPlayer == null) return;
            _score.text = _lobbyPlayer.Score.ToString();
        }

        public void SetPlayer(LobbyPlayer player)
        {
            var lobby = _clientController.ClientLobbyManager.Lobbies[player.LobbyId];

            var playername = _clientController.Players[player.Id].ClientState.Name;

            if (lobby.HostId == player.Id)
                playername = $"<color=yellow>[Host]</color> {playername}";

            _playerName.text = playername;
            _score.text = player.Score.ToString();
            _lobbyPlayer = player;
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
