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
        public BannedUsers BannedUsers { get; }

        public AuthKeys AuthKeys { get; }

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
    }
}
