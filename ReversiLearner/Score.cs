﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReversiLearner
{
    public class Score
    {
        public Score(string paramsString, int wins,int loses,int draws,double occRate)
        {
            Params = paramsString;
            Wins = wins;
            Loses = loses;
            Draws = draws;
            WorstOccupationRate = occRate;
        }
        public string Params { get; }
        public int Wins { get; }
        public int Loses { get; }
        public int Draws { get; }
        /// <summary>
        /// 石の占有率のうち、最も低かったもの
        /// </summary>
        public double WorstOccupationRate { get; }
    }
}
