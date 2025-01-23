using Reptile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace BombRushMP.Plugin
{
    public class PlayerListUI : MonoBehaviour
    {
        public static PlayerListUI Instance { get; private set; }
        public bool Displaying
        {
            get
            {
                return _window.activeSelf;
            }

            set
            {
                if (Displaying == value) return;
                if (!value)
                {
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                    var gameInput = Core.Instance.GameInput;
                    gameInput.DisableAllControllerMaps();
                    gameInput.EnableControllerMaps(BaseModule.IN_GAME_INPUT_MAPS);
                    if (SpectatorController.Instance != null)
                        gameInput.EnableControllerMaps(BaseModule.MENU_INPUT_MAPS);
                }
                else
                {
                    var gameInput = Core.Instance.GameInput;
                    gameInput.DisableAllControllerMaps();
                }
                _window.SetActive(value);
            }
        }
        private GameObject _playerReferenceObject;
        private GameObject _window;
        private ScrollRect _scrollRect;
        private List<PlayerInListUI> _playerUIPool = new();
        private List<PlayerInListUI> _playerUIInUse = new();

        private void Awake()
        {
            Instance = this;
            var canvas = transform.Find("Canvas");
            _window = canvas.Find("Player List Window").gameObject;
            var scrollView = _window.transform.Find("Scroll View");
            _scrollRect = scrollView.gameObject.GetComponent<ScrollRect>();
            _playerReferenceObject = scrollView.Find("Viewport").Find("Content").Find("Player").gameObject;
            _playerReferenceObject.SetActive(false);
            Displaying = false;
            ClientController.ClientStatesUpdate += UpdatePlayers;
            ClientController.ServerDisconnect += UpdatePlayers;
        }

        private void OnDestroy()
        {
            ClientController.ClientStatesUpdate -= UpdatePlayers;
            ClientController.ServerDisconnect -= UpdatePlayers;
        }

        private void Update()
        {
            if (Displaying)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                InputUtils.Override = true;
                try
                {
                    if (Input.GetKeyDown(MPSettings.Instance.PlayerListKey) || Input.GetKeyDown(KeyCode.Escape))
                    {
                        Displaying = false;
                    }
                }
                finally
                {
                    InputUtils.Override = false;
                }
                if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
                {
                    if (!RectTransformUtility.RectangleContainsScreenPoint(_window.RectTransform(), Input.mousePosition))
                    {
                        Displaying = false;
                    }
                }
            }
            else
            {
                if (Input.GetKeyDown(MPSettings.Instance.PlayerListKey) && CanOpen())
                {
                    Displaying = true;
                    UpdatePlayers();
                }
            }
        }

        private bool CanOpen()
        {
            var chatUI = ChatUI.Instance;
            if (chatUI.State == ChatUI.States.Focused) return false;
            var gameInput = Core.Instance.gameInput;
            var enabledMaps = gameInput.GetAllCurrentEnabledControllerMapCategoryIDs(0);
            return enabledMaps.controllerMapCategoryIDs.Contains(0) && enabledMaps.controllerMapCategoryIDs.Contains(6) && !Core.Instance.IsCorePaused;
        }

        private void UpdatePlayers()
        {
            if (!Displaying) return;
            foreach(var player in _playerUIInUse)
            {
                PutInPlayerPool(player);
            }
            _playerUIInUse.Clear();
            var players = ClientController.Instance.Players.Values.OrderBy((k) => k.ClientId);
            foreach(var player in players)
            {
                var instance = CreateOrGetFromPlayerPool();
                instance.SetPlayer(player);
                instance.transform.SetAsFirstSibling();
            }
        }

        private PlayerInListUI CreateOrGetFromPlayerPool()
        {
            PlayerInListUI result = null;
            if (_playerUIPool.Count > 0)
            {
                result = _playerUIPool[0];
                result.gameObject.SetActive(true);
                _playerUIPool.RemoveAt(0);
                _playerUIInUse.Add(result);
                return result;
            }
            result = CreatePlayerUI();
            _playerUIInUse.Add(result);
            return result;
        }

        private void PutInPlayerPool(PlayerInListUI ui)
        {
            ui.gameObject.SetActive(false);
            _playerUIPool.Add(ui);
        }

        private PlayerInListUI CreatePlayerUI()
        {
            var ui = PlayerInListUI.Create(_playerReferenceObject);
            ui.transform.SetParent(_scrollRect.content, false);
            return ui;
        }

        public static void Create()
        {
            if (Instance != null)
                Destroy(Instance.gameObject);
            var mpAssets = MPAssets.Instance;
            var prefab = mpAssets.Bundle.LoadAsset<GameObject>("Player List UI");
            var listUI = Instantiate(prefab);
            listUI.transform.SetParent(Core.Instance.UIManager.transform, false);
            listUI.AddComponent<PlayerListUI>();
        }
    }
}
