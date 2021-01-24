using Accord;
using Accord.Imaging.Filters;
using Common;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
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

        public void Greyscale()
        {
            // Code from https://stackoverflow.com/questions/2746103/what-would-be-a-good-true-black-and-white-colormatrix
            using (Graphics gr = Graphics.FromImage(current))
            {
                var gray_matrix = new float[][] {
                    new float[] { 0.299f, 0.299f, 0.299f, 0, 0 },
                    new float[] { 0.587f, 0.587f, 0.587f, 0, 0 },
                    new float[] { 0.114f, 0.114f, 0.114f, 0, 0 },
                    new float[] { 0,      0,      0,      1, 0 },
                    new float[] { 0,      0,      0,      0, 1 }
                };

                var ia = new ImageAttributes();
                ia.SetColorMatrix(new ColorMatrix(gray_matrix));
                ia.SetThreshold((float)Threshold); // Change this threshold as needed

                var rc = new Rectangle(0, 0, original.Width, original.Height);
                gr.DrawImage(original, rc, 0, 0, original.Width, original.Height, GraphicsUnit.Pixel, ia);
            }
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
