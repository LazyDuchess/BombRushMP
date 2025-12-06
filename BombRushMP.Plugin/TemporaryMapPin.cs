using Reptile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BombRushMP.Plugin
{
    public class TemporaryMapPin : MonoBehaviour
    {
        private float _timer = Mathf.Infinity;
        private MapPin _pin;

        private void Update()
        {
            _timer -= Time.deltaTime;
            if (_timer <= 0f)
                Remove();
        }

        private void Remove()
        {
            var mapController = Mapcontroller.Instance;
            mapController.m_MapPins.Remove(_pin);
            _pin.isMapPinValid = false;
            _pin.DisableMapPinGameObject();
            Destroy(_pin.gameObject);
        }

        public void Initialize(float timer, MapPin pin)
        {
            _timer = timer;
            _pin = pin;
        }
    }
}
