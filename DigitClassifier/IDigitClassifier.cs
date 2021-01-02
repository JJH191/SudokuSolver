using System.Drawing;

namespace DigitClassifier
{
    /// <summary>
    /// Interface for a class that can classify the digit provided in a bitmap
    /// </summary>
    public interface IDigitClassifier
    {
        /// <summary>
        /// Classifies the digit in the provided <paramref name="bitmap"/>
        /// </summary>
        /// <param name="bitmap">An image of the digit to classify</param>
        /// <returns>The digit recognised from the <paramref name="bitmap"/></returns>
        int GetDigit(Bitmap bitmap);
    }
}
