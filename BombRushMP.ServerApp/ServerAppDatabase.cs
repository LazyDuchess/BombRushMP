using BombRushMP.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace BombRushMP.ServerApp
{
    public class ServerAppDatabase : IServerDatabase
    {
        public BannedUsers BannedUsers { get; private set; }

        public AuthKeys AuthKeys { get; private set; }

        private const string BannedUsersPath = "banned_users.json";
        private const string AuthKeysPath = "auth_keys.json";

        public ServerAppDatabase()
        {
            if (!File.Exists(BannedUsersPath))
            {
                BannedUsers = new BannedUsers();
                BannedUsers.MakeExample();
            }
            else
                BannedUsers = JsonConvert.DeserializeObject<BannedUsers>(File.ReadAllText(BannedUsersPath));

            if (!File.Exists(AuthKeysPath))
            {
                AuthKeys = new AuthKeys();
                AuthKeys.MakeExample();
            }
            else
                AuthKeys = JsonConvert.DeserializeObject<AuthKeys>(File.ReadAllText(AuthKeysPath));
            Save();
        }

        public void Save()
        {
            File.WriteAllText(BannedUsersPath, JsonConvert.SerializeObject(BannedUsers, Formatting.Indented));
            File.WriteAllText(AuthKeysPath, JsonConvert.SerializeObject(AuthKeys, Formatting.Indented));
        }

        public void Load()
        {
            if (File.Exists(BannedUsersPath))
                BannedUsers = JsonConvert.DeserializeObject<BannedUsers>(File.ReadAllText(BannedUsersPath));

            if (File.Exists(AuthKeysPath))
                AuthKeys = JsonConvert.DeserializeObject<AuthKeys>(File.ReadAllText(AuthKeysPath));
        }

        public void LogChatMessage(string message, int stage)
        {
            var dir = "Chats";
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            var fullPath = Path.Combine(dir, $"{stage}.txt");
            using (var writer = File.AppendText(fullPath))
            {
                writer.WriteLine(message);
            }
            var globalPath = Path.Combine(dir, "Global.txt");
            using (var writer = File.AppendText(globalPath))
            {
                writer.WriteLine($"({stage}) {message}");
            }
        }
    }
}
