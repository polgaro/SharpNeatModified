using SharpNeat.EvolutionAlgorithms;
using SharpNeat.Genomes.Neat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace XorCalculator
{
    class Program
    {
        const string FILE = "train.xml";

        static void Main(string[] args)
        {
            GameExperiment experiment = new GameExperiment();

            XmlDocument xmlConfig = new XmlDocument();
            xmlConfig.Load("expconfig.xml");
            experiment.Initialize("TicTacToe", xmlConfig.DocumentElement);

            NeatEvolutionAlgorithm<NeatGenome> _ea;
            _ea = experiment.CreateEvolutionAlgorithm();

            // Create evolution algorithm and attach update event.
            _ea.UpdateEvent += new EventHandler(ea_UpdateEvent);

            _ea.StartContinue();

            Console.ReadLine();
        }

        private static void ea_UpdateEvent(object sender, EventArgs e)
        {
            NeatEvolutionAlgorithm<NeatGenome> _ea = (NeatEvolutionAlgorithm<NeatGenome>)sender;
            Console.WriteLine(string.Format("gen={0:N0} bestFitness={1:N6} maxComplexity={2} currentChampComplexity={3} avg={4}",
                _ea.CurrentGeneration,
                _ea.Statistics._maxFitness,
                _ea.Statistics._maxComplexity,
                _ea.CurrentChampGenome.Complexity,
                _ea.GenomeList.Average(g => g.EvaluationInfo.Fitness)));

            // Save the best genome to file
            var doc = NeatGenomeXmlIO.SaveComplete(new List<NeatGenome>() { _ea.CurrentChampGenome }, false);
            doc.Save(FILE);

            //doc = NeatGenomeXmlIO.SaveComplete(_ea.GenomeList, true);
            //doc.Save(NETWORK_FILE);
        }
    }
}
