using System;

namespace DigitClassifier.Activation_Functions
{
    /// <summary>
    /// Implementation of the sigmoid activation function
    /// </summary>
    public class Sigmoid : IActivationFunction
    {
        private static Sigmoid instance; // Keep track of singleton instance
        private Sigmoid() { } // Make the constructor private

        public double Function(double value)
        {
            return 1 / (1 + Math.Pow(Math.E, -value));
        }

        public double DerivativeFunction(double value)
        {
            double sigmoidedValue = 1 / (1 + Math.Pow(Math.E, -value));
            return sigmoidedValue * (1 - sigmoidedValue);
        }

        // Handles creation of the singleton
        public static Sigmoid GetInstance()
        {
            if (instance == null) instance = new Sigmoid();
            return instance;
        }
    }
}
