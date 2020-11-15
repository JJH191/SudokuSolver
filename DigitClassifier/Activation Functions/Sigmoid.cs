using System;

namespace DigitClassifier.Activation_Functions
{
    public class Sigmoid : IActivationFunction
    {
        private static Sigmoid instance;
        private Sigmoid() { }

        public double Function(double value)
        {
            return 1 / (1 + Math.Pow(Math.E, -value));
        }

        public double DerivativeFunction(double value)
        {
            double sigmoidedValue = 1 / (1 + Math.Pow(Math.E, -value));
            return sigmoidedValue * (1 - sigmoidedValue);
        }

        public static Sigmoid GetInstance()
        {
            if (instance == null) instance = new Sigmoid();
            return instance;
        }
    }
}
