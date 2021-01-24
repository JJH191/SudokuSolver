using Common;
using Models;
using System.ComponentModel;
using System.Drawing;

namespace ViewModels
{
    /// <summary>
    /// View model to provide the image of the sudoku
    /// </summary>
    public class SudokuImageViewModel : INotifyPropertyChanged
    {
        private readonly SudokuImageModel sudokuImage = new SudokuImageModel();

        // Public property to access the image
        public Bitmap Image
        {
            get => sudokuImage.Image; 
            set
            {
                sudokuImage.Image = value; // Set the new value
                Notify(nameof(Image)); // Notify the UI of a change
            }
        }

        // Public property to access the threshold value for the black and white filter
        public double Threshold
        {
            get => sudokuImage.Threshold;
            set
            {
                sudokuImage.Threshold = value; // Set the new threshold
                Notify(nameof(Threshold)); // Notify the UI it has changed
            }
        }

        /// <summary>
        /// Convert the current image to greyscale
        /// </summary>
        public void Greyscale()
        {
            sudokuImage.Greyscale(); // Make the image greyscale
            Notify(nameof(Image)); // Notify the UI of the change
        }

        /// <summary>
        /// Correct the perspective of the image with the given <paramref name="quad"/>
        /// </summary>
        /// <param name="quad">The quadrilateral representing the corners of the sudoku</param>
        /// <returns>The perspective corrected image</returns>
        public Bitmap GetAdjustedImage(QuadViewModel quad)
        {
            // Get the corners on the hull of the quad in a clockwise order
            Vector2D[] clockwisePoints = quad.GetModel().GetClockwiseHull();

            if (clockwisePoints == null) // If the points are invalid
                return null;

            return sudokuImage.GetAdjustedImage(clockwisePoints);
        }

        // Notifier for when a property changes
        public event PropertyChangedEventHandler PropertyChanged;
        private void Notify(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}