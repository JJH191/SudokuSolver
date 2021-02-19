using DigitClassifier.Activation_Functions;
using System;
using System.IO;

namespace DigitClassifier
{
    /// <summary>
    /// 
    /// </summary>
    public class Layer
    {
        // The activation function to apply after multiplying the inputs by the weights
        private readonly IActivationFunction activationFunction;

        private Matrix outputs; // The output after multiplying by the weights and applying the activation function
        private Matrix weights; // The weights of each input for the layer
        private Matrix errors; // The difference between the output and target values
        private Matrix deltas; // The amount to adjust each weight by

        private readonly Random r = new Random();

        #region Constructors
        /// <summary>
        /// Create a layer with the specified <paramref name="numberOfInputs"/> and <paramref name="numberOfOutputs"/> along with an <paramref name="activationFunction"/>
        /// </summary>
        /// <param name="numberOfInputs">The number of input values to the layer</param>
        /// <param name="numberOfOutputs">The number of output values the layer will produce</param>
        /// <param name="activationFunction">The activation function to apply after multiplying the inputs by the weights</param>
        public Layer(int numberOfInputs, int numberOfOutputs, IActivationFunction activationFunction)
        {
            this.activationFunction = activationFunction;
            weights = new Matrix(numberOfOutputs, numberOfInputs);
        }

        /// <summary>
        /// Create a layer with already defined <paramref name="weights"/> and an <paramref name="activationFunction"/>
        /// </summary>
        /// <param name="activationFunction">The activation function to apply after multiplying the inputs by the weights</param>
        /// <param name="weights">The weights for the layer to apply to the inputs</param>
        private Layer(IActivationFunction activationFunction, Matrix weights)
        {
            this.activationFunction = activationFunction;
            this.weights = weights;
        }
        #endregion

        /// <summary>
        /// Randomise all the weights between -0.5 and 0.5
        /// </summary>
        public void Randomise()
        {
            weights.Map((_) => r.NextDouble() - 0.5); // Distribute between -0.5 and 0.5
        }

        /// <summary>
        /// Feed the provided <paramref name="inputs"/> through the layer and return the outputs
        /// This will multiply the inputs by the weights then apply the activation function
        /// </summary>
        /// <param name="inputs">The input values to feed into the layer</param>
        /// <returns>The outputs after feeding the inputs through the layer</returns>
        public double[] FeedForward(double[] inputs)
        {
            outputs = weights * new Matrix(inputs);

            return Matrix.Map(outputs, activationFunction.Function).ToArray(); // Apply activation function to outputs
        }

        #region Training
        /// <summary>
        /// Trains the layer if it is the output layer of the network
        /// </summary>
        /// <param name="targets">The values the layer should aim to reach</param>
        /// <param name="prevLayerOutputs">The outputs from the previous layer</param>
        public void TrainOutput(double[] targets, double[] prevLayerOutputs)
        {
            Matrix targetSignals = new Matrix(targets);
            errors = targetSignals - outputs;

            Matrix outputDerivatives = Matrix.Map(outputs, activationFunction.DerivativeFunction);
            Matrix gamma = Matrix.ScalarProduct(errors, outputDerivatives);

            deltas = gamma * new Matrix(prevLayerOutputs).GetTransposed();
        }

        /// <summary>
        /// Trains the layer if it is the output layer of the network
        /// </summary>
        /// <param name="forwardLayer">The next layer to feed into</param>
        /// <param name="prevLayerOutputs">The outputs from the previous layer</param>
        public void TrainHidden(Layer forwardLayer, double[] prevLayerOutputs)
        {
            errors = forwardLayer.weights.GetTransposed() * forwardLayer.errors;

            Matrix outputDerivatives = Matrix.Map(outputs, activationFunction.DerivativeFunction);
            Matrix gamma = Matrix.ScalarProduct(errors, outputDerivatives);

            deltas = gamma * new Matrix(prevLayerOutputs).GetTransposed();
        }

        /// <summary>
        /// Update the weights after training the layer by an amount specified by <paramref name="learningRate"/>
        /// </summary>
        /// <param name="learningRate">The rate at which the weights will adjust</param>
        public void UpdateWeights(double learningRate)
        {
            weights += deltas * learningRate;
        }
        #endregion

        #region Serialisation
        /// <summary>
        /// Write the information needed to recreate this layer to the provided <paramref name="bw"/>
        /// </summary>
        /// <param name="bw">The binary writer to write the data of this layer to</param>
        public void Serialise(BinaryWriter bw)
        {
            bw.Write(activationFunction.GetType().ToString()); // Save the activation function
            weights.Serialise(bw); // Save the weights
        }

        /// <summary>
        /// Read the data from the <paramref name="br"/> and create a layer from it
        /// </summary>
        /// <param name="br">The binary reader to read the data from</param>
        /// <returns></returns>
        public static Layer Deserialise(BinaryReader br)
        {
            string type = br.ReadString(); // Gets the activation function name
            var getInstance = Type.GetType(type).GetMethod("GetInstance");

            Layer layer = new Layer(
                (IActivationFunction)getInstance.Invoke(null, new object[] { }), // Create the activation function
                Matrix.Deserialise(br) // Read the weights from the bianry reader
            );

            return layer;
        }
        #endregion
    }
}
