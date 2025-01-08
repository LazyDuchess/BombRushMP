using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Plugin.Gamemodes
{
    public class ToggleGamemodeSetting : GamemodeSetting
    {
        public bool IsOn => (Toggle)Value == Toggle.ON;

        public enum Toggle
        {
            ON,
            OFF
        }

        public ToggleGamemodeSetting(string label, bool on) : base(label, on ? Toggle.ON : Toggle.OFF)
        {

        }
    }
}
