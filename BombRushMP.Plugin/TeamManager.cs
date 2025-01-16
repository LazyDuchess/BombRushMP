using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BombRushMP.Plugin
{
    public static class TeamManager
    {
        public static Team[] Teams = [
            new Team("Red Team", Color.red),
            new Team("Blue Team", Color.blue),
            new Team("Yellow Team", Color.yellow),
            new Team("Green Team", Color.green)
        ];
    }
}
