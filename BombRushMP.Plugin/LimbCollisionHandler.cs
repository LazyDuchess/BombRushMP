using Reptile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BombRushMP.Plugin
{
    public class LimbCollisionHandler : MonoBehaviour
    {
        public Limb Owner;
        private Vector3 _previousVelocity = Vector3.zero;
        private Vector3 _previousPosition = Vector3.zero;

        public void OnActivation()
        {
            _previousPosition = Owner.RigidBody.position;
        }

        private void FixedUpdate()
        {
            if (Owner == null) return;
            _previousVelocity = Owner.RigidBody.velocity;
            if (Owner.DoCustomContinuousDetection)
            {
                var currentPosition = Owner.RigidBody.position;
                var diff = currentPosition - _previousPosition;
                var diffMag = diff.magnitude;
                if (diffMag > float.Epsilon)
                {
                    var ray = new Ray(_previousPosition, diff.normalized);
                    if (Physics.Raycast(ray, out var hit, diffMag, (1 << Layers.Default) | (1 << Layers.Junk) | (1 << Layers.NonStableSurface) | (1 << Layers.Wallrun) | (1 << Layers.VertSurface) | (1 << Layers.Water), QueryTriggerInteraction.Ignore))
                    {
                        Owner.RigidBody.MovePosition(_previousPosition);
                        var norm = hit.normal;
                        var projection = Vector3.Project(Owner.RigidBody.velocity, norm);
                        Owner.RigidBody.velocity -= projection;
                    }
                }
                _previousPosition = Owner.RigidBody.position;
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            if (Owner == null) return;
            if (!Owner.Active) return;

            var impactVelocity = _previousVelocity.magnitude;
            var breakable = other.gameObject.GetComponent<BreakableObject>();
            if (breakable != null && impactVelocity >= 5f)
            {
                breakable.Break(false);
                return;
            }

            if (other.gameObject.layer == Layers.Junk && impactVelocity >= 5f)
            {
                var junkHolder = other.gameObject.GetComponentInParent<JunkHolder>();
                if (junkHolder != null)
                {
                    junkHolder.FallApart(other.contacts[0].point, false);
                    return;
                }
                var junk = other.gameObject.GetComponent<Junk>();
                if (junk)
                {
                    if (junk.interactOn == Junk.Interact.ON_HITBOX)
                        junk.FallApart(false);
                    else
                        junk.FallApart(true);
                }
            }
        }
    }
}
