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
            weights = new Matrix(numberOfOutputs, numberOfInputs);
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
            weights.Map((_) => r.NextDouble() - 0.5); // Distribute between -0.5 and 0.5
        }

        public double[] FeedForward(double[] inputs)
        {
            outputs = weights * new Matrix(inputs);

            return Matrix.Map(outputs, activationFunction.Function).ToArray(); // Apply activation function to outputs
        }

        public void TrainOutput(double[] targets, double[] prevLayerOutputs)
        {
            Matrix targetSignals = new Matrix(targets);
            errors = targetSignals - outputs;


            Matrix outputDerivatives = Matrix.Map(outputs, activationFunction.DerivativeFunction);
            Matrix gamma = Matrix.ScalarMultiply(errors, outputDerivatives);

            deltas = gamma * new Matrix(prevLayerOutputs).GetTransposed();
        }

        public void TrainHidden(Layer forwardLayer, double[] prevLayerOutputs)
        {
            errors = forwardLayer.weights.GetTransposed() * forwardLayer.errors;

            Matrix outputDerivatives = Matrix.Map(outputs, activationFunction.DerivativeFunction);
            Matrix gamma = Matrix.ScalarMultiply(errors, outputDerivatives);

            deltas = gamma * new Matrix(prevLayerOutputs).GetTransposed();
        }

        public void UpdateWeights(double learningRate)
        {
            weights += deltas * learningRate;
        }

        public void Serialise(BinaryWriter bw)
        {
            bw.Write(activationFunction.GetType().ToString());
            weights.Serialise(bw);
        }

        public static Layer Deserialise(BinaryReader br)
        {
            string type = br.ReadString();
            var getInstance = Type.GetType(type).GetMethod("GetInstance");
            Layer layer = new Layer(
                (IActivationFunction)getInstance.Invoke(null, new object[] { }),
                Matrix.Deserialise(br)
            );

            return layer;
        }
    }
}
