using Accord;
using Accord.Imaging.Filters;
using Common;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Models
{
    /// <summary>
    /// A class to store the sudoku image and provide the adjustments required to make it suitable for the digit classifier
    /// </summary>
    public class SudokuImageModel
    {
        // Size of the image - images are scaled down to this size to make filters run quicker
        private static readonly int imageSize = 1000;

        private Bitmap original; // Keeps track of the unfiltered image (so filter parameters can be tweaked)
        private Bitmap current; // Keeps track of the greyscale image

        // Public property used to access the sudoku image
        public Bitmap Image
        {
            get => current; // Return the greyscale
            set
            {
                current = value.Resize(imageSize); // Scale the image down
                original = new Bitmap(current); // Save the original image
                Greyscale(); // Greyscale the current image
            }
        }

        // Threshold of the greyscale - the cutoff brightness for what is considered black and what is considered white
        public double Threshold { get; set; }

        /// <summary>
        /// Makes the image greyscale with the given threshold value
        /// </summary>
        public void Greyscale()
        {
            current.Dispose();
            BradleyLocalThresholding filter = new BradleyLocalThresholding
            {
                PixelBrightnessDifferenceLimit = (float)Threshold * 0.45f // 0.45 gave the best starting point from testing
            };
            current = filter.Apply(Grayscale.CommonAlgorithms.BT709.Apply(original));
        }

        /// <summary>
        /// Transforms the image so it is square.
        /// The size of the new image is 28 * 9 * scaleFactor as each cell is eventually scaled down to 28 * 28, so it needs to be at least that size
        /// </summary>
        /// <param name="quad">The quadrilateral that defines where the sudoku is in the image</param>
        /// <returns>A square image that contains only the sudoku</returns>
        public Bitmap GetAdjustedImage(Vector2D[] quad)
        {
            int scaleFactor = 2;
            int size = 28 * 9 * scaleFactor; // Calculate the width and height of the image (they are the same as the image is square)

            List<IntPoint> intPoints = quad.Select(corner => (IntPoint)corner).ToList(); // Convert the quad to a list of IntPoints
            QuadrilateralTransformation filter = new QuadrilateralTransformation(intPoints, size, size); // Get transformation filter

            Bitmap transformedImage = filter.Apply(current); // Apply transformation to remove any perspective from the original image
            return transformedImage;
        }
    }
}
