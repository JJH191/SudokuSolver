namespace DigitClassifier.Activation_Functions
{
    /// <summary>
    /// An interface to provide the required methdos for an activation function
    /// </summary>
    public interface IActivationFunction
    {
        /// <summary>
        /// Applies the activation function to the provided <paramref name="value"/>
        /// </summary>
        /// <param name="value">The value to apply the function to</param>
        /// <returns>The result of applying the activation function to <paramref name="value"/></returns>
        double Function(double value);

        /// <summary>
        /// Applies the derivative of the activation function to the provided <paramref name="value"/>
        /// </summary>
        /// <param name="value">The value to apply the derivative function to</param>
        /// <returns>The result of applying the derivative of the activation function to <paramref name="value"/></returns>
        double DerivativeFunction(double value); // NOTE: This is after the activation function has been applied
    }
}
