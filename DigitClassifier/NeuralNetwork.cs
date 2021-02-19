using System.IO;
using System.Linq;

namespace DigitClassifier
{
    public class NeuralNetwork
    {
        private readonly double learningRate;
        private readonly Layer[] layers;

        private NeuralNetwork(Layer[] layers, double learningRate)
        {
            this.learningRate = learningRate;
            this.layers = layers;
        }

        /// <summary>
        /// Creates a new neural network with the specified <paramref name="layers"/> and <paramref name="learningRate"/>
        /// The weights of all the layers are randomised
        /// </summary>
        /// <param name="layers">A list of layers defining the shape of the neural network</param>
        /// <param name="learningRate">The rate at which the network learns</param>
        /// <returns></returns>
        public static NeuralNetwork CreateNewNeuralNetwork(Layer[] layers, double learningRate)
        {
            NeuralNetwork network = new NeuralNetwork(layers, learningRate);
            network.Randomise();
            return network;
        }

        /// <summary>
        /// Randomises all the weights in each layer
        /// </summary>
        private void Randomise()
        {
            foreach (Layer layer in layers) layer.Randomise();
        }

        /// <summary>
        /// Train the neural network with a set of <paramref name="inputs"/> and have it aim to reach the given <paramref name="targets"/>
        /// </summary>
        /// <param name="inputs">The inputs to the neural network</param>
        /// <param name="targets">The desired outputs for the given inputs</param>
        public void Train(double[] inputs, double[] targets)
        {
            double[][] layerInputs = new double[layers.Length + 1][]; // The outputs from each layer to be used as the inputs for the next layer
            layerInputs[0] = inputs; // The first set of inputs

            // Calculate the outputs for each layer and save them in layerInputs for the next layer to use as inputs
            for (int i = 0; i < layers.Length; i++)
                layerInputs[i + 1] = layers[i].FeedForward(layerInputs[i]);

            // Backpropagate through all the layers and calculate the deltas
            for (int i = layers.Length - 1; i >= 0; i--)
            {
                if (i == layers.Length - 1) layers[i].TrainOutput(targets, layerInputs[i]);
                else layers[i].TrainHidden(layers[i + 1], layerInputs[i]);
            }

            // Update the weights of each layer based on the deltas and learning rate
            foreach (Layer l in layers) l.UpdateWeights(learningRate);
        }

        /// <summary>
        /// Query the network with a given set of <paramref name="inputs"/>
        /// </summary>
        /// <param name="inputs">The inputs to provide the neural network with</param>
        /// <returns>The outputs from the network</returns>
        public double[] Query(double[] inputs)
        {
            double[][] layerInputs = new double[layers.Length + 1][];// The outputs from each layer to be used as the inputs for the next layer
            layerInputs[0] = inputs; // The first set of inputs

            // Calculate the outputs for each layer and save them in layerInputs for the next layer to use as inputs
            for (int i = 0; i < layers.Length; i++)
                layerInputs[i + 1] = layers[i].FeedForward(layerInputs[i]);

            return layerInputs.Last(); // Return the output of the last layer
        }

        /// <summary>
        /// Save the weights and shape of the network to a given <paramref name="filepath"/>
        /// </summary>
        /// <param name="filepath">The path to save the neural network to</param>
        public void Save(string filepath)
        {
            // Create a filestream and binary writer from that
            FileStream fs = File.OpenWrite(filepath);
            BinaryWriter bw = new BinaryWriter(fs);

            // Save the learning rate and number of layers
            bw.Write(learningRate);
            bw.Write(layers.Length);

            // Save all the weights in each layer
            foreach (Layer layer in layers)
                layer.Serialise(bw);

            fs.Close();
        }

        /// <summary>
        /// Load a neural network from a give <paramref name="filepath"/>
        /// </summary>
        /// <param name="filepath">The path of the saved network</param>
        /// <returns>The network loaded from the given path</returns>
        public static NeuralNetwork Load(string filepath)
        {
            // Create a filestream and binary reader from that
            FileStream fs = File.OpenRead(filepath);
            BinaryReader br = new BinaryReader(fs);

            // Read the learning rate
            double learningRate = br.ReadDouble();

            // Read all the layer sizes and weights
            int noOfLayers = br.ReadInt32();
            Layer[] layers = new Layer[noOfLayers];

            for (int i = 0; i < noOfLayers; i++)
            {
                if (fs.Length == 0) break;
                layers[i] = Layer.Deserialise(br);
            }

            fs.Close();

            return new NeuralNetwork(layers.ToArray(), learningRate);
        }
    }
}