using Accord;
using Accord.Imaging;
using Accord.Math.Geometry;
using Common;
using DigitClassifier;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

namespace SudokuSolver
{
    /// <summary>
    /// A class to help with the classifying of all the digits in a sudoku grid
    /// </summary>
    public class GridClassificationHelper
    {
        /// <summary>
        /// Classifies all the digits in the provided <paramref name="grid"/> bitmap
        /// </summary>
        /// <param name="grid">The bitmap of the grid</param>
        /// <returns></returns>
        public int[,] ClassifyGrid(Bitmap grid)
        {
            Stopwatch s = new Stopwatch();
            s.Start();
            grid = grid.Invert(); // Invert the colours so the image works with the neural network
            float cellSize = grid.Width / 9f; // Get the size of an individual cell

            int[,] sudoku = new int[9, 9];
            NeuralNetworkDigitClassifier classifier = new NeuralNetworkDigitClassifier("res/trained_network.nn"); // Create the classifier from the saved model

            for (int j = 0; j < 9; j++)
            {
                for (int i = 0; i < 9; i++)
                {
                    // Work out the pixel coords of the cell
                    float x = cellSize * i;
                    float y = cellSize * j;

                    // Get an individual cell image
                    Bitmap cell = grid.Clone(new Rectangle((int)x, (int)y, (int)cellSize, (int)cellSize), grid.PixelFormat);
                    float emptyThreshold = 0.01f; // Threshold to class a cell as empty
                    if (cell.GetAverageBrightness() < emptyThreshold) sudoku[i, j] = -1; // If the cell is empty, set its value to -1
                    else
                    {
                        cell = CropCellToDigit(cell); // CentreDigit(cell); // Centre the digit in the cell so it is more similar to the training data
                        // cell.Save($"debug/{i},{j}.jpg");
                        if (cell == null)
                        {
                            sudoku[i, j] = -1;
                            continue;
                        }

                        // Classify the digit
                        int classifiedDigit = classifier.GetDigit(cell);

                        // If the digit is classified as 0 (which is not valid in sudoku), change it to 8 as this is the most likely 
                        if (classifiedDigit == 0) classifiedDigit = 8;
                        sudoku[i, j] = classifiedDigit;
                    }
                }
            }
            s.Stop();
            Trace.WriteLine($"Classification took {s.ElapsedMilliseconds}ms");
            return sudoku;
        }

