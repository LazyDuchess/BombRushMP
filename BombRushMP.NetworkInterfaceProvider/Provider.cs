using BombRushMP.Common.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.NetworkInterfaceProvider
{
    public static class NetworkInterfaceFactory
    {
        public static void InitializeNetworkInterface(NetworkInterfaces netInterface)
        {
            INetworkingInterface instance = null;
            switch (netInterface)
            {
                case NetworkInterfaces.Riptide:
                    instance = new RiptideInterface.RiptideInterface();
                    break;
            }
            NetworkingEnvironment.NetworkingInterface = instance;
        }
    }
}
