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
    public class Moon : MonoBehaviour
    {
        public float Distance = 50f;
#if PLUGIN
        private Transform _sun = null;
        private void Awake()
        {
            var amb = FindObjectOfType<AmbientManager>();
            if (amb != null)
            {
                var suntf = amb.transform.Find("SunSprite");
                if (suntf != null)
                    suntf.gameObject.SetActive(false);
                _sun = amb.transform;
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
        private void LateUpdate()
        {
            if (_sun == null) return;
            var cam = WorldHandler.instance.CurrentCameraTransform;
            if (cam == null) return;
            transform.position = cam.position - _sun.forward * Distance;
            transform.rotation = Quaternion.LookRotation(_sun.forward, Vector3.up);
        }
#endif
    }
}
