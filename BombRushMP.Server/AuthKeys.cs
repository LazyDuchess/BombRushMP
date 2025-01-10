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
        private AuthUser _defaultUser = new AuthUser();

        public void MakeExample()
        {
            Users["Example"] = new AuthUser() { UserKind = Common.UserKinds.Player, Badges = ["testbadge"], Tags = ["testtag"] };
        }

        /// <summary>
        /// Returns an user with the specified Auth Key. If not found, will return a Player user.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public AuthUser GetUser(string key)
        {
            if (Users.TryGetValue(key, out var result))
                return result;
            return _defaultUser;
        }
    }
}
