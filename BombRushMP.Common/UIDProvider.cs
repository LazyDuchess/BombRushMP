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
        private ulong _currentUID = 1;
        private HashSet<ulong> _usedUIDs = new();

        public ulong RequestUID()
        {
            while(_usedUIDs.Contains(_currentUID))
            {
                if (_currentUID < ulong.MaxValue)
                    _currentUID++;
                else
                    _currentUID = 1;
            }
            _usedUIDs.Add(_currentUID);
            return _currentUID++;
        }

        public void FreeUID(ulong uid)
        {
           _usedUIDs.Remove(uid);
        }
    }
}
