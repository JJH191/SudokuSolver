using DigitClassifier.Activation_Functions;
using System;
using System.Collections.Generic;

namespace DigitClassifier
{
    public class NeuralNetworkBuilder
    {
        private double learningRate;
        private readonly List<TempLayer> layers = new List<TempLayer>();

        private bool hasSetInputLayer = false;
        private bool hasSetOutputLayer = false;

        public NeuralNetworkBuilder SetLearningRate(double learningRate)
        {
            this.learningRate = learningRate;
            return this;
        }

        public NeuralNetworkBuilder SetInputLayer(int nodes)
        {
            hasSetInputLayer = true;
            layers.Add(new TempLayer(nodes, null));
            return this;
        }

        public NeuralNetworkBuilder SetOutputLayer(int nodes, IActivationFunction activationFunction)
        {
            hasSetOutputLayer = true;
            layers.Add(new TempLayer(nodes, activationFunction));
            return this;
        }

        public NeuralNetworkBuilder AddLayer(int nodes, IActivationFunction activationFunction)
        {
            layers.Add(new TempLayer(nodes, activationFunction));
            return this;
        }

        public NeuralNetwork BuildNeuralNetwork()
        {
            if (learningRate == default) throw new MissingMemberException("Learning rate must be set");
            if (!hasSetInputLayer) throw new MissingMemberException("There must be an input layer");
            if (!hasSetOutputLayer) throw new MissingMemberException("There must be an output layer");

            Layer[] networkLayers = new Layer[layers.Count - 1];
            for (int i = 0; i < networkLayers.Length; i++)
            {
                networkLayers[i] = new Layer(layers[i].nodes, layers[i + 1].nodes, layers[i + 1].activationFunction);
            }

            return NeuralNetwork.CreateNewNeuralNetwork(networkLayers, learningRate);
        }

        class TempLayer
        {
            public readonly int nodes;
            public IActivationFunction activationFunction;

            public TempLayer(int nodes, IActivationFunction activationFunction)
            {
                this.nodes = nodes;
                this.activationFunction = activationFunction;
            }
        }
    }
}
