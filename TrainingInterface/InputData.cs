using System.Linq;

namespace TrainingInterface
{
    /// <summary>
    /// A wrapper to hold the inputs and target for training the network
    /// </summary>
    public class InputData
    {
        public double[] targets; // The target values to reach
        public double[] inputs; // The input data

        public InputData(double[] inputs, int target)
        {
            // Create a new target array with 10 elements
            targets = new double[10];

            // Set all the values in the targets array to 0.01
            // I do not use 0 here as it would carry through the network causing it to not work properly
            for (int i = 0; i < targets.Length; i++)
            {
                if (i == target) targets[i] = 1; // Set the target index to 1
                else targets[i] = 0.01;
            }

            this.inputs = inputs;
        }
    }
}
