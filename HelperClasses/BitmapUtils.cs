using System;
using System.Drawing;
using System.Windows.Media;

namespace HelperClasses
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
        public static Bitmap FloodFill(this Bitmap image, PointPos startPixelPos, Colour fillCol = null, float brightness = 0, float tolerance = 0.1f)
        {
            if (fillCol == null) fillCol = Colors.White; // Default fill colour is white
            Bitmap result = new Bitmap(image); // Copy the bitmap so it doesn't affect the original

            // Create a new stack with a start length of 20 and push the first pixel to fill
            Stack<PointPos> pixelsToFill = new Stack<PointPos>(20);
            pixelsToFill.Push(startPixelPos);

            // Loop while there are still pixels to fill
            while (pixelsToFill.Count > 0)
            {
                PointPos currentPixel = pixelsToFill.Pop(); // Get the pixel on the top of the stack

                // Make sure the pixel is one within the bounds of the image - if not, skip
                int x = (int)currentPixel.X;
                int y = (int)currentPixel.Y;
                if (x < 0 || y < 0 || x >= result.Width || y >= result.Height) continue;

                Colour pixelCol = result.GetPixel(x, y); // Get the pixel colour
                if (Math.Abs(pixelCol.GetBrightness() - brightness) < tolerance) // If colour is within tolerance
                {
                    // Fill the pixel with the fill colour
                    result.SetPixel(x, y, fillCol);

                    // Add neighbouring pixels to stack
                    for (int i = x - 1; i <= x + 1; i++)
                        for (int j = y - 1; j <= y + 1; j++)
                            pixelsToFill.Push(new PointPos(i, j));
                    // NOTE: Does not matter that I'm adding the current pixel to the stack as it will already be filled to the new colour so the flood fill will not do anything
                }
            }

            return result;
        }
    }
}
