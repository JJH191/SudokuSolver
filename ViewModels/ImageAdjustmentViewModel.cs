using Models;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ViewModels
{
    public class ImageAdjustmentViewModel : INotifyPropertyChanged
    {
        private readonly SudokuImageModel sudokuImage = new SudokuImageModel();
        private readonly QuadModel quad = new QuadModel();

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

        public System.Windows.Shapes.Path Quad
        {
            get
            {
                System.Windows.Shapes.Path p = new System.Windows.Shapes.Path {
                    Fill = new SolidColorBrush(Colors.Wheat),
                    Stroke = new SolidColorBrush(Colors.Fuchsia)
                };

                PathGeometry pathGeometry = new PathGeometry();
                for (int i = 0; i < quad.Length; i++)
                    pathGeometry.AddGeometry(new LineGeometry(quad[i], quad[(i + 1) % quad.Length]));

                p.Data = pathGeometry;

                return p;
            }
        }

        public Point[] Corners
        { 
            get {
                Point[] points = new Point[quad.Length];

                for (int i = 0; i < quad.Length; i++)
                {
                    points[i] = new Point((int)quad[i].X, (int)quad[i].Y);
                }

                return points;
            }
        }

        public void Grayscale()
        {
            sudokuImage.Grayscale();
            Notify(nameof(BitmapImage));
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