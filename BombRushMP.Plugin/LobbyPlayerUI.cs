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
using UnityEngine.UI;

namespace BombRushMP.Plugin
{
    public class LobbyPlayerUI : MonoBehaviour
    {
        private TextMeshProUGUI _playerName;
        private TextMeshProUGUI _score;
        private ClientController _clientController;
        private LobbyPlayer _lobbyPlayer = null;
        private Lobby _lobby = null;
        private GameObject _readySprite;
        private GameObject _notReadySprite;
        private GameObject _afkSprite;
        private Image _bg;
        private Image _teamBg;
        public int Position = -1;
        private void Awake()
        {
            _playerName = transform.Find("Label").GetComponent<TextMeshProUGUI>();
            _score = transform.Find("Score").GetComponent<TextMeshProUGUI>();
            _readySprite = transform.Find("Ready").gameObject;
            _notReadySprite = transform.Find("Not Ready").gameObject;
            _afkSprite = transform.Find("AFK").gameObject;
            _bg = transform.Find("BG").GetComponent<Image>();
            _teamBg = transform.Find("Team BG").GetComponent<Image>();
            _clientController = ClientController.Instance;
            _playerName.spriteAsset = MPAssets.Instance.Sprites;
        }

        private void Update()
        {
            if (_lobbyPlayer == null) return;
            var player = _clientController.Players[_lobbyPlayer.Id];
            var playerVisualState = player.ClientVisualState;
            if (playerVisualState == null) return;
            _afkSprite.SetActive(playerVisualState.AFK);
            if (!_lobby.InGame)
            {
                _readySprite.SetActive(_lobbyPlayer.Ready);
                _notReadySprite.SetActive(!_lobbyPlayer.Ready);
            }
            else
            {
                _readySprite.SetActive(false);
                _notReadySprite.SetActive(false);
            }

            if (_lobby.LobbyState.HostId == _lobbyPlayer.Id)
            {
                _readySprite.SetActive(false);
                _notReadySprite.SetActive(false);
            }
        }

        private string FormatScore(float score)
        {
            if (score <= 0f)
                return "0";
            return FormattingUtility.FormatPlayerScore(CultureInfo.CurrentCulture, score);
        }

        public void SetTeam(Lobby lobby, Team team, byte teamId, int standing)
        {
            var teamName = MPUtility.GetCrewDisplayName(MPUtility.GetTeamName(lobby.LobbyState, team, teamId));
            if (string.IsNullOrWhiteSpace(teamName))
                teamName = team.Name;
            _lobbyPlayer = null;
            _playerName.text = $"{standing}. {teamName}";
            _bg.gameObject.SetActive(false);
            _readySprite.SetActive(false);
            _notReadySprite.SetActive(false);
            _teamBg.gameObject.SetActive(true);
            _teamBg.color = new Color(team.Color.r, team.Color.g, team.Color.b, _teamBg.color.a);
            _score.text = FormatScore(_lobby.LobbyState.GetScoreForTeam(teamId));
        }

        private const float TeamPlayerDarkening = 0.25f;

        public void SetPlayer(LobbyPlayer player, Team team)
        {
            _bg.gameObject.SetActive(false);
            _teamBg.gameObject.SetActive(false);

            if (team == null)
            {
                _bg.gameObject.SetActive(true);
            }
            else
            {
                _teamBg.gameObject.SetActive(true);
                _teamBg.color = new Color(team.Color.r * TeamPlayerDarkening, team.Color.g * TeamPlayerDarkening, team.Color.b * TeamPlayerDarkening, _teamBg.color.a);
            }

            var lobby = _clientController.ClientLobbyManager.Lobbies[player.LobbyId];
            _lobby = lobby;

            var playername = MPUtility.GetPlayerDisplayName(_clientController.Players[player.Id].ClientState);

            if (lobby.LobbyState.HostId == player.Id)
            {
                playername = $"<color=yellow>[Host]</color> {playername}";
            }

            _lobbyPlayer = player;
            _playerName.text = playername;
            _score.text = FormatScore(_lobbyPlayer.Score);
            if (Position != -1 && team == null)
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
