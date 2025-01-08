using BombRushMP.Common;
using BombRushMP.NetworkInterfaceProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.ServerApp
{
    public class ServerSettings
    {
        public string NetworkInterface = NetworkInterfaces.LiteNetLib.ToString();
        public int Port = 41585;
        public ushort MaxPlayers = 65534;
        public bool UseNativeSockets = true;
        public float TicksPerSecond = 1f/Constants.DefaultNetworkingTickRate;
    }
}
