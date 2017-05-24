using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ReversiLearner
{
    class Learn
    {
        public Learn()
        {

        }
        public Learn(int match, int width, string path)
        {
            Count = match;
            MutationWidth = width;
            var array = File.ReadAllText(path).Replace("\r", "").Split('\n').ToArray();
            ParamList.AddRange(array);
        }
        public int MutationWidth { get; }
        public int MatchNum { get; }
        public int Count{ get; }
        public List<string> ParamList { get; } = new List<string>();
        List<Score> elites= new List<Score>();
        public void InitParams()
        {
            random2 = new Random();
            for (int i = 0; i < 20; i++)
            {
                var list = new List<int>();
                for (int j = 0; j < 16; j++)
                {
                    list.Add(random2.Next(-100, 100));
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

        /// <summary>
        /// GAによるパラメータの更新
        /// </summary>
        /// <returns></returns>
        public async Task<string> FitParams()
        {
            var res = await EvaluateParams();
            elites = res.OrderBy(x => x.Loses).Take(4).ToList();
            ParamList.Clear();

            //エリート2つはそのまま残す
            ParamList.Add(elites[0].Params);
            ParamList.Add(elites[1].Params);

            //エリートの交叉
            var cross1 = Cross(elites[0].Params, elites[1].Params);
            var cross2 = Cross(elites[0].Params, elites[2].Params);
            var cross3 = Cross(elites[0].Params, elites[3].Params);
            var cross4 = Cross(elites[1].Params, elites[0].Params);
            var cross5 = Cross(elites[1].Params, elites[2].Params);
            var cross6 = Cross(elites[1].Params, elites[3].Params);
            var cross7 = Cross(elites[2].Params, elites[0].Params);
            var cross8 = Cross(elites[2].Params, elites[1].Params);
            var cross9 = Cross(elites[2].Params, elites[3].Params);
            var cross10 = Cross(elites[3].Params, elites[0].Params);
            var cross11 = Cross(elites[3].Params, elites[1].Params);
            var cross12 = Cross(elites[3].Params, elites[2].Params);

            //突然変異
            ParamList.Add(Mutation(cross1));
            ParamList.Add(Mutation(cross2));
            ParamList.Add(Mutation(cross3));
            ParamList.Add(Mutation(cross4));
            ParamList.Add(Mutation(cross5));
            ParamList.Add(Mutation(cross6));
            ParamList.Add(Mutation(cross7));
            ParamList.Add(Mutation(cross8));
            ParamList.Add(Mutation(cross9));
            ParamList.Add(Mutation(cross10));
            ParamList.Add(Mutation(cross11));
            ParamList.Add(Mutation(cross12));

            return string.Format("best: {0}\n{1}-{2}-{3}\n",elites[0].Params,elites[0].Wins,elites[0].Loses,elites[0].Draws);
        }

        #region GAによる個体生成
        Random random1 = new Random(DateTime.Now.Millisecond);
        Random random2 = new Random(DateTime.Now.Millisecond);
        /// <summary>
        /// 大幅な突然変異
        /// </summary>
        /// <returns></returns>
        private string Mutation(string params1)
        {
            var list1 = params1.Split(',').Select(x => int.Parse(x)).ToList();
            var list2 = new List<int>();
            for (int j = 0; j < 16; j++)
            {
                list2.Add(random1.Next(-MutationWidth, MutationWidth));
            }
            var res = Enumerable.Zip(list1, list2, (x, y) => 
            {
                if (x+y>100)
                {
                    return 100;
                }
                else if (x+y<-100)
                {
                    return -100;
                }
                else
                {
                    return x + y;
                }
            });
            return string.Join(",", res);
        }
        /// <summary>
        /// 交叉
        /// </summary>
        /// <param name="params1"></param>
        /// <param name="params2"></param>
        /// <returns></returns>
        private string Cross(string params1, string params2)
        {
            var list1 = params1.Split(',').Select(x => int.Parse(x)).ToList();
            var list2 = params2.Split(',').Select(x => int.Parse(x)).ToList();
            var splitter = random2.Next(1,14);
            var list1Former = new List<int>();
            var list2Latter = new List<int>();
            for (int i = 0; i < splitter; i++)
            {
                list1Former.Add(list1[i]);
            }
            for (int i = splitter; i < 16; i++)
            {
                list2Latter.Add(list2[i]);
            }
            var resList = Enumerable.Concat(list1Former, list2Latter);
            return string.Join(",", resList);
        }
        #endregion

        /// <summary>
        /// 対局
        /// </summary>
        /// <param name="paramsString"></param>
        /// <returns></returns>
        private async Task<Score> MatchWithRandomEngine(string paramsString)
        {
            Console.WriteLine("{0}", paramsString);
            return await Task.Run(() =>
            {
                var senteWin = 0;
                var senteLose = 0;
                var senteDraw = 0;
                var senteList = new List<Match>();
                for (int i = 0; i < Count; i++)
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
                for (int i = 0; i < Count; i++)
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
