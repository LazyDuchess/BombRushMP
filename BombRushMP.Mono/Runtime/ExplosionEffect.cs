using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BombRushMP.Mono.Runtime
{
    public class ExplosionEffect : MonoBehaviour
    {
        public float Lifetime = 0.8f;
        private float _timer = 0f;

        private void Update()
        {
            if (_timer >= Lifetime)
            {
                Destroy(gameObject);
            }
            _timer += Time.deltaTime;
        }
    }
}
