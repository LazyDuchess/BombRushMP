using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BombRushMP.Common;

namespace BombRushMP.Server
{
    public class AuthKeys
    {
        public Dictionary<string, AuthUser> Users = new();
        private AuthUser _defaultUser = new AuthUser();

        public void MakeExample()
        {
            Users["Default"] = new AuthUser(UserKinds.Player, [], [], "Default user");
        }

        /// <summary>
        /// Returns an user with the specified Auth Key. If not found, will return the Default user.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public AuthUser GetUser(string key)
        {
            if (Users.TryGetValue(key, out var result))
                return result;
            else if (Users.TryGetValue("Default", out var result2))
                return result2;
            return _defaultUser;
        }
    }
}
