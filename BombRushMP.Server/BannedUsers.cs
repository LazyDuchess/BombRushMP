using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Server
{
    public class BannedUsers
    {
        public List<BannedUser> Users = new();

        public void MakeExample()
        {
            Users.Add(new BannedUser() { Address = "example", NameAtTimeOfBan = "Goofiest Gooner", Reason = "Being annoying" });
        }
    }
}
