using Common;
using System;
using System.IO;
using System.Linq;

namespace TrainingInterface
{
    /// <summary>
    /// Dataset including the hand-drawn digits from the MNIST dataset
    /// </summary>
    public class MnistDataset : IDataset
    {
        private readonly InputData[] inputData;

        public MnistDataset(string path)
        {
            string[] lines = File.ReadAllLines(path);

            // TODO (ESSENTIAL): Not my code (https://github.com/Hagsten/NeuralNetwork/blob/master/NeuralNetwork/Problems/HandwrittenDigits.cs)
            // TODO (ESSENTIAL): CHECK!!!!!
            inputData = new InputData[lines.Length];
            for (int i = 0; i < lines.Length; i++)
            {
                string[] targetAndInputs = lines[i].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries); // First element is the target, the rest are the inputs

                // TODO (CLEANING): Move normalising to a utils class (e.g. maprange function)
                double[] inputs = targetAndInputs.Skip(1).Select(x => Utils.MapRange(double.Parse(x), 0, 255, 0.01, 1)).ToArray(); // Convert inputs from string to double and normalise between 0.01 and 1
                int target = int.Parse(targetAndInputs[0]);

                inputData[i] = new InputData(inputs, target);
            }
        }

        public InputData[] GetData() => inputData;

        public void Shuffle()
        {
            inputData.Shuffle();
        }
    }
}
