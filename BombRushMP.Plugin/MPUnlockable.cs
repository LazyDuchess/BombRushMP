using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Plugin
{
    public class MPUnlockable
    {
        public int Identifier = -1;
        public bool UnlockedByDefault = true;

        public MPUnlockable(int id, bool unlockedByDefault = true)
        {
            Identifier = id;
            UnlockedByDefault = unlockedByDefault;
        }
    }
}
