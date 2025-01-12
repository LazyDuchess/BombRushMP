using BombRushMP.Common;
using BombRushMP.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Plugin.OfflineInterface
{
    public class LocalServerDatabase : IServerDatabase
    {
        public const string OfflineAuthKey = "Offline";
        public BannedUsers BannedUsers { get; } = new BannedUsers();

        public AuthKeys AuthKeys { get; } = new AuthKeys();

        public LocalServerDatabase(string adminKey)
        {
            AuthKeys.Users[adminKey] = new AuthUser() { UserKind = UserKinds.Admin, Badges = [0] };
        }

        public void Load()
        {
            // nun
        }

        public void Save()
        {
            // nun
        }

        public void LogChatMessage(string message, int stage)
        {
            // nun
        }
    }
}
