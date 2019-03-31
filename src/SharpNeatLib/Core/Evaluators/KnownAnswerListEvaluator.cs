using System;
using System.Collections.Generic;
using System.Text;

namespace SharpNeat.Core
{
    public class KnownAnswerListEvaluator<TGenome, TPhenome> : IGenomeListEvaluator<TGenome>
        where TGenome : class, IGenome<TGenome>
        where TPhenome : class
    {

        private Dictionary<TGenome, FitnessInfo> knownAnswers;

        public ulong EvaluationCount
        {
            get { return (ulong?)knownAnswers?.Count ?? 0; }
        }

        public bool StopConditionSatisfied
        {
            get { return false; }
        }

        public void SetKnownAnswers(Dictionary<TGenome, FitnessInfo> _knownAnswers)
        {
            this.knownAnswers = _knownAnswers;
        }

        public void Evaluate(IList<TGenome> genomeList)
        {
            if (this.knownAnswers == null)
                throw new Exception("The evaluate method for this evaluator can only be called after a successful SetKnownAnswers");

            foreach (var x in genomeList)
            {
                FitnessInfo fitnessInfo = knownAnswers[x];
                x.EvaluationInfo.SetFitness(fitnessInfo._fitness);
                x.EvaluationInfo.AuxFitnessArr = fitnessInfo._auxFitnessArr;
            }

        }

        public void Reset()
        {
            this.knownAnswers = null;
        }
    }
}
