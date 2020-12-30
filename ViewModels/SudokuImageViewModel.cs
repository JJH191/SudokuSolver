using HelperClasses;
using Models;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Media.Imaging;

namespace ViewModels
{
    public class SudokuImageViewModel : INotifyPropertyChanged
    {
        private readonly SudokuImageModel sudokuImage = new SudokuImageModel();

        public Bitmap Image
        {
            get => sudokuImage.Image;
            set
            {
                sudokuImage.Image = value;
                Notify(nameof(Image));
            }
        }


        public double Threshold
        {
            get => sudokuImage.Threshold;
            set
            {
                sudokuImage.Threshold = value;
                Notify(nameof(Threshold));
            }
        }

        public void Grayscale()
        {
            sudokuImage.Grayscale();
            Notify(nameof(Image));
        }

        public Bitmap GetAdjustedImage(QuadViewModel quad)
        {
            Vector2D[] clockwisePoints = quad.GetModel().GetClockwiseHull();

            // TODO: deal with invalid corner
            if (clockwisePoints.Length != 4) throw new Exception("Invalid corners!");

            return sudokuImage.GetAdjustedImage(clockwisePoints);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void Notify(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}