        /// <summary>
        /// Locates the digit in a given <paramref name="cell"/> and crops it so only the image is in the final image
        /// </summary>
        /// <param name="cell">The image of the cell with a digit</param>
        /// <param name="border">The minimum width of the gap between the edges and digit</param>
        /// <returns>An image cropped to only the digit in the provided cell image</returns>
        private Bitmap CropCellToDigit(Bitmap cell, int border = 10)
        {
            // Finds a point inside the digit in the cell
            Vector2I startPoint = FindPointInDigit(cell);
            if (startPoint == null) return null; // If the start point was null, there was not a digit in the cell

            // The bounding box of the digit. Start with a 1x1 rectangle at the start point
            Rectangle digitBoundingBox = new Rectangle(startPoint.X, startPoint.Y, 1, 1);

            // Variables to keep track of whether the box expanded horizontally or vertically
            bool didExpandHorizontal = true;
            bool didExpandVertical = true;

            // Continue expanding the bounding box until we reach the edges of the image
            while (didExpandHorizontal || didExpandVertical)
            {
                // Reset the did expand variables
                didExpandHorizontal = false;
                didExpandVertical = false;

                // Loop through all the pixels on the left and right side of the current bounding box
                for (int i = digitBoundingBox.Y; i < digitBoundingBox.Y + digitBoundingBox.Height; i++)
                {
                    // If the pixel to the left of the bounding box is bright enough, expand the box to the left by 1px
                    if (digitBoundingBox.X > 0 && cell.GetPixel(digitBoundingBox.X - 1, i).GetBrightness() > 0.6f)
                    {
                        digitBoundingBox.X -= 1;
                        digitBoundingBox.Width += 1;
                        didExpandHorizontal = true;
                    }

                    // If the pixel to the right of the bounding box is bright enough, expand the box to the right by 1px
                    if (digitBoundingBox.X + digitBoundingBox.Width < cell.Width - 1 && cell.GetPixel(digitBoundingBox.X + digitBoundingBox.Width, i).GetBrightness() > 0.6f)
                    {
                        digitBoundingBox.Width += 1;
                        didExpandHorizontal = true;
                    }

                    // If we expanded the bounding box, don't bother checking the rest of the edge pixels
                    if (didExpandHorizontal) break;
                }

                // Loop through all the pixels on the top and bottom side of the current bounding box
                for (int i = digitBoundingBox.X; i < digitBoundingBox.X + digitBoundingBox.Width; i++)
                {
                    // If the pixel above the bounding box is bright enough, expand the box up by 1px
                    if (digitBoundingBox.Y > 0 && cell.GetPixel(i, digitBoundingBox.Y - 1).GetBrightness() > 0.6f)
                    {
                        digitBoundingBox.Y -= 1;
                        digitBoundingBox.Height += 1;
                        didExpandVertical = true;
                    }

                    // If the pixel underneath the bounding box is bright enough, expand the box down by 1px
                    if (digitBoundingBox.Y + digitBoundingBox.Height < cell.Height - 1 && cell.GetPixel(i, digitBoundingBox.Y + digitBoundingBox.Height).GetBrightness() > 0.6f)
                    {
                        digitBoundingBox.Height += 1;
                        didExpandVertical = true;
                    }

                    // If we expanded the bounding box, don't bother checking the rest of the edge pixels
                    if (didExpandVertical) break;
                }
            }

            // If the width and height of the digit is less than 10% of the cell width and height then it is most likely a spec rather than a digit
            if ((float)digitBoundingBox.Width / cell.Width < 0.1 && (float)digitBoundingBox.Height / cell.Height < 0.1) // TODO (CHECK): Make sure this works properly
                return null;

            // Centre the digit with the given bounding box and border around it
            return CentreDigit(cell, digitBoundingBox, border);
        }

        /// <summary>
        /// Centres the digit from the <paramref name="cell"/> bitmap in a new bitmap with a gap around the edges specified with <paramref name="border"/>
        /// </summary>
        /// <param name="cell">The cell containing the digit</param>
        /// <param name="digitBoundingBox">The bounding box specifying the coordinates and dimensions of the image</param>
        /// <param name="border">The minimum gap between the digit and the sides of the image</param>
        /// <returns>A new image with the digit centred in it</returns>
        private Bitmap CentreDigit(Bitmap cell, Rectangle digitBoundingBox, int border)
        {
            // If the rectangle surrounding the digit is within 4px of the cell width, it is likely that the border was included in the rectangle
            // This happens when the digit touches the border, so to fix it, I shrink the rectangle by 4px and then recrop the cell (to centre it)
            bool isBorderIncluded = Math.Abs(cell.Width - digitBoundingBox.Width) + Math.Abs(cell.Height - digitBoundingBox.Height) < 4;
            if (isBorderIncluded) digitBoundingBox.Inflate(-4, -4);

            Bitmap centred = new Bitmap(cell.Width, cell.Height);
            using (Graphics g = Graphics.FromImage(centred))
            {
                // Fill the new image with black
                g.FillRectangle(new SolidBrush(Color.Black), 0, 0, cell.Width, cell.Height);

                // Start with a square, then scale the smallest dimension down to have the correct aspect ratio
                Rectangle destRect = new Rectangle(border, border, centred.Width - border * 2, centred.Height - border * 2); // Destination rectangle that is a square with a border of the specified width
                if (digitBoundingBox.Width > digitBoundingBox.Height)
                {
                    destRect.Height = (int)(destRect.Width * ((float)digitBoundingBox.Height / digitBoundingBox.Width));
                    destRect.Y = (centred.Height - destRect.Height) / 2;
                }
                else
                {
                    destRect.Width = (int)(destRect.Height * ((float)digitBoundingBox.Width / digitBoundingBox.Height));
                    destRect.X = (centred.Width - destRect.Width) / 2;
                }

                g.DrawImage(cell, destRect, digitBoundingBox, GraphicsUnit.Pixel); // Copy across the image from the source rect to the dest rect
            }

            // If the border was in the image, the digit might not be centred properly. Now we have cropped the border out, run it through the CropCellToDigit function again to centre and crop it
            if (isBorderIncluded) return CropCellToDigit(centred);
            return centred;
        }

