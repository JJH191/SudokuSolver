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

        public static NeuralNetwork CreateNewNeuralNetwork(Layer[] layers, double learningRate)
        {
            NeuralNetwork network = new NeuralNetwork(layers, learningRate);
            network.Randomise();
            return network;
        }

        private void Randomise()
        {
            foreach (Layer layer in layers) layer.Randomise();
        }

        public void Train(double[] inputs, double[] targets)
        {
            double[][] outputs = new double[layers.Length + 1][];
            outputs[0] = inputs;

            // Calculate the outputs for each layer
            for (int i = 0; i < layers.Length; i++)
                outputs[i + 1] = layers[i].FeedForward(outputs[i]);

            // Backpropagate
            for (int i = layers.Length - 1; i >= 0; i--)
            {
                if (i == layers.Length - 1) layers[i].TrainOutput(targets, outputs[i]);
                else layers[i].TrainHidden(layers[i + 1], outputs[i]);
            }

            foreach (Layer l in layers) l.UpdateWeights(learningRate);
        }

        public double[] Query(double[] inputs)
        {
            double[][] values = new double[layers.Length + 1][];
            values[0] = inputs;

            for (int i = 0; i < layers.Length; i++)
                values[i + 1] = layers[i].FeedForward(values[i]);

            return values.Last();
        }

        public void Save(string filepath)
        {
            FileStream fs = File.OpenWrite(filepath);
            BinaryWriter bw = new BinaryWriter(fs);

            bw.Write(learningRate);
            bw.Write(layers.Length);

            foreach (Layer layer in layers)
                layer.Serialise(bw);

            fs.Close();
        }

        public static NeuralNetwork Load(string filepath)
        {
            FileStream fs = File.OpenRead(filepath);
            BinaryReader br = new BinaryReader(fs);

            double learningRate = br.ReadDouble();

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