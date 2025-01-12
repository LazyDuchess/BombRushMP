using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common.Networking
{
    public enum NetChannels : byte
    {
        Default,
        Gamemodes,
        ClientAndLobbyUpdates,
        VisualUpdates,
        Chat,
        Animation,
        MAX
    }
}
