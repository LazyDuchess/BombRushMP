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
        ClientLobbyCreate,
        ClientLobbyJoin,
        ClientLobbyLeave,
        ServerLobbies,
        ServerLobbyCreateResponse,
        ServerLobbyDeleted,
        ClientLobbyStart,
        ServerLobbyStart,
        ServerLobbyEnd,
        ServerGamemodeBegin,
        ClientGameModeScore,
        ClientLobbyEnd,
        ClientGraffitiRaceGSpots,
        ClientLobbySetGamemode,
        ServerPlayerCount,
        ClientLobbySetReady,
        ClientLobbyInvite,
        ServerLobbyInvite,
        ClientLobbyDeclineInvite,
        ClientLobbyDeclineAllInvites,
        ClientLobbyKick,
        ClientChat,
        ServerChat,
        ClientComboOver,
        ClientAuth,
        ServerBanList,
        ClientGraffitiRaceStart,
        ClientGamemodeCountdown,
        ClientScoreBattleLength,
        ClientHitByPlayer,
        ServerSetChibi,
        ClientSetTeam,
        ClientTeamGraffRaceScore,
        ServerTeamGraffRaceScore,
        ClientLobbySetAllowTeamSwitching,
        ClientLobbySetPlayerTeam,
        ClientLobbySetChallenge,
        ServerLobbySoftUpdate,
        ServerClientDisconnected,
        ServerSetSpecialSkin,
        ServerServerStateUpdate,
        ServerTeleportPlayer
    }
}
