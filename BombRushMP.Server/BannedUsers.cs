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

        public bool IsBannedByHWID(string hwid)
        {
            if (string.IsNullOrWhiteSpace(hwid)) return false;
            return Users.Values.Any(x => x.HWID == hwid);
        }

        public bool IsBannedByGUID(string guid)
        {
            if (string.IsNullOrWhiteSpace(guid) || !Guid.TryParse(guid, out _)) return false;
            return Users.Values.Any(x => x.GUID == guid);
        }

        public void Ban(string address, string hwid = "", string guid = "",  string name = "None", string reason = "None")
        {
            Users[address] = new BannedUser() { NameAtTimeOfBan = name, Reason = reason, HWID = hwid, GUID = guid };
        }

        public void Unban(string address)
        {
            if (Users.TryGetValue(address, out var result))
                Users.Remove(address);
        }
    }
}
