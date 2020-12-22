using Accord;
using Accord.Imaging.Filters;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using ViewModels;

namespace Models
{
    public class SudokuImageModel
    {
        private static readonly int imageDisplaySize = 500;

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

        // TODO: Not my code
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
                //ia.SetThreshold((float)Threshold); // Change this threshold as needed

                var rc = new Rectangle(0, 0, original.Width, original.Height);
                gr.DrawImage(original, rc, 0, 0, original.Width, original.Height, GraphicsUnit.Pixel, ia);
            }

            // TODO: Try neural network with greyscale rather than pure black/white
            current = AdjustContrast(current, (int)(Threshold * 200));
        }

        // NOT MY CODE
        public static Bitmap AdjustContrast(Bitmap Image, float Value)
        {
            Value = (100.0f + Value) / 100.0f;
            Value *= Value;
            Bitmap NewBitmap = (Bitmap)Image.Clone();
            BitmapData data = NewBitmap.LockBits(
                new Rectangle(0, 0, NewBitmap.Width, NewBitmap.Height),
                ImageLockMode.ReadWrite,
                NewBitmap.PixelFormat);
            int Height = NewBitmap.Height;
            int Width = NewBitmap.Width;

            unsafe
            {
                for (int y = 0; y < Height; ++y)
                {
                    byte* row = (byte*)data.Scan0 + (y * data.Stride);
                    int columnOffset = 0;
                    for (int x = 0; x < Width; ++x)
                    {
                        byte B = row[columnOffset];
                        byte G = row[columnOffset + 1];
                        byte R = row[columnOffset + 2];

                        float Red = R / 255.0f;
                        float Green = G / 255.0f;
                        float Blue = B / 255.0f;
                        Red = (((Red - 0.5f) * Value) + 0.5f) * 255.0f;
                        Green = (((Green - 0.5f) * Value) + 0.5f) * 255.0f;
                        Blue = (((Blue - 0.5f) * Value) + 0.5f) * 255.0f;

                        int iR = (int)Red;
                        iR = iR > 255 ? 255 : iR;
                        iR = iR < 0 ? 0 : iR;
                        int iG = (int)Green;
                        iG = iG > 255 ? 255 : iG;
                        iG = iG < 0 ? 0 : iG;
                        int iB = (int)Blue;
                        iB = iB > 255 ? 255 : iB;
                        iB = iB < 0 ? 0 : iB;

                        row[columnOffset] = (byte)iB;
                        row[columnOffset + 1] = (byte)iG;
                        row[columnOffset + 2] = (byte)iR;

                        columnOffset += 4;
                    }
                }
            }

            NewBitmap.UnlockBits(data);

            return NewBitmap;
        }

        public Bitmap GetAdjustedImage(PointPos[] quad)
        {
            QuadrilateralTransformation filter = new QuadrilateralTransformation(quad.Select(corner => (IntPoint)corner).ToList(), 28 * 9 * 2, 28 * 9 * 2);
            var transformedImage = filter.Apply(current);
            return transformedImage;
        }

        // TODO: Not my code
        private Bitmap ResizeImage(Bitmap image, int width)
        {
            int height = (int)((float)width / image.Width * image.Height);

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
