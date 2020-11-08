using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class SudokuImageModel
    {
        private static readonly int imageDisplaySize = 600;

        private Bitmap original;
        private Bitmap current;
        public Bitmap Image
        {
            get => current;
            set
            {
                // Scale the image so that it is quick to grayscale
                current = ResizeImage(value, imageDisplaySize);
                original = new Bitmap(current);
                Grayscale();
            }
        }

        public double Threshold { get; set; }

        // Not my code
        public void Grayscale()
        {
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

        // Not my code
        private Bitmap ResizeImage(Bitmap image, float _width)
        {
            int width = (int)_width;
            int height = (int)((_width / image.Width) * image.Height);

            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }
    }
}
