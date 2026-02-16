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
        public AuthUser GetUser(string key, string challenge)
        {
            foreach(var us in Users)
            {
                var otherHashed = AuthUser.HashPassword(us.Key, challenge);
                if (otherHashed == key) return us.Value;
            }
            if (Users.TryGetValue("Default", out var result))
                return result;
            return _defaultUser;
        }
    }
}
