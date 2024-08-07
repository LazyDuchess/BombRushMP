using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common
{
    /// <summary>
    /// Automatically provides unique IDs for objects, starting from 1.
    /// </summary>
    public class UIDProvider
    {
        private uint _currentUID = 1;
        private HashSet<uint> _usedUIDs = new();

        public uint RequestUID()
        {
            while(_usedUIDs.Contains(_currentUID))
            {
                if (_currentUID < uint.MaxValue)
                    _currentUID++;
                else
                    _currentUID = 1;
            }
            _usedUIDs.Add(_currentUID);
            return _currentUID++;
        }

        public void FreeUID(uint uid)
        {
           _usedUIDs.Remove(uid);
        }
    }
}