        /// <summary>
        /// Finds a point inside the digit in the given cell
        /// This works by searching left, right, up and down from the centre pixel until it finds one bright enough
        /// </summary>
        /// <param name="cell">The cell to find the digit in</param>
        /// <returns>A Vector2I of the location of a pixel in the digit</returns>
        private Vector2I FindPointInDigit(Bitmap cell)
        {
            // TODO (OPTIMISING): Only search left, right, up, down rather than flood check
            Vector2I start = new Vector2I(cell.Width / 2, cell.Height / 2); // Start at the centre of the image

            Common.Queue<Vector2I> toSearch = new Common.Queue<Vector2I>(28); // Keeps track of the pixels to check
            List<Vector2I> alreadySearched = new List<Vector2I>(); // Keeps track of the pixels that have been searched
            toSearch.Push(start); // Add the starting pixel to the pixels to search

            float percentageWidthForBorder = 1 / 3f; // Ignore pixels within the outer 1/3 of the image
            int borderToIgnore = (int)(cell.Width * percentageWidthForBorder); // Calculate the number of pixels to ignore from the percentage

            // While there are still pixels to search
            while (toSearch.Count > 0)
            {
                Vector2I current = toSearch.Pop(); // Get the next pixel to search
                if (alreadySearched.Contains(current)) continue; // Skip if we have already searched this pixel

                // TODO (OPTIMISING): Only check if outside border, do not worry about 0, width or height
                // Skip if the pixel is in the region to ignore or outside the borders of the image
                if (current.X < 0 || current.Y < 0 || current.X >= cell.Width || current.Y >= cell.Height) return null;
                if (current.X < borderToIgnore || current.Y < borderToIgnore || current.X >= cell.Width - borderToIgnore || current.Y >= cell.Height - borderToIgnore)
                    return null;

                // Add the pixel to already searched so it isn't searched again
                alreadySearched.Add(current);

                // If the brightness of this pixel is heigh enough, this is a pixel in the digit, so return this location
                if (cell.GetPixel(current.X, current.Y).GetBrightness() > 0.6f) return current;
                else
                {
                    // Otherwise add the pixels to the left, right, up and down of this one to the queue to search
                    toSearch.Push(current + new Vector2I(-1, 0));
                    toSearch.Push(current + new Vector2I(1, 0));
                    toSearch.Push(current + new Vector2I(0, -1));
                    toSearch.Push(current + new Vector2I(0, 1));
                }
            }

            // No pixel was bright enough, so return null
            return null;
        }

