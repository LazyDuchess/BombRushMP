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

        public static Vector3 GetUnityPosition(this ClientVisualState clientVisualState)
        {
            return new Vector3(clientVisualState.Position.X, clientVisualState.Position.Y, clientVisualState.Position.Z);
        }

        public static void SetUnityRotation(this ClientVisualState clientVisualState, Quaternion rotation)
        {
            clientVisualState.Rotation = new System.Numerics.Quaternion(rotation.x, rotation.y, rotation.z, rotation.w);
        }

        public static void SetUnityPosition(this ClientVisualState clientVisualState, Vector3 position)
        {
            clientVisualState.Position = new System.Numerics.Vector3(position.x, position.y, position.z);
        }
    }
}
