using Reptile;
using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace BombRushMP.Plugin
{
    public class StatsUI : MonoBehaviour
    {
        public static StatsUI Instance { get; private set; }
        private GameObject _canvas;
        private TextMeshProUGUI _statsLabel;
        private void Awake()
        {
            Instance = this;
            _canvas = transform.Find("Canvas").gameObject;
            _statsLabel = _canvas.transform.Find("Stats").GetComponent<TextMeshProUGUI>();
            Deactivate();
        }

        public void Activate()
        {
            _canvas.SetActive(true);
        }

        public void Deactivate()
        {
            _canvas.SetActive(false);
        }

        public static void Create()
        {
            if (Instance != null)
                Destroy(Instance.gameObject);
            var mpAssets = MPAssets.Instance;
            var prefab = mpAssets.Bundle.LoadAsset<GameObject>("Stats UI");
            var statsUi = Instantiate(prefab);
            statsUi.transform.SetParent(Core.Instance.UIManager.transform, false);
            statsUi.AddComponent<StatsUI>();
        }
    }
}
