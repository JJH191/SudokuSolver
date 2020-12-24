using HelperClasses;
using Models;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;

namespace ViewModels
{
    public class SudokuImageViewModel : INotifyPropertyChanged
    {
        private readonly SudokuImageModel sudokuImage = new SudokuImageModel();

        public BitmapImage BitmapImage
        {
            get => Bitmap2BitmapImage(sudokuImage.Image);
            set
            {
                sudokuImage.Image = BitmapImage2Bitmap(value);
                Notify(nameof(BitmapImage));
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
            Notify(nameof(BitmapImage));
        }

        public Bitmap GetAdjustedImage(QuadViewModel quad, double ActualWidth)
        {
            // Due to DPI scaling, the width on the canvas is not always the same as the actual width of the image, so I scale the image to account for this so the quad ends up in the right spot
            double scaleFactor = sudokuImage.Image.Width / ActualWidth;
            PointPos[] clockwisePoints = quad.GetModel().GetClockwisePoints().Select(point => point * scaleFactor).ToArray();

            return sudokuImage.GetAdjustedImage(clockwisePoints);
        }

        // Not my code
        private Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
        {
            if (bitmapImage == null) return null;

            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);

                Bitmap bitmap = new Bitmap(outStream);
                return new Bitmap(bitmap);
            }
        }

        private BitmapImage Bitmap2BitmapImage(Bitmap bitmap)
        {
            if (bitmap == null) return null;
            using (MemoryStream ms = new MemoryStream())
            {
                bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                ms.Position = 0;

                BitmapImage bImg = new BitmapImage();
                bImg.BeginInit();
                bImg.StreamSource = new MemoryStream(ms.ToArray());
                bImg.EndInit();

                return bImg;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void Notify(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}