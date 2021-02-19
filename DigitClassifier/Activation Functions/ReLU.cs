using System;

namespace DigitClassifier.Activation_Functions
{
    /// <summary>
    /// Implementation of the rectified linear unit activation function
    /// </summary>
    public class ReLU : IActivationFunction
    {
        private static ReLU instance; // Keep track of singleton instance
        private ReLU() { } // Make the constructor private

        public double Function(double value)
        {
            return Math.Max(0, value);
        }

        public double DerivativeFunction(double value)
        {
            if (value < 0) return 0;
            else return 1;
        }

        // Handles creation of the singleton
        public static ReLU GetInstance()
        {
            if (instance == null) instance = new ReLU();
            return instance;
        }
    }
}
