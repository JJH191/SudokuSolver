using Common;

namespace TrainingInterface
{
    /// <summary>
    /// Dataset including the hand-drawn digits from the EMNIST dataset
    /// </summary>
    public class EmnistDataset : IDataset
    {
        private readonly InputData[] inputData;

        public EmnistDataset(string directory)
        {
            MnistLoader loader = new MnistLoader(directory, "emnist-digits-train-images-idx3-ubyte", "emnist-digits-train-labels-idx1-ubyte");
            inputData = loader.GetInputData();
        }

        public void Shuffle()
        {
            inputData.Shuffle();
        }

        public InputData[] GetData() => inputData;
    }
}
