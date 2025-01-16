using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BombRushMP.Plugin
{
    public class Team
    {
        public string Name;
        public Color Color;

        public Team(string name, Color color)
        {
            Name = name;
            Color = color;
        }
    }
}
