using System;
using System.IO;
using System.Linq;

namespace TrainingInterface
{
    public class MnistDataset : IDataset
    {
        private readonly InputData[] inputData;
        private readonly Random rnd = new Random();

        public MnistDataset(string path)
        {
            string[] lines = File.ReadAllLines(path);

            inputData = new InputData[lines.Length];
            for (int i = 0; i < lines.Length; i++)
            {
                string[] targetAndInputs = lines[i].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries); // First element is the target, the rest are the inputs
                double[] inputs = targetAndInputs.Skip(1).Select(x => double.Parse(x) / 255 * 0.99 + 0.01).ToArray(); // Convert inputs from string to double and normalise between 0.01 and 0.99
                int target = int.Parse(targetAndInputs[0]);

                inputData[i] = new InputData(inputs, target);
            }
        }

        public InputData[] GetData()
        {
            return inputData;
        }

        public void Shuffle()
        {
            int n = inputData.Length;
            while (n > 1)
            {
                int k = rnd.Next(n--);
                InputData temp = inputData[n];
                inputData[n] = inputData[k];
                inputData[k] = temp;
            }
        }
    }
}
