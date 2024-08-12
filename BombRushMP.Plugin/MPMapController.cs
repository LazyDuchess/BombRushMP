using Reptile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BombRushMP.Plugin
{
    public class MPMapController : MonoBehaviour
    {
        public static MPMapController Instance { get; private set; }
        public bool BeingDisplayed { get; private set; } = false;
        private MPMapUI _mapUI;
        private WorldHandler _worldHandler;
        private UIManager _uiManager;
        private Mapcontroller _mapController;
        private BaseModule _baseModule;
        private MPSettings _mpSettings;
        private ClientController _clientController;
        private bool _mapActive = false;

        private void Awake()
        {
            Instance = this;
            _worldHandler = WorldHandler.instance;
            _uiManager = Core.Instance.UIManager;
            _mapController = Mapcontroller.Instance;
            _baseModule = Core.Instance.BaseModule;
            _mpSettings = MPSettings.Instance;
            _clientController = ClientController.Instance;
            var mapUIGameObject = transform.Find("Canvas").Find("Map").gameObject;
            _mapUI = mapUIGameObject.AddComponent<MPMapUI>();
            _mapActive = _mapUI.gameObject.activeSelf;
        }

        private void Update()
        {
            var newMapActive = false;
            if (_mapController == null) return;
            if (ShouldDisplayMap())
            {
                newMapActive = true;
                if (!_mapController.m_Camera.enabled)
                    _mapController.m_Camera.enabled = true;
                if (!BeingDisplayed)
                {
                    _mapController.RestoreDefaultMapControllerState();
                }
                BeingDisplayed = true;
            }
            else
            {
                newMapActive = false;
                BeingDisplayed = false;
            }
            if (newMapActive != _mapActive)
            {
                _mapActive = newMapActive;
                _mapUI.gameObject.SetActive(_mapActive);
            }
        }

        private MinimapOverrideModes GetCurrentMinimapOverrideMode()
        {
            if (_clientController == null)
                return MinimapOverrideModes.None;
            var currentLobby = _clientController.ClientLobbyManager.CurrentLobby;
            if (currentLobby == null)
                return MinimapOverrideModes.None;
            if (!currentLobby.InGame)
                return MinimapOverrideModes.None;
            return currentLobby.CurrentGamemode.MinimapOverrideMode;
        }

        private bool ShouldDisplayMap()
        {
            if (_baseModule.IsInGamePaused)
                return false;

            if (!_uiManager.gameplay.gameplayScreen.gameObject.activeInHierarchy)
                return false;

            var player = _worldHandler.GetCurrentPlayer();

            if (player.phone.state != Reptile.Phone.Phone.PhoneState.OFF && player.phone.state != Reptile.Phone.Phone.PhoneState.SHUTTINGDOWN)
                return false;

            var currentMinimapOverrideMode = GetCurrentMinimapOverrideMode();

            if (currentMinimapOverrideMode == MinimapOverrideModes.ForceOn)
                return true;
            else if (currentMinimapOverrideMode == MinimapOverrideModes.ForceOff)
                return false;

            if (!_mpSettings.ShowMinimap)
                return false;

            return true;
        }

        public static void Create()
        {
            if (Instance != null)
                Destroy(Instance.gameObject);
            var mpAssets = MPAssets.Instance;
            var prefab = mpAssets.Bundle.LoadAsset<GameObject>("Map UI");
            var lobbyUi = Instantiate(prefab);
            lobbyUi.transform.SetParent(Core.Instance.UIManager.transform, false);
            lobbyUi.AddComponent<MPMapController>();
        }
    }
}
