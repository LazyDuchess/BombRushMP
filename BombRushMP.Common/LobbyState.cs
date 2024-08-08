﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Common
{
    public class LobbyState
    {
        public bool InGame = false;
        public GamemodeIDs Gamemode = GamemodeIDs.ScoreBattle;
        public int Stage = 0;
        public uint Id = 0;
        public ushort HostId = 0;
        public Dictionary<ushort, LobbyPlayer> Players = new();

        public LobbyState()
        {

        }

        public LobbyState(int stage, uint id, ushort hostId)
        {
            Stage = stage;
            Id = id;
            HostId = hostId;
        }

        public void Read(BinaryReader reader)
        {
            InGame = reader.ReadBoolean();
            Gamemode = (GamemodeIDs)reader.ReadInt32();
            Stage = reader.ReadInt32();
            Id = reader.ReadUInt32();
            HostId = reader.ReadUInt16();
            Players.Clear();
            var playerCount = reader.ReadInt32();
            for (var i = 0; i < playerCount; i++)
            {
                var player = new LobbyPlayer();
                player.Read(reader);
                player.LobbyId = Id;
                Players[player.Id] = player;
            }
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(InGame);
            writer.Write((int)Gamemode);
            writer.Write(Stage);
            writer.Write(Id);
            writer.Write(HostId);
            writer.Write(Players.Count);
            foreach(var player in Players)
            {
                player.Value.Write(writer);
            }
        }
    }
}
