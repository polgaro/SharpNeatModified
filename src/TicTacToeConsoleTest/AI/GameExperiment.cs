using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpNeat.Domains;
using SharpNeat.Phenomes;
using SharpNeat.Core;
using SharpNeat.Genomes.Neat;

namespace TicTacToeConsoleTest
{
    public class GameExperiment : SimpleNeatExperiment
    {
        public override IPhenomeEvaluator<IBlackBox> PhenomeEvaluator
        {
            //get { return new GameEvaluator(); }
            get { return null; }
        }

        public override int InputCount
        {
            get { return 9; }
        }

        public override int OutputCount
        {
            get { return 9; }
        }

        public override bool EvaluateParents
        {
            get { return true; }
        }

        public override IGenomeListEvaluator<NeatGenome> GetGenomeListEvaluator(IGenomeDecoder<NeatGenome, IBlackBox> genomeDecoder)
        {
            return new ZeroListEvaluator<NeatGenome>();
        }
    }
}
