using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReversiLearner
{
    public class Score
    {
        public Score(string paramsString, int wins,int loses,int draws)
        {
            Params = paramsString;
            Wins = wins;
            Loses = loses;
            Draws = draws;
        }
        public string Params { get; }
        public int Wins { get; }
        public int Loses { get; }
        public int Draws { get; }
    }
}
