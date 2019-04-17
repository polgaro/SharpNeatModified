using SharpNeat.Core;
using SharpNeat.EvolutionAlgorithms;
using SharpNeat.Genomes.Neat;
using SharpNeat.Phenomes;
using SharpNeatLib.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using TitTacToeGame;

namespace TicTacToeConsoleTest
{
    class Program
    {
        const string FILE = "train.xml";
        const string NETWORK_FILE = "network.xml";
        static Random random = new Random();

        static void Main(string[] args)
        {
            bool train = true;

            GameExperiment experiment = new GameExperiment();

            // Load config XML.
            XmlDocument xmlConfig = new XmlDocument();
            xmlConfig.Load("expconfig.xml");
            experiment.Initialize("TicTacToe", xmlConfig.DocumentElement);

            if (!train)
            {
                #region Play
                NeatGenome genome = null;

                // Try to load the genome from the XML document.
                try
                {
                    using (XmlReader xr = XmlReader.Create(FILE))
                        genome = NeatGenomeXmlIO.ReadCompleteGenomeList(xr, false, (NeatGenomeFactory)experiment.CreateGenomeFactory())[0];
                }
                catch (Exception e1)
                {
                    return;
                }

                // Get a genome decoder that can convert genomes to phenomes.
                var genomeDecoder = experiment.CreateGenomeDecoder();

                // Decode the genome into a phenome (neural network).
                var phenome = genomeDecoder.Decode(genome);

                IPlayer p1 = new NeatBrain(phenome);
                IPlayer p2 = new RandomBrain();

                while (true)
                {

                    Game Game = new Game(p1, p2);
                    Game.PlayUntilFinished();

                    Draw(Game.Board);

                    Console.WriteLine(Game.GetGameState().ToString());
                    Console.ReadKey();
                    Console.WriteLine();
                }
                #endregion Play
            }
            else
            {
                #region Instructions
                Console.WriteLine("This is the training example. If you want to APPLY it, modify the boolean 'train' to false.");
                Console.WriteLine("Take into acocunt that training generates a file 'expconfig.xml' that you can load to play.");
                #endregion


                #region Train
                NeatEvolutionAlgorithm<NeatGenome> _ea;

                List<NeatGenome> initialPopulation;
                if (File.Exists(NETWORK_FILE))
                {
                    using (XmlReader xr = XmlReader.Create(NETWORK_FILE))
                        initialPopulation = NeatGenomeXmlIO.ReadCompleteGenomeList(xr, true, (NeatGenomeFactory)experiment.CreateGenomeFactory());

                    _ea = experiment.CreateEvolutionAlgorithm(initialPopulation);
                }
                else
                {
                    _ea = experiment.CreateEvolutionAlgorithm();
                }

                // Create evolution algorithm and attach update event.
                _ea.UpdateEvent += new EventHandler(ea_UpdateEvent);

                bool useUnnatendedTrain = false;
                if (useUnnatendedTrain)
                {
                    #region Unattended Train
                    // Start algorithm (it will run on a background thread).
                    _ea.StartContinue();
                    #endregion
                }
                else
                {
                    #region Attended Train
                    while (true)
                    {
                        //start the generation
                        _ea.StartGeneration();

                        //create offpring
                        CreateOffpringDTO<NeatGenome> offspringData = _ea.CreateOffpring();

                        //create genome decoder
                        var decoder = experiment.CreateGenomeDecoder();

                        //decode them all
                        Dictionary<NeatGenome, IBlackBox> decodedData = Enumerable.ToDictionary<NeatGenome, NeatGenome, IBlackBox>(
                                _ea.GenomeList,
                                x => x,
                                x => decoder.Decode(x)
                            );

                        //create all the possible games
                        var games = (
                            from x in decodedData.Keys
                            from y in decodedData.Keys
                            where x != y
                            select CreateGame(x, y, decodedData[x], decodedData[y])
                            
                        ).ToList();

                        //initialize dictionary
                        ConcurrentDictionary<NeatGenome, double> convertedAnswers = new ConcurrentDictionary<NeatGenome, double>();

                        //initialize data in the dictionary
                        foreach(NeatGenome x in decodedData.Keys)
                        {
                            convertedAnswers[x] = 0;
                        }

                        //play all games!
                        Parallel.ForEach(games, 
                            //new ParallelOptions { MaxDegreeOfParallelism = 1000},
                            game =>
                        {
                            game.Game.PlayUntilFinished();

                            lock (game.X)
                            {
                                convertedAnswers[game.X] += GameEvaluator.GetScore(game.Game.GetGameState(), true);
                            }
                            lock (game.Y)
                            {
                                convertedAnswers[game.Y] += GameEvaluator.GetScore(game.Game.GetGameState(), false);
                            }
                        });

                        //Convert to dictionary
                        Dictionary<NeatGenome, FitnessInfo> answers =
                            convertedAnswers.ToDictionary(x => x.Key, y => new FitnessInfo(y.Value, y.Value) );

                        //set answers
                        KnownAnswerListEvaluator<NeatGenome, IBlackBox> evaluator = new KnownAnswerListEvaluator<NeatGenome, IBlackBox>();
                        evaluator.SetKnownAnswers(answers);

                        //call the evaluator
                        evaluator.Evaluate(_ea.GenomeList);

                        //Update the species
                        _ea.UpdateSpecies(offspringData);

                        //call callbacks :)
                        _ea.PerformUpdateCallbacks();
                    }
                    #endregion Attended Train
                }
                Console.ReadLine();
                #endregion Train
            }
        }

        public class GameDTO
        {
            public IBlackBox P1 { get; set; }
            public IBlackBox P2 { get; set; }
            public Game Game { get; set; }
            public NeatGenome X { get; internal set; }
            public NeatGenome Y { get; internal set; }
        }

        private static GameDTO CreateGame(NeatGenome x, NeatGenome y, IBlackBox p1, IBlackBox p2)
        {
            IPlayer p1Brain = new NeatBrain(p1);
            IPlayer p2Brain = new NeatBrain(p2);

            return new GameDTO
            {
                P1 = p1,
                P2 = p2,
                X = x,
                Y = y,
                Game = new Game(p1Brain, p2Brain)
            };
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

            doc = NeatGenomeXmlIO.SaveComplete(_ea.GenomeList, true);
            doc.Save(NETWORK_FILE);
        }

        #region Draw
        private static void Draw(Board board)
        {
            WriteLine(board.Squares, 0);
            WriteLine(board.Squares, 1);
            WriteLine(board.Squares, 2);
        }

        private static void WriteLine(Square[,] squares, int line)
        {
            WriteCell(squares[0, line]);
            WriteCell(squares[1, line]);
            WriteCell(squares[2, line]);
            Console.WriteLine();
        }

        private static void WriteCell(Square square)
        {
            Console.Write(TranslateValue(square.State));
        }

        private static string TranslateValue(Square.StateEnum state)
        {
            switch (state)
            {
                case Square.StateEnum.Player1:
                    return "X";
                case Square.StateEnum.Player2:
                    return "O";
                default:
                    return " ";
            }
        }
        #endregion Draw
    }
}
