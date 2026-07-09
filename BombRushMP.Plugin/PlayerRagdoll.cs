using Reptile;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.LowLevel;

namespace BombRushMP.Plugin
{
    public class PlayerRagdoll
    {
        public struct Parameters
        {
            public Modes Mode;
            public Vector3 ForcePosition;
            public float Force;
            public Vector3 FixedForce;

            public Parameters(Modes mode)
            {
                Mode = mode;
                ForcePosition = Vector3.zero;
                Force = 0f;
            }

            public Parameters(Modes mode, float force, Vector3 forcePosition)
            {
                Mode = mode;
                ForcePosition = forcePosition;
                Force = force;
            }

            public Parameters(Modes mode, float force, Vector3 forcePosition, Vector3 fixedForce)
            {
                Mode = mode;
                ForcePosition = forcePosition;
                Force = force;
                FixedForce = fixedForce;
            }
        }

        public enum Modes
        {
            Hit,
            Manual
        }

        public const string RagdollEventPacketId = "ACN-RAGDOLL_EVENT";
        public const string RagdollStatePacketId = "ACN-RAGDOLL_STATE";

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

        public ulong CurrentEventPacketId = 0;
        public ulong CurrentStatePacketId = 0;

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

        public void HackUpdateColliders()
        {
            foreach(var limb in Limbs)
            {
                limb.HackUpdateCollider();
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
            if (Owner.Player.phone != null)
                Owner.Player.phone.TurnOff(false);
            Owner.Player.StopHoldProps();
            Owner.Player.SetDustEmission(0);
            Owner.Player.SetBoostpackAndFrictionEffects(BoostpackEffectMode.OFF, FrictionEffectMode.OFF);
            Visual.VFX.boostpackTrail.SetActive(false);

            Limb closestLimb = null;
            var closestLimbDist = 0f;
            var hitDirection = Vector3.zero;

            foreach(var limb in Limbs)
            {
                var dist = Vector2.Distance(limb.Transform.position, args.ForcePosition);
                if (closestLimb == null)
                {
                    closestLimb = limb;
                    closestLimbDist = dist;
                }
                else if (dist < closestLimbDist)
                {
                    closestLimb = limb;
                    closestLimbDist = dist;
                }
            }

            hitDirection = (closestLimb.Transform.position - args.ForcePosition).normalized;

            foreach (var limb in Limbs)
            {
                var limbVel = vel;
                if (closestLimb == limb)
                {
                    limbVel += hitDirection * args.Force + args.FixedForce;
                }
                limb.Activate(limbVel);
            }

            var clientController = ClientController.Instance;
            if (Owner.Local)
            {
                var limbRots = new List<Quaternion>();
                foreach(var limb in Limbs)
                {
                    limbRots.Add(limb.Transform.rotation);
                }
                var ragdollState = new RagdollState(limbRots, CurrentStatePacketId);
                var ragdollEv = new RagdollEvent(RagdollEvent.Events.Start, CurrentEventPacketId, ragdollState, args.Force, args.ForcePosition, args.FixedForce);
                CurrentEventPacketId++;
                using var ms = new MemoryStream();
                using var writer = new BinaryWriter(ms);
                ragdollEv.Write(writer);
                clientController.BroadcastCustomPacket(ms.ToArray(), RagdollEventPacketId, Common.Networking.IMessage.SendModes.ReliableUnordered);
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

            var clientController = ClientController.Instance;
            if (Owner.Local)
            {
                var ragdollEv = new RagdollEvent(RagdollEvent.Events.Stop, CurrentEventPacketId);
                CurrentEventPacketId++;
                using var ms = new MemoryStream();
                using var writer = new BinaryWriter(ms);
                ragdollEv.Write(writer);
                clientController.BroadcastCustomPacket(ms.ToArray(), RagdollEventPacketId, Common.Networking.IMessage.SendModes.ReliableUnordered);
            }
        }

        public void TickLocalNetworking()
        {
            if (!Active) return;
            var clientController = ClientController.Instance;
            var limbRots = new List<Quaternion>();
            foreach (var limb in Limbs)
            {
                limbRots.Add(limb.Transform.rotation);
            }
            var ragdollState = new RagdollState(limbRots, CurrentStatePacketId);
            using var ms = new MemoryStream();
            using var writer = new BinaryWriter(ms);
            ragdollState.Write(writer);
            clientController.BroadcastCustomPacket(ms.ToArray(), RagdollStatePacketId, Common.Networking.IMessage.SendModes.Unreliable);
            CurrentStatePacketId++;
        }

        

        public void UpdateRemoteNetworking()
        {
            if (!Valid) return;
            var mpPlayer = MPUtility.GetMuliplayerPlayer(Owner.Player);
            if (mpPlayer == null) return;
            if (mpPlayer.LatestRemoteEvent != null)
            {
                if (mpPlayer.LatestRemoteEvent.Event == RagdollEvent.Events.Start && !Active)
                {
                    mpPlayer.LatestRemoteRagdollState = mpPlayer.LatestRemoteEvent.State;
                    BecomeRagdoll(new Parameters(Modes.Manual, mpPlayer.LatestRemoteEvent.Force, mpPlayer.LatestRemoteEvent.ForcePosition, mpPlayer.LatestRemoteEvent.FixedForce));
                    SetRemoteStateNow();
                }
                else if (mpPlayer.LatestRemoteEvent.Event == RagdollEvent.Events.Stop && Active)
                {
                    StopRagdoll();
                }
            }
            if (!Active) return;
            Limbs[0].Transform.position = Owner.Player.transform.position;
            if (mpPlayer.LatestRemoteRagdollState == null) return;
            for(var i=0;i<Limbs.Count;i++)
            {
                var limb = Limbs[i];
                var limbState = mpPlayer.LatestRemoteRagdollState.LimbRotations[i];
                limb.Transform.rotation = Quaternion.Lerp(limb.Transform.rotation, limbState, Time.deltaTime * ClientConstants.PlayerInterpolation);
            }
        }

        public void SetRemoteStateNow()
        {
            if (!Valid) return;
            var mpPlayer = MPUtility.GetMuliplayerPlayer(Owner.Player);
            if (mpPlayer == null) return;
            if (mpPlayer.LatestRemoteRagdollState == null) return;
            Limbs[0].Transform.position = Owner.Player.transform.position;
            if (mpPlayer.LatestRemoteRagdollState == null) return;
            for (var i = 0; i < Limbs.Count; i++)
            {
                var limb = Limbs[i];
                var limbState = mpPlayer.LatestRemoteRagdollState.LimbRotations[i];
                limb.Transform.rotation = limbState;
            }
        }

        public static void InitializeStatic()
        {
            ClientController.RegisterCustomPacketHandler(RagdollEventPacketId, (player, data) =>
            {
                var clientController = ClientController.Instance;
                if (!clientController.Players.TryGetValue(player, out var mpPlayer)) return;
                using var ms = new MemoryStream(data);
                using var reader = new BinaryReader(ms);
                var ev = new RagdollEvent();
                ev.Read(reader);
                if (mpPlayer.LatestRemoteEvent == null)
                {
                    mpPlayer.LatestRemoteEvent = ev;
                }
                else if (ev.PacketId >= mpPlayer.LatestRemoteEvent.PacketId)
                {
                    mpPlayer.LatestRemoteEvent = ev;
                }
            });

            ClientController.RegisterCustomPacketHandler(RagdollStatePacketId, (player, data) =>
            {
                var clientController = ClientController.Instance;
                if (!clientController.Players.TryGetValue(player, out var mpPlayer)) return;
                using var ms = new MemoryStream(data);
                using var reader = new BinaryReader(ms);
                var state = new RagdollState();
                state.Read(reader);
                if (mpPlayer.LatestRemoteRagdollState == null)
                {
                    mpPlayer.LatestRemoteRagdollState = state;
                }
                else if (state.PacketId >= mpPlayer.LatestRemoteRagdollState.PacketId)
                {
                    mpPlayer.LatestRemoteRagdollState = state;
                }
            });
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
