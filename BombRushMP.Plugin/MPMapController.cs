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
        private MPMapUI _mapUI;
        private WorldHandler _worldHandler;
        private UIManager _uiManager;
        private Mapcontroller _mapController;
        private BaseModule _baseModule;
        public bool BeingDisplayed { get; private set; } = false;

        private void Awake()
        {
            Instance = this;
            _worldHandler = WorldHandler.instance;
            _uiManager = Core.Instance.UIManager;
            _mapController = Mapcontroller.Instance;
            _baseModule = Core.Instance.BaseModule;
            var mapUIGameObject = transform.Find("Canvas").Find("Map").gameObject;
            _mapUI = mapUIGameObject.AddComponent<MPMapUI>();
        }

        private void Update()
        {
            if (_mapController == null) return;
            if (ShouldDisplayMap())
            {
                if (!_mapUI.gameObject.activeSelf)
                    _mapUI.gameObject.SetActive(true);
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
                if (_mapUI.gameObject.activeSelf)
                    _mapUI.gameObject.SetActive(false);
                BeingDisplayed = false;
            }
        }

        private bool ShouldDisplayMap()
        {
            if (Core.Instance.BaseModule.IsInGamePaused)
                return false;
            if (!_uiManager.gameplay.gameplayScreen.gameObject.activeInHierarchy)
                return false;
            var player = _worldHandler.GetCurrentPlayer();
            if (player.phone.state != Reptile.Phone.Phone.PhoneState.OFF && player.phone.state != Reptile.Phone.Phone.PhoneState.SHUTTINGDOWN)
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
