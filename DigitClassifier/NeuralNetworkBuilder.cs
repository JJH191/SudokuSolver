using DigitClassifier.Activation_Functions;
using System;
using System.Collections.Generic;

namespace DigitClassifier
{
    /// <summary>
    /// A class to assist in building a neural network
    /// </summary>
    public class NeuralNetworkBuilder
    {
        private double learningRate; // Learning rate of the network
        private readonly List<LayerInfo> layers = new List<LayerInfo>(); // Create a list of information about layers

        // Booleans to keep track of whether the neural network has been fully defined
        private bool hasSetInputLayer = false;
        private bool hasSetOutputLayer = false;

        /// <summary>
        /// Sets the learning rate of the neural network to <paramref name="learningRate"/>
        /// </summary>
        /// <param name="learningRate">The learning rate</param>
        /// <returns>This <see cref="NeuralNetworkBuilder"/></returns>
        public NeuralNetworkBuilder SetLearningRate(double learningRate)
        {
            this.learningRate = learningRate;
            return this;
        }

        /// <summary>
        /// Sets the input of the neural network to have the specified number of <paramref name="nodes"/>
        /// </summary>
        /// <param name="nodes">The number of nodes in the input layer</param>
        /// <returns>This <see cref="NeuralNetworkBuilder"/></returns>
        public NeuralNetworkBuilder SetInputLayer(int nodes)
        {
            hasSetInputLayer = true; // The input layer has been specified
            layers.Add(new LayerInfo(nodes, null)); // Create a new layer for the input. The input layer has no activiation function, so null is used
            return this;
        }

        /// <summary>
        /// Sets the output of the neural network to have the specified number of <paramref name="nodes"/> and use the specified <paramref name="activationFunction"/>
        /// </summary>
        /// <param name="nodes">The number of nodes in the output layer</param>
        /// <param name="activationFunction">The activation function to apply to all the values in the output layer</param>
        /// <returns>This <see cref="NeuralNetworkBuilder"/></returns>
        public NeuralNetworkBuilder SetOutputLayer(int nodes, IActivationFunction activationFunction)
        {
            if (!hasSetInputLayer) throw new InvalidOperationException("Input layer must be set first");

            hasSetOutputLayer = true; // The output layer has been specified
            layers.Add(new LayerInfo(nodes, activationFunction)); // Create a new layer for output with the provided options
            return this;
        }

        /// <summary>
        /// Adds a hidden layer to the neural network that has the specified number of <paramref name="nodes"/> and uses the specified <paramref name="activationFunction"/>
        /// </summary>
        /// <param name="nodes">The number of nodes in the hidden layer</param>
        /// <param name="activationFunction">The activation function to apply to all the values in the hidden layer</param>
        /// <returns>This <see cref="NeuralNetworkBuilder"/></returns>
        public NeuralNetworkBuilder AddLayer(int nodes, IActivationFunction activationFunction)
        {
            if (!hasSetInputLayer) throw new InvalidOperationException("Input layer must be set first");

            layers.Add(new LayerInfo(nodes, activationFunction)); // Create a new hidden layer with the provided options
            return this;
        }

        public NeuralNetwork BuildNeuralNetwork()
        {
            // If an essential part of the network was not specified, throw an error
            if (learningRate == default) throw new MissingMemberException("Learning rate must be set");
            if (!hasSetInputLayer) throw new MissingMemberException("There must be an input layer");
            if (!hasSetOutputLayer) throw new MissingMemberException("There must be an output layer");

            // Create the layers - these are the connections between the nodes, so there is one less
            Layer[] networkLayers = new Layer[layers.Count - 1];
            for (int i = 0; i < networkLayers.Length; i++)
            {
                // Create a layer with the number of inputs of the LayerInfo at i and the number of outputs of the next layer
                // Use the activation function of the next layer
                networkLayers[i] = new Layer(layers[i].nodes, layers[i + 1].nodes, layers[i + 1].activationFunction);
            }

            // Create a new neural network with the specified layers and learning rate
            return NeuralNetwork.CreateNewNeuralNetwork(networkLayers, learningRate);
        }

        /// <summary>
        /// A class to keep track of the information needed for each layer
        /// </summary>
        class LayerInfo
        {
            public readonly int nodes; // The number of nodes in this layer
            public IActivationFunction activationFunction; // The activation function to apply to each element in this layer

            /// <summary>
            /// Create a container for the information about a layer - the number of <paramref name="nodes"/> and the <paramref name="activationFunction"/>
            /// </summary>
            /// <param name="nodes">The number of nodes in the layer</param>
            /// <param name="activationFunction">The activation function for that layer</param>
            public LayerInfo(int nodes, IActivationFunction activationFunction)
            {
                this.nodes = nodes;
                this.activationFunction = activationFunction;
            }
        }
    }
}
