using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReversiLearner
{
    class Learn
    {
        public Learn()
        {

        }
        public List<string> ParamList { get; } = new List<string>();
        public void InitParams()
        {
            var random = new Random();
            for (int i = 0; i < 20; i++)
            {
                var list = new List<int>();
                for (int j = 0; j < 16; j++)
                {
                    list.Add(random.Next(-100, 100));
                }
                ParamList.Add(string.Join(",", list));
            }
        }
        public async Task<List<Score>> EvaluateParams()
        {
            return await Task.Run(async () =>
            {
                var res = new List<Score>();
                foreach (var param in ParamList)
                {
                    res.Add(await MatchWithRandomEngine(param));
                }
                return res;
            });
            
        }
        public void Crossing()
        {

        }
        public async Task<Score> MatchWithRandomEngine(string paramsString)
        {
            Console.WriteLine("{0}", paramsString);
            return await Task.Run(() =>
            {
                var senteWin = 0;
                var senteLose = 0;
                var senteDraw = 0;
                var senteList = new List<Match>();
                for (int i = 0; i < 100; i++)
                {
                    var match = new Match(paramsString);
                    match.SenteThinking();
                    senteList.Add(match);
                }
                Parallel.ForEach(senteList, match =>
                {
                    var res = match.Execute();
                    switch (res)
                    {
                        case Reversi.Core.MatchResult.Draw:
                            senteDraw++;
                            break;
                        case Reversi.Core.MatchResult.Sente:
                            senteWin++;
                            break;
                        case Reversi.Core.MatchResult.Gote:
                            senteLose++;
                            break;
                        case Reversi.Core.MatchResult.NotYet:
                            break;
                        default:
                            break;
                    }
                });
                Console.WriteLine("先手: {0}-{1}-{2}", senteWin, senteLose, senteDraw);
                var goteWin = 0;
                var goteLose = 0;
                var goteDraw = 0;
                var goteList = new List<Match>();
                for (int i = 0; i < 100; i++)
                {
                    var match = new Match(paramsString);
                    match.GoteThinking();
                    goteList.Add(match);
                }
                Parallel.ForEach(senteList, match =>
                {
                    var res = match.Execute();
                    switch (res)
                    {
                        case Reversi.Core.MatchResult.Draw:
                            goteDraw++;
                            break;
                        case Reversi.Core.MatchResult.Sente:
                            goteWin++;
                            break;
                        case Reversi.Core.MatchResult.Gote:
                            goteLose++;
                            break;
                        case Reversi.Core.MatchResult.NotYet:
                            break;
                        default:
                            break;
                    }
                });
                Console.WriteLine("後手: {0}-{1}-{2}", goteWin, goteLose, goteDraw);
                Console.WriteLine("合計: {0}-{1}-{2}", senteWin + goteWin, senteLose + goteLose, senteDraw + goteDraw);
                Console.WriteLine("--------------");
                return new Score(paramsString, senteWin + goteWin, senteLose + goteLose, senteDraw + goteDraw);
            });
            
        }
    }
}
