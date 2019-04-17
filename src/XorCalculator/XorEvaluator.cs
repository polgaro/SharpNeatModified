using SharpNeat.Core;
using SharpNeat.Phenomes;
using System;

namespace XorCalculator
{
    public class XorEvaluator : IPhenomeEvaluator<IBlackBox>
    {
        bool _perfectScore = false;

        public ulong EvaluationCount
        {
            get
            {
                return 0;
            }
        }

        public bool StopConditionSatisfied
        {
            get
            {
                return _perfectScore;
            }
        }

        public FitnessInfo Evaluate(IBlackBox phenome)
        {
            //generate the brain with the algorithm
            XorBrain neuron = new XorBrain(phenome);
            int points = 0;
            int amountOfTries = 1000;

            for(int i = 0; i < 1000; i++)
            {
                int input1 = new Random().Next(2);
                int input2 = new Random().Next(2);
                int realAnswer = input1 ^ input2;
                int neatAnswer = neuron.Calculate(input1, input2);

                if (realAnswer == neatAnswer)
                    points++;
            }

            if (amountOfTries == points)
                _perfectScore = true;

            return new FitnessInfo(points, points);

        }

        public void Reset()
        {
            
        }
    }
}