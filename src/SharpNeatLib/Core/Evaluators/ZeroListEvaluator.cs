using SharpNeat.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SharpNeat.Core
{
    public class ZeroListEvaluator<TGenome> : IGenomeListEvaluator<TGenome>
        where TGenome : class, IGenome<TGenome>
    {
        readonly ParallelOptions _parallelOptions = new ParallelOptions();

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
                return false;
            }
        }

        public void Evaluate(IList<TGenome> genomeList)
        {
            Parallel.ForEach(genomeList, _parallelOptions, delegate (TGenome genome)
            {
                
                FitnessInfo fitnessInfo = new FitnessInfo(0,0);
                genome.EvaluationInfo.SetFitness(fitnessInfo._fitness);
                genome.EvaluationInfo.AuxFitnessArr = fitnessInfo._auxFitnessArr;
                
            });
        }

        public void Reset()
        {
            
        }
    }
}
