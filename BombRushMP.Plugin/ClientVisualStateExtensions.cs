using BombRushMP.Common.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BombRushMP.Plugin
{
    public static class ClientVisualStateExtensions
    {
        public static Quaternion GetUnityRotation(this ClientVisualState clientVisualState)
        {
            return new Quaternion(clientVisualState.Rotation.X, clientVisualState.Rotation.Y, clientVisualState.Rotation.Z, clientVisualState.Rotation.W);
        }

        public static Quaternion GetUnityVisualRotation(this ClientVisualState clientVisualState)
        {
            return new Quaternion(clientVisualState.VisualRotation.X, clientVisualState.VisualRotation.Y, clientVisualState.VisualRotation.Z, clientVisualState.VisualRotation.W);
        }

        public static Vector3 GetUnityPosition(this ClientVisualState clientVisualState)
        {
            return new Vector3(clientVisualState.Position.X, clientVisualState.Position.Y, clientVisualState.Position.Z);
        }

        public static Vector3 GetUnityVisualPosition(this ClientVisualState clientVisualState)
        {
            return new Vector3(clientVisualState.VisualPosition.X, clientVisualState.VisualPosition.Y, clientVisualState.VisualPosition.Z);
        }

        public static Vector3 GetUnityVelocity(this ClientVisualState clientVisualState)
        {
            return new Vector3(clientVisualState.Velocity.X, clientVisualState.Velocity.Y, clientVisualState.Velocity.Z);
        }

        public static void SetUnityRotation(this ClientVisualState clientVisualState, Quaternion rotation)
        {
            clientVisualState.Rotation = new System.Numerics.Quaternion(rotation.x, rotation.y, rotation.z, rotation.w);
        }

        public static void SetUnityVisualRotation(this ClientVisualState clientVisualState, Quaternion rotation)
        {
            clientVisualState.VisualRotation = new System.Numerics.Quaternion(rotation.x, rotation.y, rotation.z, rotation.w);
        }

        public static void SetUnityPosition(this ClientVisualState clientVisualState, Vector3 position)
        {
            clientVisualState.Position = new System.Numerics.Vector3(position.x, position.y, position.z);
        }

        public static void SetUnityVisualPosition(this ClientVisualState clientVisualState, Vector3 position)
        {
            clientVisualState.VisualPosition = new System.Numerics.Vector3(position.x, position.y, position.z);
        }

        public static void SetUnityVeolcity(this ClientVisualState clientVisualState, Vector3 velocity)
        {
            clientVisualState.Velocity = new System.Numerics.Vector3(velocity.x, velocity.y, velocity.z);
        }
    }
}