        // Code from https://stackoverflow.com/questions/5945156/c-sharp-detect-rectangles-in-image
        // Modified to work as a function rather than just being in the Main() function
        // I also added a function to check multiple subtypes (adding in some extra ones so it worked better for me)
        // Then I added in some code to keep track of all the most extreme corners (tl, tr, bl, br) and expand these by 3px to make the corners lie on the line rather than next to it
        /// <summary>
        /// Detects the corners of the sudoku in the given <paramref name="image"/>
        /// </summary>
        /// <param name="image">The image to find the sudoku corners in</param>
        /// <returns>An array of points of the corners of the sudoku</returns>
        public Vector2D[] DetectCorners(Bitmap image)
        {
            // locating objects
            BlobCounter blobCounter = new BlobCounter
            {
                // Modified configuration so it works for my case
                FilterBlobs = true,
                MinHeight = 30,
                MinWidth = 30,
                MaxHeight = 700,
                MaxWidth = 700
            };

            blobCounter.ProcessImage(image);
            Blob[] blobs = blobCounter.GetObjectsInformation();

            // Check for rectangles
            SimpleShapeChecker shapeChecker = new SimpleShapeChecker();

            IntPoint topLeft = new IntPoint(image.Width, image.Height); // Start with max distance from top left
            IntPoint topRight = new IntPoint(0, image.Height); // Start with max distance from top right
            IntPoint bottomRight = new IntPoint(0, 0); // Start with max distance from bottom right
            IntPoint bottomLeft = new IntPoint(image.Width, 0); // Start with max distance from bottom left

            foreach (var blob in blobs)
            {
                List<IntPoint> edgePoints = blobCounter.GetBlobsEdgePoints(blob);

                if (shapeChecker.IsQuadrilateral(edgePoints, out List<IntPoint> cornerPoints))
                {
                    if (IsAnySubtype(shapeChecker, cornerPoints, PolygonSubType.Parallelogram, PolygonSubType.Rectangle, PolygonSubType.Square))
                    {
                        List<PointF> Points = new List<PointF>();
                        foreach (var point in cornerPoints)
                        {
                            Points.Add(new PointF(point.X, point.Y));

                            // Could optimise by keeping track of distance rather than recalculating
                            if (point.SquaredDistanceTo(new IntPoint(0, 0)) < topLeft.SquaredDistanceTo(new IntPoint(0, 0))) topLeft = point;
                            if (point.SquaredDistanceTo(new IntPoint(image.Width, 0)) < topRight.SquaredDistanceTo(new IntPoint(image.Width, 0))) topRight = point;
                            if (point.SquaredDistanceTo(new IntPoint(image.Width, image.Height)) < bottomRight.SquaredDistanceTo(new IntPoint(image.Width, image.Height))) bottomRight = point;
                            if (point.SquaredDistanceTo(new IntPoint(0, image.Height)) < bottomLeft.SquaredDistanceTo(new IntPoint(0, image.Height))) bottomLeft = point;
                        }
                    }
                }
            }

            // Expand the bounds by a bit as the detection finds the inside of the line
            int expand = 3;
            Vector2D[] corners = new Vector2D[]
            {
                new Vector2D(ClampToBounds(topLeft.X - expand, max: image.Width - 1), ClampToBounds(topLeft.Y - expand, max: image.Height - 1)),
                new Vector2D(ClampToBounds(topRight.X + expand, max: image.Width - 1), ClampToBounds(topRight.Y - expand, max: image.Height - 1)),
                new Vector2D(ClampToBounds(bottomRight.X + expand, max: image.Width - 1), ClampToBounds(bottomRight.Y + expand, max: image.Height - 1)),
                new Vector2D(ClampToBounds(bottomLeft.X - expand, max: image.Width - 1), ClampToBounds(bottomLeft.Y + expand, max: image.Height - 1)),
            };

            return corners;
        }

        /// <summary>
        /// Clamps the provided <paramref name="value"/> between the <paramref name="min"/> and <paramref name="max"/> values so it cannot go outside of these
        /// </summary>
        /// <param name="value">The value to clamp</param>
        /// <param name="max">The maximum value it can take</param>
        /// <param name="min">The minimum value it can take</param>
        /// <returns>The value but restricted to the values between <paramref name="min"/> and <paramref name="max"/></returns>
        private double ClampToBounds(double value, double max, double min = 0)
        {
            return Math.Max(Math.Min(value, max), min);
        }

        /// <summary>
        /// Checks if the shape provided by a list of <paramref name="corners"/> is any one of the provided shape <paramref name="subTypes"/>
        /// </summary>
        /// <param name="shapeChecker">The shape checker to use to identify the shape</param>
        /// <param name="corners">The corners of the shape to check</param>
        /// <param name="subTypes">The valid subtypes of the shape</param>
        /// <returns>True if the shape is any one of the provided subtypes, false otherwise</returns>
        private bool IsAnySubtype(SimpleShapeChecker shapeChecker, List<IntPoint> corners, params PolygonSubType[] subTypes)
        {
            PolygonSubType subType = shapeChecker.CheckPolygonSubType(corners); // Get the subtype of this polygon
            return subTypes.Contains(subType); // Check if it is one of the provided subtypes
        }
    }
}
