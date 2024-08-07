using System;

namespace BombRushMP.ServerApp
{
    public static class ServerLogger
    {
        public static void Log(string message)
        {
            Console.WriteLine($"[{DateTime.Now.ToShortTimeString()}] {message}");
        }
    }
}
