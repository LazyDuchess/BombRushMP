// Source - https://stackoverflow.com/a/71294403
// Posted by user2029101
// Retrieved 2026-02-16, License - CC BY-SA 4.0

using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using UnityEngine;

namespace BombRushMP.Plugin
{
    /// <summary>
    /// Generates a Guid based on the current computer hardware
    /// Example: C384B159-8E36-6C85-8ED8-6897486500FF
    /// </summary>
    public class SystemGuid
    {
        private static string _systemGuid = string.Empty;
        public static string Value()
        {
            if (string.IsNullOrEmpty(_systemGuid))
            {
                var raw =
            SystemInfo.deviceUniqueIdentifier +
            SystemInfo.processorType +
            SystemInfo.graphicsDeviceName +
            SystemInfo.operatingSystem;
                _systemGuid = GetHash(raw);
            }
            return _systemGuid;
        }
        private static string GetHash(string s)
        {
            try
            {
                var lProvider = new MD5CryptoServiceProvider();
                var lUtf8 = lProvider.ComputeHash(ASCIIEncoding.UTF8.GetBytes(s));
                return new Guid(lUtf8).ToString().ToUpper();
            }
            catch (Exception lEx)
            {
                return lEx.Message;
            }
        }
    }
}
