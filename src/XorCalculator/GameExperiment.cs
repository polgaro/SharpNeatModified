using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpNeat.Phenomes;
using SharpNeat.Core;
using SharpNeat.Genomes.Neat;
using TicTacToeConsoleTest;

namespace XorCalculator
{
    public class GameExperiment : SimpleNeatExperiment
    {
        public override IPhenomeEvaluator<IBlackBox> PhenomeEvaluator
        {
            get { return new XorEvaluator(); }
        }

        public override int InputCount
        {
            get { return 2; }
        }

        public override int OutputCount
        {
            get { return 1; }
        }

        public override bool EvaluateParents
        {
            get { return true; }
        }
    }
}
