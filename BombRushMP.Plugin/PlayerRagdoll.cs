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
        public const string RagdollEventLaunchPacketId = "ACN-RAGDOLL_LAUNCH";

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

        private List<SkinnedMeshRenderer> _renderersToForceRender = new();
        private bool _shouldForceRender = false;

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
            if (_shouldForceRender)
            {
                foreach (var skin in _renderersToForceRender)
                {
                    skin.updateWhenOffscreen = true;
                }
            }
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
            Owner.Player.usingEquippedMovestyle = false;
            Owner.Player.SetMoveStyle(MoveStyle.ON_FOOT, true, false);
            Owner.Player.isDisabled = true;
            if (Owner.Player.phone != null)
                Owner.Player.phone.TurnOff(false);
            Owner.Player.StopHoldProps();
            Owner.Player.SetDustEmission(0);
            Owner.Player.SetBoostpackAndFrictionEffects(BoostpackEffectMode.OFF, FrictionEffectMode.OFF);
            Visual.VFX.boostpackTrail.SetActive(false);

            foreach (var limb in Limbs)
            {
                limb.Activate(vel);
            }

            ApplyForceToRagdoll(args.Force, args.ForcePosition, args.FixedForce);

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

        public void ApplyForceToRagdoll(float force, Vector3 point, Vector3 fixedForce)
        {
            if (!Valid) return;
            if (!Active) return;

            Limb closestLimb = null;
            var closestLimbDist = 0f;
            var hitDirection = Vector3.zero;

            foreach (var limb in Limbs)
            {
                var dist = Vector2.Distance(limb.Transform.position, point);
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

            hitDirection = (closestLimb.Transform.position - point).normalized;

            closestLimb.RigidBody.velocity += (hitDirection * force) + fixedForce;
        }

        public void StopRagdoll()
        {
            if (!Valid) return;
            if (!Active) return;
            if (_shouldForceRender)
            {
                foreach(var skin in _renderersToForceRender)
                {
                    skin.updateWhenOffscreen = false;
                }
            }
            Active = false;
            Owner.Player.usingEquippedMovestyle = false;
            Owner.Player.SetMoveStyle(MoveStyle.ON_FOOT, true, false);
            Visual.SetMoveStyleVisualAnim(Owner.Player, MoveStyle.ON_FOOT);
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
            Owner.Player.ResetVisualRot();
            Owner.Player.visualTf.position = Owner.Player.transform.position;

            var clientController = ClientController.Instance;
            if (Owner.Local)
            {
                Owner.Player.PlayAnim(Owner.Player.landHash, true, true);
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

            ClientController.RegisterCustomPacketHandler(RagdollEventLaunchPacketId, (player, data) =>
            {
                var clientController = ClientController.Instance;
                if (!clientController.Players.TryGetValue(player, out var mpPlayer)) return;
                if (!mpPlayer.ClientState.User.IsModerator) return;
                using var ms = new MemoryStream(data);
                using var reader = new BinaryReader(ms);
                var launch = new RagdollLaunchPacket();
                launch.Read(reader);
                var ragdoll = PlayerComponent.GetLocal().Ragdoll;
                if (!ragdoll.Active)
                    ragdoll.BecomeRagdoll(new Parameters(Modes.Hit, launch.Force, launch.Point, Vector3.up * launch.UpForce));
                else
                    ragdoll.ApplyForceToRagdoll(launch.Force, launch.Point, Vector3.up * launch.UpForce);
            });
        }

        private bool CanReachTransformParentChain(Transform target, Transform from)
        {
            var cur = from.parent;
            while (cur != null && cur != target)
            {
                cur = cur.parent;
            }
            if (cur == target) return true;
            return false;
        }

        private Transform FindCommonBone(Transform bone1, Transform bone2)
        {
            var cPar = bone1;
            while (cPar != null)
            {
                if (CanReachTransformParentChain(cPar, bone2))
                    return cPar;
                cPar = cPar.parent;
            }
            return cPar;
        }

        private Transform FindFirstChild(Transform from, Transform childOf)
        {
            var cur = from;
            var next = from.parent;
            while (next != null && next != childOf)
            {
                cur = next;
                next = cur.parent;
            }
            if (next == null) return null;
            return cur;
        }

        public void Initialize(PlayerComponent player)
        {
            if (Active)
                StopRagdoll();

            Owner = player;
            Valid = false;
            Visual = player.Player.characterVisual;
            Limbs.Clear();
            _shouldForceRender = false;
            _renderersToForceRender.Clear();

            try
            {
                var root = Visual.root;
                if (root == null) return;

                var leg2l = root.transform.FindRecursive("leg2l");
                if (leg2l == null) return;

                var head = root.transform.FindRecursive("head");
                if (head == null) return;

                var pelvis = FindCommonBone(leg2l, head);
                if (pelvis != root)
                {
                    _shouldForceRender = true;
                }
                if (pelvis == null) return;

                var curp = head;
                var nextp = head.parent;

                while (nextp != null && nextp != pelvis)
                {
                    curp = nextp;
                    nextp = curp.parent;
                }

                if (nextp == null) return;

                var s1 = curp;

                if (s1 == null) return;

                var handr = root.transform.FindRecursive("handr");
                var handl = root.transform.FindRecursive("handl");

                if (handr == null) return;
                if (handl == null) return;

                var s2 = FindCommonBone(handr, head);
                if (s2 == null) return;

                var leg2r = root.transform.FindRecursive("leg2r");
                if (leg2r == null) return;

                var leg1r = FindFirstChild(leg2r, pelvis);
                if (leg1r == null) return;

                var leg1l = FindFirstChild(leg2l, pelvis);
                if (leg1l == null) return;

                var shldr = FindFirstChild(handr, s2);
                var shldl = FindFirstChild(handl, s2);

                if (shldr == null) return;
                if (shldl == null) return;

                var arm2l = handl.parent;
                if (arm2l == null) return;

                var arm1l = FindFirstChild(arm2l, shldl);
                if (arm1l == null) return;

                var arm2r = handr.parent;
                if (arm2r == null) return;

                var arm1r = FindFirstChild(arm2r, shldr);
                if (arm1r == null) return;


                Valid = true;

                var rootLimb = new Limb(pelvis, Limb.LimbTypes.Root, this);

                var s1Limb = new Limb(s1, Limb.LimbTypes.Spine, this, rootLimb);
                var s2Limb = new Limb(s2, Limb.LimbTypes.Spine, this, s1Limb);

                var headLimb = new Limb(head, Limb.LimbTypes.Head, this, s2Limb);

                var leg1lLimb = new Limb(leg1l, Limb.LimbTypes.Leg, this, rootLimb);
                var leg2lLimb = new Limb(leg2l, Limb.LimbTypes.Leg, this, leg1lLimb);

                var leg1rLimb = new Limb(leg1r, Limb.LimbTypes.Leg, this, rootLimb);
                var leg2rLimb = new Limb(leg2r, Limb.LimbTypes.Leg, this, leg1rLimb);

                var arm1lLimb = new Limb(arm1l, Limb.LimbTypes.Arm, this, s2Limb);
                var arm2lLimb = new Limb(arm2l, Limb.LimbTypes.Arm, this, arm1lLimb);

                var arm1rLimb = new Limb(arm1r, Limb.LimbTypes.Arm, this, s2Limb);
                var arm2rLimb = new Limb(arm2r, Limb.LimbTypes.Arm, this, arm1rLimb);

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

                if (_shouldForceRender)
                {
                    var skins = Visual.GetComponentsInChildren<SkinnedMeshRenderer>();
                    foreach(var skin in skins)
                    {
                        if (!skin.updateWhenOffscreen)
                        {
                            _renderersToForceRender.Add(skin);
                        }
                    }
                }
            }
            catch(Exception e)
            {
                Debug.LogError($"FAILED TO CREATE RAGDOLL FOR {Visual.name}!!");
                Debug.LogException(e);
                Valid = false;
            }
        }
    }
}
