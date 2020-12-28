using DigitClassifier;
using DigitClassifier.Activation_Functions;
using System;
using System.Drawing;

namespace TrainingInterface
{
    class Program
    {
        static void Main()
        {
            //TestNetwork(new NeuralNetworkDigitClassifier("trained_network.nn"));
            //CreateAndTrainNetwork("trained_network.nn");

            Console.ReadLine();
        }

        private static void TestNetwork(NeuralNetworkDigitClassifier classifier)
        {
            while (true)
            {
                Console.Write("Image path: ");
                string path = Console.ReadLine();

                Bitmap image = (Bitmap)Image.FromFile(path);
                Console.WriteLine(classifier.GetDigit(image));
            }
        }

        private static void CreateAndTrainNetwork(string savePath)
        {
            NeuralNetworkBuilder networkBuilder = new NeuralNetworkBuilder();
            networkBuilder.SetLearningRate(0.035f) // was 0.035f
                .SetInputLayer(784)
                .AddLayer(150, Sigmoid.GetInstance())
                .AddLayer(100, Sigmoid.GetInstance())
                .SetOutputLayer(10, Sigmoid.GetInstance());

            NeuralNetwork network = networkBuilder.BuildNeuralNetwork();

            Console.WriteLine("Loading dataset");
            IDataset mnist = new MnistDataset("../res/mnist_train.csv");
            mnist.Shuffle();

            Console.WriteLine("Training network");

            //foreach (InputData inputData in mnist.GetData())
            //    network.Train(inputData.inputs, inputData.targets);

            InputData[] data = mnist.GetData();
            ProgressBar progressBar = new ProgressBar(data.Length, 75);
            for (int i = 0; i < data.Length; i++)
            {

                network.Train(data[i].inputs, data[i].targets);
                if (i % 1 == 0)
                {
                    progressBar.PrintProgress(i);
                    //    Console.WriteLine($"{i}");
                    //    Console.SetCursorPosition(0, Console.CursorTop - 1);
                }
            }
            Console.WriteLine();

            network.Save(savePath);
            Console.WriteLine($"Done training. Saved to '{savePath}'");
        }
    }
}
