namespace DigitClassifier.Activation_Functions
{
    public interface IActivationFunction
    {
        double Function(double value);
        double DerivativeFunction(double value); // This is after the activation function has been applied
    }
}
