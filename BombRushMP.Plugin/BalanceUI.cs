using Reptile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BombRushMP.Plugin
{
    public class BalanceUI : MonoBehaviour
    {
        public static BalanceUI Instance { get; private set; }
        private GameObject _grindUI;
        private Image _grindUIBG;
        private GameObject _grindUIIndicator;
        private GameObject _manualUI;
        private Image _manualUIBG;
        private GameObject _manualUIIndicator;
        private void Awake()
        {
            Instance = this;
            var canvas = transform.Find("Canvas");
            _grindUI = canvas.Find("Grind Balance UI").gameObject;
            _manualUI = canvas.Find("Manual Balance UI").gameObject;
            _grindUIBG = _grindUI.transform.Find("Image").GetComponent<Image>();
            _grindUIIndicator = _grindUI.transform.Find("Indicator").gameObject;
            _manualUIBG = _manualUI.transform.Find("Image").GetComponent<Image>();
            _manualUIIndicator = _manualUI.transform.Find("Indicator").gameObject;
            _manualUI.SetActive(false);
            _grindUI.SetActive(false);
        }

        private void Update()
        {
            var uiManager = Core.Instance.UIManager;
            if (uiManager.gameplay == null) return;
            if (uiManager.gameplay.gameplayScreen == null) return;
            if (!uiManager.gameplay.gameplayScreen.gameObject.activeInHierarchy)
            {
                _grindUI.SetActive(false);
                _manualUI.SetActive(false);
                return;
            }
            var worldHandler = WorldHandler.instance;
            if (worldHandler == null) return;
            var player = worldHandler.GetCurrentPlayer();
            if (player == null) return;
            var proSkater = ProSkaterPlayer.Get(player);
            if (proSkater == null)
            {
                _grindUI.SetActive(false);
                _manualUI.SetActive(false);
                return;
            }

            if (!proSkater.DidGrind)
                _grindUI.SetActive(false);
            else
            {
                _grindUI.SetActive(true);
                var col = _grindUIBG.color;
                col.a = 0.5f;
                if (player.ability != null && (player.ability is GrindAbility || player.ability is HandplantAbility))
                    col.a = 1f;
                _grindUIBG.color = col;
                _grindUIIndicator.transform.localRotation = Quaternion.Euler(0f, 0f, -proSkater.GrindBalance.Current * 50f);
            }

            if (!proSkater.DidManual)
                _manualUI.SetActive(false);
            else
            {
                _manualUI.SetActive(true);
                var col = _manualUIBG.color;
                col.a = 0.5f;
                if (proSkater.IsOnManual())
                    col.a = 1f;
                _manualUIBG.color = col;
                _manualUIIndicator.transform.localRotation = Quaternion.Euler(0f, 0f, -proSkater.ManualBalance.Current * 50f);
            }
        }

        public static void Create()
        {
            if (Instance != null)
                Destroy(Instance.gameObject);
            var mpAssets = MPAssets.Instance;
            var prefab = mpAssets.Bundle.LoadAsset<GameObject>("Balance UI");
            var balanceUi = Instantiate(prefab);
            balanceUi.transform.SetParent(Core.Instance.UIManager.transform, false);
            balanceUi.AddComponent<BalanceUI>();
        }
    }
}
