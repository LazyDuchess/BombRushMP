﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BombRushMP.Server.Gamemodes
{
    public class TeamScoreBattle : ScoreBattle
    {
        public TeamScoreBattle() : base()
        {
            TeamBased = true;
        }
    }
}
