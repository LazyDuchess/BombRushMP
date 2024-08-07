using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common.Packets
{
    public enum Packets : ushort
    {
        ClientState,
        ServerConnectionResponse,
        ServerClientStates,
        ClientVisualState,
        ServerClientVisualStates,
        PlayerAnimation,
        PlayerVoice,
        PlayerGenericEvent,
        PlayerGraffitiSlash,
        PlayerGraffitiFinisher,
        ClientLobbyCreate
    }
}
