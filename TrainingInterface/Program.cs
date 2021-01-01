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
            CreateAndTrainNetwork("trained_network.nn");
            //HandwrittenDigits.Run();
        
            Console.ReadLine();
        }
        
        /// <summary>
        /// Allows the provided <paramref name="classifier"/> to be tested with an image path
        /// </summary>
        /// <param name="classifier"></param>
        private static void TestNetwork(NeuralNetworkDigitClassifier classifier)
        {
            while (true)
            {
                Console.Write("Image path: "); // Ask for the image path
                string path = Console.ReadLine();
        
                Bitmap image = (Bitmap)Image.FromFile(path); // Load the image
                Console.WriteLine(classifier.GetDigit(image)); // Classify the digit
            }
        }
        
        /// <summary>
        /// Creates a new network, trains it, then saves it to the provided <paramref name="savePath"/>
        /// </summary>
        /// <param name="savePath"></param>
        private static void CreateAndTrainNetwork(string savePath)
        {
            // Build a new neural network
            NeuralNetworkBuilder networkBuilder = new NeuralNetworkBuilder();
            networkBuilder.SetLearningRate(0.3f) // was 0.035f
                .SetInputLayer(784) // 784 = 28 * 28 (the size of the input images)
                .AddLayer(100, Sigmoid.GetInstance())
                .SetOutputLayer(10, Sigmoid.GetInstance());
        
            NeuralNetwork network = networkBuilder.BuildNeuralNetwork();
        
            // Load in the dataset and shuffle it
            Console.WriteLine("Loading dataset");
            IDataset mnist = new MnistDataset("../res/mnist_train.csv");
            IDataset emnist = new EmnistDataset("../res");

            Console.WriteLine("Training network");

            emnist.Shuffle();
            InputData[] data = emnist.GetData();
            ProgressBar progressBar = new ProgressBar(data.Length, 75);
            for (int i = 0; i < data.Length; i++)
            {
                network.Train(data[i].inputs, data[i].targets); // Train the network with the provided data
                if (i % 10 == 0) progressBar.PrintProgress(i); // Print the progress bar every 10 iterations (do not need to do it every iteration)
            }
            Console.WriteLine();
        
            network.Save(savePath); // Save the network
            Console.WriteLine($"Done training. Saved to '{savePath}'");
        }

    }
}
