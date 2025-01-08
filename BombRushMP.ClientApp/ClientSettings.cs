using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BombRushMP.NetworkInterfaceProvider;

namespace BombRushMP.ClientApp
{
    public class ClientSettings
    {
        public string NetworkInterface = NetworkInterfaces.LiteNetLib.ToString();
        public string Address = "localhost";
        public int Port = 41585;
        public bool UseNativeSockets = true;
    }
}
