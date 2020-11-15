using DigitClassifier.Activation_Functions;
using System;
using System.IO;

namespace DigitClassifier
{
    public class Layer
    {
        readonly IActivationFunction activationFunction;

        private Matrix outputs;
        private Matrix weights;
        private Matrix errors;
        private Matrix deltas;

        public Layer(int numberOfInputs, int numberOfOutputs, IActivationFunction activationFunction)
        {
            this.activationFunction = activationFunction;
            weights = Matrix.Create(numberOfOutputs, numberOfInputs);
        }

        private Layer(IActivationFunction activationFunction, Matrix weights)//, Matrix outputs, Matrix errors, Matrix deltas)
        {
            this.activationFunction = activationFunction;
            this.weights = weights;
            //this.outputs = outputs;
            //this.errors = errors;
            //this.deltas = deltas;
        }

        public void Randomise()
        {
            Random r = new Random();
            weights.Initialize(() => r.NextDouble() - 0.5); // Distribute between -0.5 and 0.5
        }

        public double[] FeedForward(double[] inputs)
        {
            outputs = weights * Matrix.Create(inputs);
            return Matrix.Map(outputs, activationFunction.Function).ToArray(); // Apply activation function to outputs
        }

        public void TrainOutput(double[] targets, double[] prevLayerOutputs)
        {
            Matrix targetSignals = Matrix.Create(targets);
            errors = targetSignals - outputs;

            Matrix outputDerivatives = Matrix.Map(outputs, activationFunction.DerivativeFunction);
            Matrix gamma = errors * outputDerivatives;

            deltas = gamma * Matrix.Create(prevLayerOutputs).Transpose();
        }

        public void TrainHidden(Layer forwardLayer, double[] prevLayerOutputs)
        {
            errors = forwardLayer.weights.Transpose() * forwardLayer.errors;

            Matrix outputDerivatives = Matrix.Map(outputs, activationFunction.DerivativeFunction);
            Matrix gamma = errors * outputDerivatives;

            deltas = gamma * Matrix.Create(prevLayerOutputs).Transpose();
        }

        public void UpdateWeights(double learningRate)
        {
            weights += learningRate * deltas;
        }

        public void Serialize(BinaryWriter bw)
        {
            bw.Write(activationFunction.GetType().ToString());
            weights.Serialize(bw);
        }

        public static Layer Deserialize(BinaryReader br)
        {
            string type = br.ReadString();
            var getInstance = Type.GetType(type).GetMethod("GetInstance");
            Layer layer = new Layer(
                (IActivationFunction)getInstance.Invoke(null, new object[] { }),
                Matrix.Deserialize(br)
            );

            return layer;
        }
    }
}
