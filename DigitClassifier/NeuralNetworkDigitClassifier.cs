using System.Drawing;
using System.Linq;

namespace DigitClassifier
{
    public class NeuralNetworkDigitClassifier : IDigitClassifier
    {
        private readonly NeuralNetwork classifier;

        public NeuralNetworkDigitClassifier(string neuralNetworkFilePath)
        {
            classifier = NeuralNetwork.Load(neuralNetworkFilePath);
        }

        public int GetDigit(Bitmap bitmap)
        {
            Bitmap scaledImage = new Bitmap(bitmap, 28, 28);
            double[] pixels = new double[28 * 28];

            for (int i = 0; i < scaledImage.Width; i++)
            {
                for (int j = 0; j < scaledImage.Height; j++)
                {
                    Color pixel = scaledImage.GetPixel(i, j);
                    pixels[j * scaledImage.Width + i] = pixel.GetBrightness() * 0.99 + 0.01;
                }
            }

            double[] response = classifier.Query(pixels);
            double max = response.Max(x => x);

            return response.ToList().IndexOf(max);
        }
    }
}
