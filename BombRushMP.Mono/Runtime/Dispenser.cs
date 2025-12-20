using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BombRushMP.Mono.Runtime
{
    public class Dispenser : MonoBehaviour
    {
        public Tnt Prefab;
        public float ForwardForce = 1f;
        public float UpForce = 0.5f;
#if PLUGIN
        public void Trigger()
        {
            var instance = Instantiate(Prefab);
            instance.transform.position = transform.position + transform.forward;
            instance.ApplyForce((transform.forward * ForwardForce) + (Vector3.up * UpForce));
        }
#endif
    }
}
