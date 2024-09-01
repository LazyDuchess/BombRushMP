using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Plugin
{
    public class MPUnlockable
    {
        public string Identifier = "";
        public bool UnlockedByDefault = true;

        public MPUnlockable(string id, bool unlockedByDefault = true)
        {
            Identifier = id;
            UnlockedByDefault = unlockedByDefault;
        }
    }
}
