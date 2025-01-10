using BombRushMP.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Server
{
    public class AuthUser
    {
        public string UserKind = UserKinds.Player.ToString();
        public string[] Tags = [];
        public string[] Badges = [];
    }
}
