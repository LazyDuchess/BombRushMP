using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common.Networking
{
    public interface IConnectionFailedEventArgs
    {
        public RejectReason Reason { get; }
    }
}
