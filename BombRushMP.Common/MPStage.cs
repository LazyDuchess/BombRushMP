using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common
{
    public class MPStage
    {
        public string DisplayName;
        public int Id;
        public MPStage(string displayName, int id)
        {
            DisplayName = displayName;
            Id = id;
        }
    }
}
