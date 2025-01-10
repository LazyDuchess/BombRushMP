using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Server
{
    public class AuthKeys
    {
        public Dictionary<string, AuthUser> Users = new();

        public void MakeExample()
        {
            Users["Example"] = new AuthUser() { UserKind = Common.UserKinds.Player, Badges = ["testbadge"], Tags = ["testtag"] };
        }
    }
}
