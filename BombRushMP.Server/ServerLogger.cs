using System;

namespace BombRushMP.Server
{
    public static class ServerLogger
    {
        public static void Log(string message)
        {
            Console.WriteLine($"[{DateTime.Now.ToShortTimeString()}] {message}");
        }
    }
}
