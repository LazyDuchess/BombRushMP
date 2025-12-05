using BombRushMP.Common;
using Reptile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace BombRushMP.Plugin
{
    public class SpectatorController : MonoBehaviour
    {
        public bool Forced = false;
        public ushort CurrentSpectatingClient = 0;
        public static SpectatorController Instance { get; private set; }
        private ClientController _clientController = null;
        private List<ushort> _players = new();
        private Player _currentSpectatingPlayer = null;
        private GameplayCamera _gameplayCamera = null;
        private WorldHandler _worldHandler = null;
        private GameInput _gameInput = null;
        private UIManager _uiManager = null;

        private void Awake()
        {
            var chatUi = ChatUI.Instance;
            if (chatUi != null)
                chatUi.SetState(ChatUI.States.Unfocused);
            var playerList = PlayerListUI.Instance;
            MPUtility.CloseMenusAndSpectator();
            _uiManager = Core.instance.UIManager;
            _gameInput = Core.Instance.GameInput;
            _worldHandler = WorldHandler.instance;
            var currentPlayer = _worldHandler.GetCurrentPlayer();
            if (!currentPlayer.userInputEnabled && !Forced)
            {
                EndSpectating();
                return;
            }
            _uiManager.gameplay.gameObject.SetActive(false);
            _gameInput.DisableAllControllerMaps(0);
            _gameInput.EnableControllerMaps(BaseModule.IN_GAME_INPUT_MAPS);
            _gameInput.EnableControllerMaps(BaseModule.MENU_INPUT_MAPS);
            currentPlayer.phone.TurnOff();
            currentPlayer.userInputEnabled = false;
            currentPlayer.SetIgnoreCamInput(true);
            currentPlayer.cam = null;
            currentPlayer.DropCombo();
            currentPlayer.SetVelocity(Vector3.zero);
            currentPlayer.motor._rigidbody.isKinematic = true;
            _gameplayCamera = GameplayCamera.instance;
            _clientController = ClientController.Instance;
            CachePlayers();
            if (_players.Count == 0 && !Forced)
            {
                EndSpectating();
                return;
            }
            CurrentSpectatingClient = _players[0];
            SpectatorUI.Instance.Activate();
        }
        
        public void SpectatePlayer(MPPlayer player)
        {
            var index = _players.IndexOf(player.ClientId);
            if (index < 0)
                return;
            CurrentSpectatingClient = _players[index];
        }

        public bool CanTeleportToCurrentPlayer()
        {
            if (_clientController == null) return false;
            var user = _clientController.GetLocalUser();
            if (user?.IsModerator == true) return true;
            if (_clientController.ClientLobbyManager.CurrentLobby != null && _clientController.ClientLobbyManager.CurrentLobby.InGame)
                return false;
            return (_clientController.Players[CurrentSpectatingClient].ClientState.AllowTeleports);
        }

        private void Update()
        {
            MPSaveData.Instance.Stats.TimeSpentSpectating += Core.dt;
            var user = ClientController.Instance.GetLocalUser();
            CachePlayers();
            if (_players.Count == 0 && !Forced)
            {
                EndSpectating();
                return;
            }
            var playerId = _players[GetCurrentSpectatingIndex()];
            var targetPlayer = _worldHandler.GetCurrentPlayer();
            if (playerId != ClientController.Instance.LocalID)
                targetPlayer = ClientController.Instance.Players[playerId].Player;
            if (_currentSpectatingPlayer != targetPlayer)
            {
                if (_currentSpectatingPlayer != null)
                {
                    _currentSpectatingPlayer.cam = null;
                }
                targetPlayer.cam = _gameplayCamera;
                _currentSpectatingPlayer = targetPlayer;
                _gameplayCamera.SetPlayerToFollow(targetPlayer);
            }
            if (_gameInput.GetButtonNew(2, 0) && CanTeleportToCurrentPlayer() && !Forced)
            {
                EndSpectating();
                MPUtility.PlaceCurrentPlayer(targetPlayer.transform.position, targetPlayer.transform.rotation);
                return;
            }

            if (_gameInput.GetButtonNew(3, 0) && !Forced)
            {
                EndSpectating();
                return;
            }

            if (_gameInput.GetButtonNew(57, 0))
                Previous();

            if (_gameInput.GetButtonNew(29, 0))
                Next();
        }

        private void Next()
        {
            var currentIndex = GetCurrentSpectatingIndex();
            currentIndex++;
            if (currentIndex >= _players.Count)
                currentIndex = 0;
            CurrentSpectatingClient = _players[currentIndex];
        }

        private void Previous()
        {
            var currentIndex = GetCurrentSpectatingIndex();
            currentIndex--;
            if (currentIndex < 0)
                currentIndex = _players.Count - 1;
            CurrentSpectatingClient = _players[currentIndex];
        }

        private void CachePlayers()
        {
            _players.Clear();
            var myLobby = _clientController.ClientLobbyManager.CurrentLobby;
            LobbyPlayer myLobbyPlayer = null;
            if (myLobby != null)
            {
                myLobbyPlayer = myLobby.LobbyState.Players[_clientController.LocalID];
            }
            foreach(var pl in _clientController.Players)
            {
                if (pl.Value.Player == null) continue;
                if (pl.Value.ShouldIgnore()) continue;
                if (myLobby != null && myLobby.InGame && myLobby.CurrentGamemode != null)
                {
                    if (!myLobby.LobbyState.Players.TryGetValue(pl.Key, out var lobbyPlayer))
                        continue;
                    if (myLobby.CurrentGamemode.TeamBased && lobbyPlayer.Team != myLobbyPlayer.Team)
                        continue;
                }
                _players.Add(pl.Key);
            }
            _players.Add(_clientController.LocalID);
        }

        private int GetCurrentSpectatingIndex()
        {
            for(var i = 0; i < _players.Count; i++)
            {
                var player = _players[i];
                if (player == CurrentSpectatingClient)
                    return i;
            }
            return 0;
        }

        public void EndSpectating()
        {
            var chatUi = ChatUI.Instance;
            if (chatUi != null)
                chatUi.SetState(ChatUI.States.Unfocused);
            SpectatorUI.Instance.Deactivate();
            if (_currentSpectatingPlayer != null)
            {
                _currentSpectatingPlayer.cam = null;
            }
            _uiManager.gameplay.gameObject.SetActive(true);
            _gameInput.DisableAllControllerMaps(0);
            _gameInput.EnableControllerMaps(BaseModule.IN_GAME_INPUT_MAPS, 0);
            if (SpectatorController.Instance != null)
                _gameInput.EnableControllerMaps(BaseModule.MENU_INPUT_MAPS, 0);
            var currentPlayer = _worldHandler.GetCurrentPlayer();
            currentPlayer.userInputEnabled = true;
            currentPlayer.cam = _gameplayCamera;
            currentPlayer.SetIgnoreCamInput(false);
            currentPlayer.motor._rigidbody.isKinematic = false;
            _gameplayCamera.SetPlayerToFollow(currentPlayer);
            Destroy(gameObject);
        }

        public static void StartSpectating(bool forced)
        {
            if (Instance != null)
            {
                if (forced)
                    Instance.Forced = true;
                return;
            }
            var go = new GameObject("Spectator Controller");
            Instance = go.AddComponent<SpectatorController>();
            Instance.Forced = forced;
        }
    }
}
