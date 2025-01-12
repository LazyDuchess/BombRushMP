using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Server
{
    public interface IServerDatabase
    {
        public BannedUsers BannedUsers { get; }
        public AuthKeys AuthKeys { get; }

        public void Load();
        public void Save();
        public void LogChatMessage(string message, int stage);
    }
}
