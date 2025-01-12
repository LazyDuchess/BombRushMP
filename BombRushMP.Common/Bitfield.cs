using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common
{
    public class Bitfield
    {
        public bool this[Enum key]
        {
            get => GetValue(key);
            set => SetValue(key, value);
        }
        private bool[] _values = [];
        public Bitfield()
        {

        }

        public bool GetValue(Enum index)
        {
            return _values[Convert.ToInt32(index)];
        }

        public void SetValue(Enum index, bool value)
        {
            _values[Convert.ToInt32(index)] = value;
        }
    }
}
