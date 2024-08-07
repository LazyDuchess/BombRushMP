using System;
using UnityEngine;

namespace BombRushMP.Plugin
{
    public static class ClientLogger
    {
        public static void Log(string message)
        {
            Debug.Log($"[{DateTime.Now.ToShortTimeString()}] {message}");
        }
    }
}
