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
        public ushort CurrentSpectatingClient = 0;
        public static SpectatorController Instance { get; private set; }
        private ClientController _clientController = null;
        private List<MPPlayer> _players = null;
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
            if (playerList != null)
                playerList.Displaying = false;
            _uiManager = Core.instance.UIManager;
            _gameInput = Core.Instance.GameInput;
            _worldHandler = WorldHandler.instance;
            var currentPlayer = _worldHandler.GetCurrentPlayer();
            if (!currentPlayer.userInputEnabled)
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
            currentPlayer.CompletelyStop();
            currentPlayer.motor._rigidbody.isKinematic = true;
            _gameplayCamera = GameplayCamera.instance;
            _clientController = ClientController.Instance;
            CachePlayers();
            if (_players.Count == 0)
            {
                EndSpectating();
                return;
            }
            CurrentSpectatingClient = _players[0].ClientId;
            SpectatorUI.Instance.Activate();
        }
        
        public void SpectatePlayer(MPPlayer player)
        {
            var index = _players.IndexOf(player);
            if (index < 0)
                return;
            CurrentSpectatingClient = _players[index].ClientId;
        }

        private void Update()
        {
            CachePlayers();
            if (_players.Count == 0)
            {
                EndSpectating();
                return;
            }
            var targetPlayer = _players[GetCurrentSpectatingIndex()].Player;
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
            if (_gameInput.GetButtonNew(3, 0))
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
            CurrentSpectatingClient = _players[currentIndex].ClientId;
        }

        private void Previous()
        {
            var currentIndex = GetCurrentSpectatingIndex();
            currentIndex--;
            if (currentIndex < 0)
                currentIndex = _players.Count - 1;
            CurrentSpectatingClient = _players[currentIndex].ClientId;
        }

        private void CachePlayers()
        {
            _players = _clientController.Players.Values.Where((x) => x.Player != null).ToList();
        }

        private int GetCurrentSpectatingIndex()
        {
            for(var i = 0; i < _players.Count; i++)
            {
                var player = _players[i];
                if (player.ClientId == CurrentSpectatingClient)
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

        public static void StartSpectating()
        {
            if (Instance != null)
                return;
            var go = new GameObject("Spectator Controller");
            Instance = go.AddComponent<SpectatorController>();
        }
    }
}
