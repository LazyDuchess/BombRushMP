using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Server
{
    public class BannedUsers
    {
        public Dictionary<string, BannedUser> Users = new();

        public void MakeExample()
        {
            Users["Example"] = new BannedUser() { NameAtTimeOfBan = "Goofiest Gooner", Reason = "Being annoying" };
        }

        public bool IsBanned(string address)
        {
            return Users.ContainsKey(address);
        }

        public void Ban(string address, string name = "None", string reason = "None")
        {
            Users[address] = new BannedUser() { NameAtTimeOfBan = name, Reason = reason };
        }

        public void Unban(string address)
        {
            if (Users.TryGetValue(address, out var result))
                Users.Remove(address);
        }
    }
}
