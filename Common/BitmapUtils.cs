using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Media;

namespace Common
{
    /// <summary>
    /// A set of utility functions for bitmap
    /// </summary>
    public static class BitmapUtils
    {
        /// <summary>
        /// Finds the pixel brightness average of the given bitmap
        /// </summary>
        /// <param name="bmp">Bitmap to find the brightness of</param>
        /// <returns>The average pixel brightness of the bitmap</returns>
        public static double GetAverageBrightness(this Bitmap bmp)
        {
            // Keeps track of the total brightness
            double totalBrightness = 0;

            // Loop through all the pixels and total their brightnesses
            for (int j = 0; j < bmp.Height; j++)
            {
                for (int i = 0; i < bmp.Width; i++)
                {
                    Colour col = bmp.GetPixel(i, j);
                    totalBrightness += col.GetBrightness();
                }
            }

            // Divide the total brightness by the number of pixels to get the average
            return totalBrightness / (bmp.Width * bmp.Height);
        }

        /// <summary>
        /// Fills in all connected pixels with a specified colour
        /// </summary>
        /// <param name="image">The image to flood fill</param>
        /// <param name="startPixelPos">The pixel to start the flood fill from</param>
        /// <param name="fillCol">The colour to fill the area in with</param>
        /// <param name="brightness">The brightness of the colour to replace (brightness is used as it is faster than a full colour and the image is greyscale anyway)</param>
        /// <param name="tolerance">The amount a pixel to fill can vary from the given brightness</param>
        /// <returns>A new image with the flood filled bitmap</returns>
        public static Bitmap FloodFill(this Bitmap image, Vector2D startPixelPos, Colour fillCol = null, float brightness = 0, float tolerance = 0.1f)
        {
            if (fillCol == null) fillCol = Colors.White; // Default fill colour is white
            Bitmap result = new Bitmap(image); // Copy the bitmap so it doesn't affect the original
            BitmapLocker lockBitmap = new BitmapLocker(result);

            lockBitmap.LockBits();

            // Create a new stack with a start length of 20 and push the first pixel to fill
            Stack<Vector2D> pixelsToFill = new Stack<Vector2D>(20);
            pixelsToFill.Push(startPixelPos);

            // Loop while there are still pixels to fill
            while (pixelsToFill.Count > 0)
            {
                Vector2D currentPixel = pixelsToFill.Pop(); // Get the pixel on the top of the stack

                // Make sure the pixel is one within the bounds of the image - if not, skip
                int x = (int)currentPixel.X;
                int y = (int)currentPixel.Y;
                if (x < 0 || y < 0 || x >= result.Width || y >= result.Height) continue;

                Colour pixelCol = lockBitmap.GetPixel(x, y); // Get the pixel colour
                if (Math.Abs(pixelCol.GetBrightness() - brightness) < tolerance) // If colour is within tolerance
                {
                    // Fill the pixel with the fill colour
                    lockBitmap.SetPixel(x, y, fillCol);

                    // Add neighbouring pixels to stack
                    for (int i = x - 1; i <= x + 1; i++)
                        for (int j = y - 1; j <= y + 1; j++)
                            pixelsToFill.Push(new Vector2D(i, j));
                    // NOTE: Does not matter that I'm adding the current pixel to the stack as it will already be filled to the new colour so the flood fill will not do anything
                }
            }

            lockBitmap.UnlockBits();

            return result;
        }

        // Code from https://stackoverflow.com/questions/1922040/how-to-resize-an-image-c-sharp/24199315
        // Modified so it only takes a width so that the aspect ratio is maintained
        /// <summary>
        /// Resize the image to the specified width and height
        /// </summary>
        /// <param name="image">The image to resize</param>
        /// <param name="width">The width to resize to</param>
        /// <param name="height">The height to resize to</param>
        /// <returns>The resized image</returns>
        public static Bitmap Resize(this Bitmap image, int width)
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

        // Code from https://stackoverflow.com/questions/33024881/invert-image-faster-in-c-sharp
        // I cleaned this up a bit to make it more readable, including renaming variables.
        // For example, the for loops go to less than width/height, whereas the original went to less than or equal to the width/height - 1
        public static Bitmap Invert(this Bitmap image)
        {
            Bitmap result = new Bitmap(image);
            BitmapLocker bitmapLocker = new BitmapLocker(result);

            bitmapLocker.LockBits();
            for (int y = 0; y < result.Height; y++)
            {
                for (int x = 0; x < result.Width; x++)
                {
                    Colour inv = bitmapLocker.GetPixel(x, y);
                    inv = new Colour(255, 255 - inv.R, 255 - inv.G, 255 - inv.B);
                    bitmapLocker.SetPixel(x, y, inv);
                }
            }
            bitmapLocker.UnlockBits();

            return result;
        }
    }
}
