using Reptile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace BombRushMP.Plugin
{
    public class Limb
    {
        public enum LimbTypes
        {
            Root,
            Spine,
            Head,
            Arm,
            Leg
        }
        public Transform Transform { get; set; }
        public Rigidbody RigidBody { get; set; }
        public Collider Collider { get; set; }
        public LimbTypes LimbType { get; set; }

        public bool Active { get; set; } = false;
        public void Activate(Vector3 velocity)
        {
            if (Active) return;
            if (RigidBody == null) return;
            Active = true;
            RigidBody.isKinematic = false;
            Collider.enabled = true;
            Transform.gameObject.layer = Layers.Enemies;
            RigidBody.velocity = velocity;
        }

        public void Deactivate()
        {
            if (!Active) return;
            if (RigidBody == null) return;
            Active = false;
            RigidBody.isKinematic = true;
            Collider.enabled = false;
            Transform.gameObject.layer = Layers.Default;
        }

        public void HackUpdateCollider()
        {
            Collider.enabled = Active;
        }

        public Limb(Transform tf, LimbTypes type, Limb parent = null)
        {
            Transform = tf;
            RigidBody = tf.gameObject.AddComponent<Rigidbody>();
            RigidBody.interpolation = RigidbodyInterpolation.Interpolate;
            RigidBody.collisionDetectionMode = CollisionDetectionMode.Continuous;
            LimbType = type;
            RigidBody.isKinematic = true;
            CharacterJoint joint = null;
            if (parent != null)
            {
                joint = tf.gameObject.AddComponent<CharacterJoint>();
                joint.connectedBody = parent.RigidBody;
                joint.enablePreprocessing = false;
                joint.enableProjection = true;
                joint.autoConfigureConnectedAnchor = true;
            }
            switch (type)
            {
                case LimbTypes.Root:
                    {
                        var box = tf.gameObject.AddComponent<BoxCollider>();
                        box.size = new Vector3(0.17f, 0.18f, 0.16f);
                        Collider = box;
                        RigidBody.mass = 1f;
                    }
                    break;
                case LimbTypes.Spine:
                    {
                        var box = tf.gameObject.AddComponent<BoxCollider>();
                        box.size = new Vector3(0.17f, 0.18f, 0.16f);
                        Collider = box;
                        RigidBody.mass = 0.5f;
                        if (parent != null)
                        {
                            joint.axis = new Vector3(0f, 1f, 0f);
                            joint.swingAxis = new Vector3(0f, 0f, 0f);
                            joint.lowTwistLimit = new SoftJointLimit() { limit = -30f };
                            joint.highTwistLimit = new SoftJointLimit() { limit = 15f };
                            joint.swing1Limit = new SoftJointLimit() { limit = 20f };
                            joint.swing2Limit = new SoftJointLimit() { limit = 20f };
                        }
                    }
                    break;
                case LimbTypes.Head:
                    {
                        var box = tf.gameObject.AddComponent<BoxCollider>();
                        box.center = new Vector3(-0.06f, 0f, 0f);
                        box.size = new Vector3(0.22f, 0.21f, 0.22f);
                        Collider = box;
                        RigidBody.mass = 0.5f;
                        if (parent != null)
                        {
                            joint.lowTwistLimit = new SoftJointLimit() { limit = -60f };
                            joint.highTwistLimit = new SoftJointLimit() { limit = 60f };
                            joint.swing1Limit = new SoftJointLimit() { limit = 30f };
                            joint.swing2Limit = new SoftJointLimit() { limit = 20f };
                        }
                    }
                    break;
                case LimbTypes.Arm:
                    {
                        var cap = tf.gameObject.AddComponent<CapsuleCollider>();
                        cap.center = new Vector3(-0.13f, 0f, 0f);
                        cap.radius = 0.06f;
                        cap.height = 0.3f;
                        cap.direction = 0;
                        Collider = cap;
                        RigidBody.mass = 0.25f;
                        if (parent != null)
                        {
                            if (parent.LimbType == LimbTypes.Arm)
                            {
                                joint.axis = new Vector3(0f, 1f, 0f);
                                joint.swingAxis = new Vector3(0f, 1f, 0f);
                                joint.lowTwistLimit = new SoftJointLimit() { limit = -60f };
                                joint.highTwistLimit = new SoftJointLimit() { limit = 0f };
                                joint.swing1Limit = new SoftJointLimit() { limit = 20f };
                                joint.swing2Limit = new SoftJointLimit() { limit = 0f };
                            }
                            else
                            {
                                joint.axis = new Vector3(0f, 0f, 1f);
                                joint.swingAxis = new Vector3(1f, 0f, 0f);
                                joint.lowTwistLimit = new SoftJointLimit() { limit = -80f };
                                joint.highTwistLimit = new SoftJointLimit() { limit = 80f };
                                joint.swing1Limit = new SoftJointLimit() { limit = 40f };
                                joint.swing2Limit = new SoftJointLimit() { limit = 90f };
                            }
                        }
                    }
                    break;
                case LimbTypes.Leg:
                    {
                        var cap = tf.gameObject.AddComponent<CapsuleCollider>();
                        cap.center = new Vector3(-0.25f, 0f, 0f);
                        cap.radius = 0.09f;
                        cap.height = 0.51f;
                        cap.direction = 0;
                        Collider = cap;
                        RigidBody.mass = 0.3f;
                        if (parent != null)
                        {
                            if (parent.LimbType == LimbTypes.Leg)
                            {
                                joint.axis = new Vector3(0f, 1f, 0f);
                                joint.swingAxis = new Vector3(0f, 0f, 0f);
                                joint.lowTwistLimit = new SoftJointLimit() { limit = 0f };
                                joint.highTwistLimit = new SoftJointLimit() { limit = 140f };
                                joint.swing1Limit = new SoftJointLimit() { limit = 10f };
                                joint.swing2Limit = new SoftJointLimit() { limit = 0f };
                            }
                            else
                            {
                                joint.axis = new Vector3(0f, 1f, 0f);
                                joint.swingAxis = new Vector3(0f, 0f, 0f);
                                joint.lowTwistLimit = new SoftJointLimit() { limit = -80f };
                                joint.highTwistLimit = new SoftJointLimit() { limit = 15f };
                                joint.swing1Limit = new SoftJointLimit() { limit = 10f };
                                joint.swing2Limit = new SoftJointLimit() { limit = 40f };
                            }
                        }
                    }
                    break;
            }
            if (Collider != null)
                Collider.enabled = false;
        }
    }
}
