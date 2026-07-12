using System.Collections.Generic;

namespace BombRushMP.Common
{
    public static class Constants
    {
        public const uint ProtocolVersion = 24;
        public const float DefaultNetworkingTickRate = 0.03125f;
        public const float ScoreBattleDuration = 180f;
        public const int MaxPayloadSize = 10000;
        public const char CommandChar = '/';
        public const int MaxMessageLength = 256;
        public const int MaxNameLength = 64;
        public const int MaxCrewNameLength = 32;
        public const string PropHuntLockedTag = "phlocked";
        public static HashSet<int> AlwaysAllowedCustomPackets = [ Compression.HashString("ACN-RAGDOLL_EVENT"), Compression.HashString("ACN-RAGDOLL_STATE"), Compression.HashString("ACN-RAGDOLL_LAUNCH"), Compression.HashString("ACN-BANNEDMODS"), Compression.HashString("ACN-BANNEDMODS-JOINEDLOBBY"), Compression.HashString("ACN-PREFERENCESPACKET") ];
    }
}