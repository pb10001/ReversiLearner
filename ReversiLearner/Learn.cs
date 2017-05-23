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
        public Learn(string path)
        {
            var array = File.ReadAllText(path).Replace("\r", "").Split('\n').ToArray();
            ParamList.AddRange(array);
        }
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
            elites = res.OrderByDescending(x => x.Wins).Take(10).ToList();
            ParamList.Clear();

            //エリート2つはそのまま残す
            ParamList.Add(elites[0].Params);
            ParamList.Add(elites[1].Params);

            //エリート2つの交叉
            ParamList.Add(Crossing(elites[0].Params, elites[1].Params));
            ParamList.Add(Crossing(elites[1].Params, elites[0].Params));

            //大幅な突然変異を含む個体生成
            var mut0 = MajorMutation(elites[0].Params);
            var mut1 = MajorMutation(elites[1].Params);
            ParamList.Add(mut0.Item1);
            ParamList.Add(mut0.Item2);
            ParamList.Add(mut1.Item1);
            ParamList.Add(mut1.Item2);

            //小幅な突然変異を含む個体生成
            ParamList.Add(MinorMutation(elites[0].Params));
            ParamList.Add(MinorMutation(elites[1].Params));
            ParamList.Add(MinorMutation(elites[2].Params));
            ParamList.Add(MinorMutation(elites[3].Params));
            ParamList.Add(MinorMutation(elites[4].Params));
            ParamList.Add(MinorMutation(elites[5].Params));
            ParamList.Add(MinorMutation(elites[6].Params));
            ParamList.Add(MinorMutation(elites[7].Params));
            ParamList.Add(MinorMutation(elites[8].Params));
            ParamList.Add(MinorMutation(elites[9].Params));

            return string.Format("best: {0}\n{1}-{2}-{3}\n",elites[0].Params,elites[0].Wins,elites[0].Loses,elites[0].Draws);
        }

        #region GAによる個体生成
        Random random1 = new Random(DateTime.Now.Millisecond);
        Random random2 = new Random(DateTime.Now.Millisecond);
        /// <summary>
        /// 大幅な突然変異
        /// </summary>
        /// <returns></returns>
        private Tuple<string,string> MajorMutation(string params1)
        {
            var list1 = params1.Split(',').Select(x => int.Parse(x)).ToList();
            var list2 = new List<int>();
            for (int j = 0; j < 16; j++)
            {
                list2.Add(random1.Next(-100, 100));
            }
            var splitter = random2.Next(1, 14);
            var list1Former = new List<int>();
            var list1Latter = new List<int>();
            var list2Former = new List<int>();
            var list2Latter = new List<int>();
            for (int i = 0; i < splitter; i++)
            {
                list1Former.Add(list1[i]);
                list2Former.Add(list2[i]);
            }
            for (int i = splitter; i < 16; i++)
            {
                list1Latter.Add(list1[i]);
                list2Latter.Add(list2[i]);
            }
            var resList1 = Enumerable.Concat(list1Former, list2Latter);
            var resList2 = Enumerable.Concat(list2Former, list1Latter);
            return new Tuple<string, string>(string.Join(",",resList1),string.Join(",",resList2));
        }
        /// <summary>
        /// 小幅な突然変異
        /// </summary>
        /// <param name="params1"></param>
        /// <returns></returns>
        private string MinorMutation(string params1)
        {
            var list1 = params1.Split(',').Select(x => int.Parse(x)).ToList();
            var splitter = random2.Next(0, 15);
            list1[splitter] = random1.Next(-100, 100);
            return string.Join(",", list1);
        }
        /// <summary>
        /// 交叉
        /// </summary>
        /// <param name="params1"></param>
        /// <param name="params2"></param>
        /// <returns></returns>
        private string Crossing(string params1, string params2)
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
