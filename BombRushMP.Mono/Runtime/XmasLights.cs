using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BombRushMP.Mono.Runtime
{
    public class XmasLights : MonoBehaviour
    {
        public GameObject Alt1;
        public GameObject Alt2;
        public float Interval = 1f;
        private float _timer = 0f;

        private void Awake()
        {
            Alt1.SetActive(true);
            Alt2.SetActive(false);
        }

        private void Update()
        {
            if (_timer >= Interval)
            {
                _timer = 0f;
                Toggle();
            }
            _timer += Time.deltaTime;
        }

        private void Toggle()
        {
            if (Alt1.activeSelf)
            {
                Alt1.SetActive(false);
                Alt2.SetActive(true);
            }
            else
            {
                Alt1.SetActive(true);
                Alt2.SetActive(false);
            }
        }
    }
}
