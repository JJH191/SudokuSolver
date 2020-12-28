using System;
using System.Drawing;
using System.Windows.Media;

namespace HelperClasses
{
    // TODO: rename to BitmapUtils?
    public static class BitmapUtils
    {
        // Move to helper classes
        // Finds pythagorean distance between colors (normalised between 0 and 1)
        //public static double ColorDifference(Color col1, Color col2)
        //{
        //    return (Math.Pow(col1.R - col2.R, 2) + Math.Pow(col1.G - col2.G, 2) + Math.Pow(col1.B - col2.B, 2)) / (255 * 255);
        //}

        // NOT MY CODE, move to helper classes
        //public static Color GetAverageColor(Bitmap bmp)
        //{
        //    //Used for tally
        //    int r = 0;
        //    int g = 0;
        //    int b = 0;
        //
        //    int total = 0;
        //
        //    for (int x = 0; x < bmp.Width; x++)
        //    {
        //        for (int y = 0; y < bmp.Height; y++)
        //        {
        //            Color clr = bmp.GetPixel(x, y);
        //
        //            r += clr.R;
        //            g += clr.G;
        //            b += clr.B;
        //
        //            total++;
        //        }
        //    }
        //
        //    //Calculate average
        //    r /= total;
        //    g /= total;
        //    b /= total;
        //
        //    return Color.FromArgb(r, g, b);
        //}

        public static double GetAverageBrightness(this Bitmap bmp)
        {
            double totalBrightness = 0;

            for (int j = 0; j < bmp.Height; j++)
            {
                for (int i = 0; i < bmp.Width; i++)
                {
                    Colour col = bmp.GetPixel(i, j);
                    totalBrightness += col.GetBrightness();
                }
            }

            return totalBrightness / (bmp.Width * bmp.Height);
        }

        public static Bitmap FloodFill(this Bitmap image, PointPos startPixelPos, Colour fillCol = null, float brightness = 0, float tolerance = 0.1f)
        {
            if (fillCol == null) fillCol = Colors.White;
            Bitmap result = new Bitmap(image);

            Stack<PointPos> pixelsToFill = new Stack<PointPos>(20);
            pixelsToFill.Push(startPixelPos);

            while (pixelsToFill.Count > 0)
            {
                PointPos currentPixel = pixelsToFill.Pop();

                int x = (int)currentPixel.X;
                int y = (int)currentPixel.Y;
                if (x < 0 || y < 0 || x >= result.Width || y >= result.Height) continue;

                Colour pixelCol = result.GetPixel(x, y);
                if (Math.Abs(pixelCol.GetBrightness() - brightness) < tolerance) // If colour is not within tolerance
                {
                    result.SetPixel(x, y, fillCol);

                    // Add neighbouring pixels to stack
                    for (int i = x - 1; i <= x + 1; i++)
                        for (int j = y - 1; j <= y + 1; j++)
                            pixelsToFill.Push(new PointPos(i, j)); // Does not matter that I'm adding this pixel to the stack as it will already be filled to the new colour
                }
            }

            return result;
        }
    }
}
