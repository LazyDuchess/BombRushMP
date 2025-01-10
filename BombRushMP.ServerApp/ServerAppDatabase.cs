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
            }
            else
                BannedUsers = JsonConvert.DeserializeObject<BannedUsers>(BannedUsersPath);

            if (!File.Exists(AuthKeysPath))
            {
                AuthKeys = new AuthKeys();
            }
            else
                AuthKeys = JsonConvert.DeserializeObject<AuthKeys>(AuthKeysPath);
            Save();
        }

        public void Save()
        {
            File.WriteAllText(BannedUsersPath, JsonConvert.SerializeObject(BannedUsers));
            File.WriteAllText(AuthKeysPath, JsonConvert.SerializeObject(AuthKeys));
        }
    }
}
