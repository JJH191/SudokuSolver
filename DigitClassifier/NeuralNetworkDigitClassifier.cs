using Common;
using System.Drawing;
using System.Linq;

namespace DigitClassifier
{
    /// <summary>
    /// A digit classifier using a neural network
    /// </summary>
    public class NeuralNetworkDigitClassifier : IDigitClassifier
    {
        // Pre-trained neural network
        private readonly NeuralNetwork classifier;

        /// <summary>
        /// Create a new digit calssifier using a pre-trained neural network from the path <paramref name="neuralNetworkFilePath"/>
        /// </summary>
        /// <param name="neuralNetworkFilePath">The path of the pre-trained network</param>
        public NeuralNetworkDigitClassifier(string neuralNetworkFilePath)
        {
            // Load the pre-trained network
            classifier = NeuralNetwork.Load(neuralNetworkFilePath);
        }

        /// <summary>
        /// Classifies the digit in the provided <paramref name="bitmap"/>
        /// </summary>
        /// <param name="bitmap">An image of a digit</param>
        /// <returns>The digit that was recognised in the <paramref name="bitmap"/></returns>
        public int GetDigit(Bitmap bitmap)
        {
            Bitmap scaledImage = new Bitmap(bitmap, 28, 28); // Scale the iamge to the size the neural network understands
            double[] pixels = new double[28 * 28]; // Create an array for the pixels in the bitmap

            for (int i = 0; i < scaledImage.Width; i++)
            {
                for (int j = 0; j < scaledImage.Height; j++)
                {
                    Color pixel = scaledImage.GetPixel(i, j); // Get the colour at each pixel

                    pixels[j * scaledImage.Width + i] = Utils.MapRange(pixel.GetBrightness(), 0, 1, 0.01, 1); // Get the brightness, map it so its between 0.01 and 1 (to prevents zeros propagating through the network), then add it to the pixels array
                }
            }

            // Query the network
            double[] response = classifier.Query(pixels);
            double max = response.Max(x => x); // Get the maximum value in the response (this is the one the network is most confident about)

            return response.ToList().IndexOf(max); // The digit is the index of the highest value in the response
        }

        // TODO (EXTRA FEATURE): Implement second most likely digit
    }
}
