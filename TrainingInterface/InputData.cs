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
            // I do not use 0 here as it would carry through the network causing it to not work properly
            targets = Enumerable.Range(0, 10).Select(x => 0.01).ToArray(); // Set all the values in the targets array to 0.01
            targets[target] = 1; // Set the target index to 1

            this.inputs = inputs;
        }
    }
}
