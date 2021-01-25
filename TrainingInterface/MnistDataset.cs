using Common;

namespace TrainingInterface
{
    /// <summary>
    /// Dataset including the hand-drawn digits from the MNIST dataset
    /// </summary>
    public class MnistDataset : IDataset
    {
        private readonly InputData[] inputData;

        public MnistDataset(string directory)
        {
            MnistLoader loader = new MnistLoader(directory, "mnist-digits-train-images-idx3-ubyte", "mnist-digits-train-labels-idx1-ubyte");
            inputData = loader.GetInputData();
        }

        public InputData[] GetData() => inputData;

        public void Shuffle()
        {
            inputData.Shuffle();
        }
    }
}
