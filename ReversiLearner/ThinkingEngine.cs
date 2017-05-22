using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reversi.Core;
using ThinkingEngineBase;

namespace ReversiLearner
{
    public class ThinkingEngine:IThinkingEngine
    {
        Eval evaluator;
        public Eval Evaluator
        {
            set
            {
                if (evaluator==default(Eval))
                {
                    evaluator = value;
                }
                else
                {
                    throw new InvalidOperationException("既に登録されています");
                }
            }
        }
        //合法手のリスト
        List<ReversiMove> legalMoves = new List<ReversiMove>();

        public string Name
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void SetTimeLimit(int milliSecond)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 盤の情報をもとに思考し、次の手を返す
        /// </summary>
        /// <param name="board"></param>
        /// <param name="player"></param>
        /// <returns></returns>
        public async Task<ReversiMove> Think(ReversiBoard board, StoneType player)
        {
            return await Task.Run(() =>
            {
                legalMoves = board.SearchLegalMoves(player); //合法手
                if (legalMoves.Count == 0)
                {
                    throw new InvalidOperationException("合法手がありません");
                }
                else
                {
                    var best = -999999;
                    var bestMove = default(ReversiMove);
                    foreach (var item in board.SearchLegalMoves(player))
                    {
                        var child = board.AddStone(item.Row, item.Col, player);
                        var eval = evaluator.Execute(child.BlackToMat(),child.WhiteToMat());
                        switch (player)
                        {
                            case StoneType.None:
                                break;
                            case StoneType.Sente:
                                if (best < eval)
                                {
                                    best = eval;
                                    bestMove = item;
                                }
                                break;
                            case StoneType.Gote:
                                if (best < -eval)
                                {
                                    best = -eval;
                                    bestMove = item;
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    return bestMove;
                }
            });

        }
    }
}
