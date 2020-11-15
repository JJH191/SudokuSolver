using System;

namespace DigitClassifier.Activation_Functions
{
    public class ReLU : IActivationFunction
    {
        private static ReLU instance;
        private ReLU() { }

        public double Function(double value)
        {
            return Math.Max(0, value);
        }

        public double DerivativeFunction(double value)
        {
            if (value < 0) return 0;
            else return 1;
        }

        public static ReLU GetInstance()
        {
            if (instance == null) instance = new ReLU();
            return instance;
        }
    }
}
