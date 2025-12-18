using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
#if PLUGIN
using Reptile;
#endif

namespace BombRushMP.Mono.Runtime
{
    public class TrapTrigger : MonoBehaviour
    {
        public AudioSource ClickSFX;
        public float PushInDistance = 0.1f;
        private GameObject _plateMesh;
        private float _triggerTimer = 0f;
        private Vector3 _initialPos;

#if PLUGIN
        private void Awake()
        {
            _plateMesh = transform.parent.gameObject;
            _initialPos = _plateMesh.transform.position;
        }

        private void Update()
        {
            if (_triggerTimer > 0f)
            {
                _plateMesh.transform.position = _initialPos + (PushInDistance * Vector3.down);
            }
            else
            {
                _plateMesh.transform.position = _initialPos;
            }
            _triggerTimer -= Time.deltaTime;
            if (_triggerTimer < 0f)
                _triggerTimer = 0f;
        }

        private void OnTriggerStay(Collider other)
        {
            var player = other.GetComponentInParent<Player>();
            if (player == null) return;
            if (player.isAI) return;
            if (!player.IsGrounded()) return;
            if (_triggerTimer <= 0f)
                Fire();
            _triggerTimer = 0.1f;
            
        }

        private void Fire()
        {
            ClickSFX.Play();
        }
#endif
    }
}
