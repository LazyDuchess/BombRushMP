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
        private bool _readySpriteActive = true;
        private bool _notReadySpriteActive = true;
        public int Position = -1;
        private void Awake()
        {
            _playerName = transform.Find("Label").GetComponent<TextMeshProUGUI>();
            _score = transform.Find("Score").GetComponent<TextMeshProUGUI>();
            _readySprite = transform.Find("Ready").gameObject;
            _notReadySprite = transform.Find("Not Ready").gameObject;
            _clientController = ClientController.Instance;
            _readySpriteActive = _readySprite.activeSelf;
            _notReadySpriteActive = _notReadySprite.activeSelf;
        }

        private string FormatScore(float score)
        {
            if (score <= 0f)
                return "0";
            return FormattingUtility.FormatPlayerScore(CultureInfo.CurrentCulture, _lobbyPlayer.Score);
        }

        public void SetPlayer(LobbyPlayer player)
        {
            var readySpriteNewActive = false;
            var notReadySpriteNewActive = false;

            var lobby = _clientController.ClientLobbyManager.Lobbies[player.LobbyId];

            var playername = _clientController.Players[player.Id].ClientState.Name;

            if (lobby.LobbyState.HostId == player.Id)
                playername = $"<color=yellow>[Host]</color> {playername}";
            else if (!lobby.LobbyState.InGame)
            {
                if (player.Ready)
                    readySpriteNewActive = true;
                else
                    notReadySpriteNewActive = true;
            }

            if (readySpriteNewActive != _readySpriteActive)
            {
                _readySpriteActive = readySpriteNewActive;
                _readySprite.SetActive(_readySpriteActive);
            }

            if (notReadySpriteNewActive != _notReadySpriteActive)
            {
                _notReadySpriteActive = notReadySpriteNewActive;
                _notReadySprite.SetActive(_notReadySpriteActive);
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
