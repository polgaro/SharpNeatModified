using SharpNeat.Phenomes;
using System;
using System.Collections.Generic;
using System.Text;

namespace XorCalculator
{
    public class XorBrain
    {
        IBlackBox _brain;
        public XorBrain(IBlackBox brain)
        {
            _brain = brain;
        }

        internal int Calculate(int input1, int input2)
        {
            // Clear the network
            _brain.ResetState();

            //set inputs
            _brain.InputSignalArray[0] = input1;
            _brain.InputSignalArray[1] = input2;

            // Activate the network
            _brain.Activate();

            return (_brain.OutputSignalArray[0] == 1) ? 1 : 0;

            //return (_brain.OutputSignalArray[0] > 0.5) ? 1 : 0;
        }
    }
}
