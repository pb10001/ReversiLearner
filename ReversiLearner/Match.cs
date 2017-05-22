using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reversi.Core;
using ThinkingEngineBase;
using RandomEngine;

namespace ReversiLearner
{
    public class Match
    {
        public Match(string paramsString)
        {
            this.paramsString = paramsString;
        }
        int turnNum;
        int passNum;
        string paramsString;
        bool previouslyPassed;
        ReversiBoard board;
        StoneType currentPlayer;
        IThinkingEngine senteEngine;
        IThinkingEngine goteEngine;
        public MatchResult Winner { get; private set; }
        private void Init()
        {
            turnNum = 0;
            passNum = 0;
            previouslyPassed = false;
            currentPlayer = StoneType.Sente;
            board = ReversiBoard.InitBoard();
            Winner = MatchResult.NotYet;
        }
        public MatchResult Execute()
        {
            Init();
            while (turnNum-passNum<60)
            {
                if (currentPlayer == StoneType.Sente)
                {
                    if (board.SearchLegalMoves(StoneType.Sente).Count!=0)
                    {
                        turnNum++;
                        var move = senteEngine.Think(board, StoneType.Sente).Result;
                        board = board.AddStone(move.Row, move.Col, StoneType.Sente);
                        previouslyPassed = false;
                        currentPlayer = StoneType.Gote;
                    }
                    else
                    {
                        if (previouslyPassed)
                        {
                            if (board.NumOfBlack() > board.NumOfWhite())
                            {
                                Winner = MatchResult.Sente;
                            }
                            else if (board.NumOfBlack() < board.NumOfWhite())
                            {
                                Winner = MatchResult.Gote;
                            }
                            else
                            {
                                Winner = MatchResult.Draw;
                            }
                            return Winner;
                        }
                        else
                        {
                            passNum++;
                            currentPlayer = StoneType.Gote;
                            previouslyPassed = true;
                        }
                    }
                }
                else
                {
                    if (board.SearchLegalMoves(StoneType.Gote).Count != 0)
                    {
                        turnNum++;
                        var move = goteEngine.Think(board, StoneType.Gote).Result;
                        board = board.AddStone(move.Row, move.Col, StoneType.Gote);
                        previouslyPassed = false;
                        currentPlayer = StoneType.Sente;
                    }
                    else
                    {
                        if (previouslyPassed)
                        {
                            if (board.NumOfBlack() > board.NumOfWhite())
                            {
                                Winner = MatchResult.Sente;
                            }
                            else if (board.NumOfBlack() < board.NumOfWhite())
                            {
                                Winner = MatchResult.Gote;
                            }
                            else
                            {
                                Winner = MatchResult.Draw;
                            }
                            return Winner;
                        }
                        else
                        {
                            passNum++;
                            currentPlayer = StoneType.Sente;
                            previouslyPassed = true;
                        }
                    }
                }
            }
            if (board.NumOfBlack() > board.NumOfWhite())
            {
                Winner = MatchResult.Sente;
            }
            else if (board.NumOfBlack() < board.NumOfWhite())
            {
                Winner = MatchResult.Gote;
            }
            else
            {
                Winner = MatchResult.Draw;
            }
            return Winner;
        }
        public void SenteThinking()
        {
            var engine = new ThinkingEngine();
            engine.Evaluator = Eval.FromParamsString(paramsString);
            senteEngine = engine;
            goteEngine = new RandomThinking();
        }
        public void GoteThinking()
        {
            var engine = new ThinkingEngine();
            engine.Evaluator = Eval.FromParamsString(paramsString);
            senteEngine = new RandomThinking();
            goteEngine = engine;
        }
        
    }
}
