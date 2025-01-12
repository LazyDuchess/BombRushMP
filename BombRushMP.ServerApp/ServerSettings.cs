using BombRushMP.Common;
using BombRushMP.NetworkInterfaceProvider;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.ServerApp
{
    public class ServerSettings
    {
        [JsonIgnore]
        public NetworkInterfaces NetworkInterface
        {
            get
            {
                if (Enum.TryParse<NetworkInterfaces>(_networkInterface, out var result))
                    return result;
                return NetworkInterfaces.LiteNetLib;
            }

            set
            {
                _networkInterface = value.ToString();
            }
        }
        [JsonProperty("NetworkInterface")]
        private string _networkInterface = NetworkInterfaces.LiteNetLib.ToString();
        public int Port = 41585;
        public ushort MaxPlayers = 65534;
        public bool UseNativeSockets = true;
        public float TicksPerSecond = 1f/Constants.DefaultNetworkingTickRate;
        public bool LogChats = true;
        public bool LogChatsToFiles = false;
        public bool AllowNameChanges = false;
        public float ChatCooldown = 0.5f;
    }
}
