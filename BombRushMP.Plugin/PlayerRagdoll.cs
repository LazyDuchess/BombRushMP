using Reptile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BombRushMP.Plugin
{
    public class PlayerRagdoll
    {
        public struct Parameters
        {
            public Modes Mode;
            public Vector3 ForcePosition;
            public float ForceRadius;
            public float Force;

            public Parameters(Modes mode)
            {
                Mode = mode;
                ForcePosition = Vector3.zero;
                ForceRadius = 0f;
                Force = 0f;
            }

            public Parameters(Modes mode, float force, float forceRadius, Vector3 forcePosition)
            {
                Mode = mode;
                ForcePosition = forcePosition;
                ForceRadius = forceRadius;
                Force = force;
            }
        }

        public enum Modes
        {
            Hit,
            Manual
        }

        public const string RagdollDisallowedTag = "noragdoll";
        public const float RagdollMinimumTime = 1f;
        private const float MinTimeForStillness = 0.5f;
        private const float MaxVelocityForStillness = 0.5f;
        public PlayerComponent Owner { get; private set; } = null;
        public bool Valid { get; private set; } = false;
        public bool Active { get; private set; } = false;
        public Modes Mode { get; private set; } = Modes.Manual;
        public float Timer { get; private set; } = 0f;
        public bool Still { get; private set; } = false;
        private bool _still = false;
        private float _stillTimer = 0f;
        public CharacterVisual Visual { get; private set; } = null;
        public List<Limb> Limbs = new();

        public void OnDestroy()
        {
            if (Active)
            {
                Active = false;
                if (Visual != null)
                {
                    GameObject.Destroy(Visual.gameObject);
                }
            }
        }

        public void OnFixedUpdate()
        {
            Timer += Time.deltaTime;
            Still = false;
            if (Active)
            {
                if (Limbs[0].RigidBody.velocity.magnitude <= MaxVelocityForStillness)
                {
                    _still = true;
                    _stillTimer += Time.deltaTime;
                    if (_stillTimer >= MinTimeForStillness)
                    {
                        Still = true;
                    }
                }
                else
                {
                    _stillTimer = 0f;
                    _still = false;
                }
            }
            else
            {
                _stillTimer = 0f;
                _still = false;
            }
        }

        public void BecomeRagdoll(Parameters args)
        {
            if (!Valid) return;
            if (Active) return;
            var vel = Owner.Player.GetPracticalWorldVelocity();
            if (!Owner.Player.IsDead())
                Owner.Player.CompletelyStop();
            Active = true;
            Timer = 0f;
            _stillTimer = 0f;
            _still = false;
            Mode = args.Mode;
            Visual.transform.parent = null;
            Owner.Player.characterVisual.anim.enabled = false;
            Owner.Player.motor.HaveCollision(false);
            Owner.Player.motor.SetKinematic(true);
            Owner.Player.SwitchToEquippedMovestyle(false, false, true, false);
            Owner.Player.isDisabled = true;
            Owner.Player.phone.TurnOff(false);
            Owner.Player.StopHoldProps();
            Owner.Player.SetDustEmission(0);
            Owner.Player.SetBoostpackAndFrictionEffects(BoostpackEffectMode.OFF, FrictionEffectMode.OFF);
            Visual.VFX.boostpackTrail.SetActive(false);
            foreach (var limb in Limbs)
            {
                limb.Activate(vel);
            }
        }

        public void StopRagdoll()
        {
            if (!Valid) return;
            if (!Active) return;
            Active = false;
            Timer = 0f;
            Visual.transform.parent = Owner.Player.interactionCollider.transform.parent;
            Visual.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            Owner.Player.characterVisual.anim.enabled = true;
            Owner.Player.motor.HaveCollision(true);
            Owner.Player.motor.SetKinematic(false);
            Owner.Player.isDisabled = false;
            foreach (var limb in Limbs)
            {
                limb.Deactivate();
            }
            Owner.Player.OrientVisualInstantReset();
            Owner.Player.ActivateAbility(Owner.Player.slideAbility);
        }

        public void Initialize(PlayerComponent player)
        {
            if (Active)
                StopRagdoll();

            Owner = player;
            Valid = false;
            Visual = player.Player.characterVisual;
            Limbs.Clear();

            var root = Visual.root;
            if (root == null) return;

            var s1 = root.transform.FindRecursive("s1");
            if (s1 == null) return;
            var s2 = root.transform.FindRecursive("s2");
            if (s2 == null) return;

            var head = root.transform.FindRecursive("head");
            if (head == null) return;

            var leg1l = root.transform.FindRecursive("leg1l");
            if (leg1l == null) return;
            var leg2l = root.transform.FindRecursive("leg2l");
            if (leg2l == null) return;

            var leg1r = root.transform.FindRecursive("leg1r");
            if (leg1r == null) return;
            var leg2r = root.transform.FindRecursive("leg2r");
            if (leg2r == null) return;

            var arm1l = root.transform.FindRecursive("arm1l");
            if (arm1l == null) return;
            var arm2l = root.transform.FindRecursive("arm2l");
            if (arm2l == null) return;

            var arm1r = root.transform.FindRecursive("arm1r");
            if (arm1r == null) return;
            var arm2r = root.transform.FindRecursive("arm2r");
            if (arm2r == null) return;

            Valid = true;

            var rootLimb = new Limb(root, Limb.LimbTypes.Root);

            var s1Limb = new Limb(s1, Limb.LimbTypes.Spine, rootLimb);
            var s2Limb = new Limb(s2, Limb.LimbTypes.Spine, s1Limb);

            var headLimb = new Limb(head, Limb.LimbTypes.Head, s2Limb);

            var leg1lLimb = new Limb(leg1l, Limb.LimbTypes.Leg, rootLimb);
            var leg2lLimb = new Limb(leg2l, Limb.LimbTypes.Leg, leg1lLimb);

            var leg1rLimb = new Limb(leg1r, Limb.LimbTypes.Leg, rootLimb);
            var leg2rLimb = new Limb(leg2r, Limb.LimbTypes.Leg, leg1rLimb);

            var arm1lLimb = new Limb(arm1l, Limb.LimbTypes.Arm, s2Limb);
            var arm2lLimb = new Limb(arm2l, Limb.LimbTypes.Arm, arm1lLimb);

            var arm1rLimb = new Limb(arm1r, Limb.LimbTypes.Arm, s2Limb);
            var arm2rLimb = new Limb(arm2r, Limb.LimbTypes.Arm, arm1rLimb);

            Limbs = [
                rootLimb, 
                s1Limb, 
                s2Limb, 
                headLimb, 
                leg1lLimb, 
                leg2lLimb,
                leg1rLimb,
                leg2rLimb,
                arm1lLimb,
                arm2lLimb,
                arm1rLimb,
                arm2rLimb
            ];
        }
    }
}
