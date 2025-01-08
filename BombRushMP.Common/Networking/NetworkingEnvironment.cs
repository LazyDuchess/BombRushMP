using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common.Networking
{
    public static class NetworkingEnvironment
    {
        public static INetworkingInterface NetworkingInterface;
        public static bool UseNativeSocketsIfAvailable = true;
        public static Action<string> LogEventHandler;

        public static void Log(string message)
        {
            LogEventHandler?.Invoke(message);
        }
    }
}
