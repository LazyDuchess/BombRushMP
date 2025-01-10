using BombRushMP.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BombRushMP.Server
{
    public class AuthUser
    {
        [JsonIgnore]
        public UserKinds UserKind
        {
            get
            {
                if (Enum.TryParse<UserKinds>(_userKind, out var result))
                    return result;
                return UserKinds.Player;
            }

            set
            {
                _userKind = value.ToString();
            }
        }
        [JsonProperty("UserKind")]
        private string _userKind = UserKinds.Player.ToString();
        public string[] Tags = [];
        public string[] Badges = [];
    }
}
