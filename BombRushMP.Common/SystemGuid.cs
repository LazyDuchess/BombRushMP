// Source - https://stackoverflow.com/a/71294403
// Posted by user2029101
// Retrieved 2026-02-16, License - CC BY-SA 4.0

using System;
using System.Collections.Generic;
using System.Text;
using System.Management;
using System.Security.Cryptography;

namespace BombRushMP.Common
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
                var lCpuId = GetCpuId();
                var lBiodId = GetBiosId();
                var lMainboard = GetMainboardId();
                var lGpuId = GetGpuId();
                var lMac = GetMac();
                var lConcatStr = $"CPU: {lCpuId}\nBIOS:{lBiodId}\nMainboard: {lMainboard}\nGPU: {lGpuId}\nMAC: {lMac}";
                _systemGuid = GetHash(lConcatStr);
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

        #region Original Device ID Getting Code

        //Return a hardware identifier
        private static string GetIdentifier(string pWmiClass, List<string> pProperties)
        {
            string lResult = string.Empty;
            try
            {
                foreach (ManagementObject lItem in new ManagementClass(pWmiClass).GetInstances())
                {
                    foreach (var lProperty in pProperties)
                    {
                        try
                        {
                            switch (lProperty)
                            {
                                case "MACAddress":
                                    if (string.IsNullOrWhiteSpace(lResult) == false)
                                        return lResult; //Return just the first MAC

                                    if (lItem["IPEnabled"].ToString() != "True")
                                        continue;
                                    break;
                            }

                            var lItemProperty = lItem[lProperty];
                            if (lItemProperty == null)
                                continue;

                            var lValue = lItemProperty.ToString();
                            if (string.IsNullOrWhiteSpace(lValue) == false)
                                lResult += $"{lValue}; ";
                        }
                        catch { }
                    }

                }
            }
            catch { }
            return lResult.TrimEnd(' ', ';');
        }

        private static List<string> ListOfCpuProperties = new List<string> { "UniqueId", "ProcessorId", "Name", "Manufacturer" };

        private static string GetCpuId()
        {
            return GetIdentifier("Win32_Processor", ListOfCpuProperties);
        }

        private static List<string> ListOfBiosProperties = new List<string> { "Manufacturer", "SMBIOSBIOSVersion", "IdentificationCode", "SerialNumber", "ReleaseDate", "Version" };
        //BIOS Identifier
        private static string GetBiosId()
        {
            return GetIdentifier("Win32_BIOS", ListOfBiosProperties);
        }

        private static List<string> ListOfMainboardProperties = new List<string> { "Model", "Manufacturer", "Name", "SerialNumber" };
        //Motherboard ID
        private static string GetMainboardId()
        {
            return GetIdentifier("Win32_BaseBoard", ListOfMainboardProperties);
        }

        private static List<string> ListOfGpuProperties = new List<string> { "Name" };
        //Primary video controller ID
        private static string GetGpuId()
        {
            return GetIdentifier("Win32_VideoController", ListOfGpuProperties);
        }

        private static List<string> ListOfNetworkProperties = new List<string> { "MACAddress" };
        private static string GetMac()
        {
            return GetIdentifier("Win32_NetworkAdapterConfiguration", ListOfNetworkProperties);
        }

        #endregion
    }
}
