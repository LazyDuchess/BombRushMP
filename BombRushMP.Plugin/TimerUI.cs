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
    public class TimerUI : MonoBehaviour
    {
        public static TimerUI Instance { get; private set; }
        private GameObject _canvas;
        private TextMeshProUGUI _label;
        private void Awake()
        {
            Instance = this;
            _canvas = transform.Find("Canvas").gameObject;
            _label = _canvas.transform.Find("Timer").GetComponent<TextMeshProUGUI>();
            Deactivate();
        }

        public void Activate()
        {
            StopAllCoroutines();
            _canvas.SetActive(true);
        }

        public void Deactivate()
        {
            StopAllCoroutines();
            _canvas.SetActive(false);
        }

        public void DeactivateDelayed()
        {
            StartCoroutine(DeactivateCoroutine());
        }

        private IEnumerator DeactivateCoroutine()
        {
            yield return new WaitForSeconds(3f);
            Deactivate();
        }

        public void SetTime(float time)
        {
            var str = time.ToString(CultureInfo.CurrentCulture);
            var startIndex = ((int)time).ToString().Length + 3;
            if (str.Length > startIndex) str = str.Remove(startIndex);
            if (time == 0.0) str = "0.00";
            SetText(str);
        }

        public void SetText(string text)
        {
            _label.text = text;
        }

        public static void Create()
        {
            if (Instance != null)
                Destroy(Instance.gameObject);
            var mpAssets = MPAssets.Instance;
            var prefab = mpAssets.Bundle.LoadAsset<GameObject>("Timer UI");
            var timerUi = Instantiate(prefab);
            timerUi.transform.SetParent(Core.Instance.UIManager.transform, false);
            timerUi.AddComponent<TimerUI>();
        }
    }
}
