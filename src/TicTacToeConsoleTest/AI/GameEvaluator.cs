using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpNeat.Core;
using SharpNeat.Phenomes;
using TitTacToeGame;

namespace TicTacToeConsoleTest
{
    public class GameEvaluator : IPhenomeEvaluator<IBlackBox>
    {
        private ulong _evalCount;
        private bool _stopConditionSatisfied;
        static Random random = new Random();

        #region IPhenomeEvaluator<IBlackBox> Members

        /// <summary>
        /// Gets the total number of evaluations that have been performed.
        /// </summary>
        public ulong EvaluationCount
        {
            get { return _evalCount; }
        }

        /// <summary>
        /// Gets a value indicating whether some goal fitness has been achieved and that
        /// the the evolutionary algorithm/search should stop. This property's value can remain false
        /// to allow the algorithm to run indefinitely.
        /// </summary>
        public bool StopConditionSatisfied
        {
            get { return _stopConditionSatisfied; }
        }

        public FitnessInfo Evaluate(IBlackBox p1Brain)
        {
            double fitness = 0;

            //Play 100 games
            for (int i = 0; i < 100; i++)
            {
                fitness += GetFitness(p1Brain);
            }

            return new FitnessInfo(fitness, fitness);
        }

        public FitnessInfo Evaluate(IBlackBox p1Brain, IEnumerable<IBlackBox> p2Players)
        {
            double fitness = 0;
            object objLock = new object();

            Parallel.ForEach(p2Players, 
                new ParallelOptions { MaxDegreeOfParallelism = 1000 },
                p2 =>
            {
                /*foreach (var p2 in p2Players.AsParallel())
                {*/
                    var localFitness = GetFitness(p1Brain, p2);
                    lock (objLock)
                    {
                        fitness += localFitness;
                    }
                //}
            });

            return new FitnessInfo(fitness, fitness);
        }

        public double GetFitness(IBlackBox p1Brain, IBlackBox p2Brain = null)
        {
            //generate the brain with the algorithm
            IPlayer p1 = new NeatBrain(p1Brain);

            IPlayer p2;
            if (p2Brain == null)
                p2 = new RandomBrain();
            else
                p2 = new NeatBrain(p2Brain);

            return GetMatchPoints(p1, p2);
        }

        private double GetMatchPoints(IPlayer p1, IPlayer p2)
        {
            bool useP1 = random.Next(2) == 1;

            if (!useP1)
                Swap(ref p1, ref p2);

            Game Game = new Game(p1, p2);
            try
            {
                Game.PlayUntilFinished();
                //score
                return GetScore(Game.GetGameState(), useP1);
            }
            catch
            {
                return Game.NumberOfMoves;
            }
        }

        public static void Swap<T>(ref T p1, ref T p2)
        {
            T aux = p1;
            p1 = p2;
            p2 = aux;
        }

        public static double GetScore(WinnerEnum winnerEnum, bool isP1)
        {
            switch(winnerEnum)
            {
                case WinnerEnum.Draw:
                    return 90;
                case WinnerEnum.Player1:
                    return isP1 ? 100 : 0;
                case WinnerEnum.Player2:
                    return isP1 ? 0 : 100;
            }
            return 0;
        }

        public void Reset()
        {
        }
        #endregion
    }
}
