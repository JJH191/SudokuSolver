using Accord;
using Accord.Imaging;
using Accord.Math.Geometry;
using Common;
using DigitClassifier;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace SudokuSolver
{
    public class GridClassificationHelper
    {
        public int[,] ClassifyGrid(Bitmap grid)
        {
            grid = grid.Invert(); // Invert the colours so the image works with the neural network
            float cellSize = grid.Width / 9f; // Get the size of an individual cell

            int[,] sudoku = new int[9, 9];
            NeuralNetworkDigitClassifier classifier = new NeuralNetworkDigitClassifier("trained_network.nn"); // Create the classifier from the saved model

            for (int j = 0; j < 9; j++)
            {
                for (int i = 0; i < 9; i++)
                {
                    // Work out the pixel coords of the cell
                    float x = cellSize * i;
                    float y = cellSize * j;

                    // Get an individual cell image
                    Bitmap cell = grid.Clone(new System.Drawing.Rectangle((int)x, (int)y, (int)cellSize, (int)cellSize), grid.PixelFormat);
                    float emptyThreshold = 0.05f; // Threshold to class a cell as empty
                    if (cell.GetAverageBrightness() < emptyThreshold) sudoku[i, j] = -1; // If the cell is empty, set its value to -1
                    else
                    {
                        cell = CropCellToDigit(cell); // CentreDigit(cell); // Centre the digit in the cell so it is more similar to the training data
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

            return sudoku;
        }

        private Bitmap CropCellToDigit(Bitmap cell, int border = 10)
        {
            Vector2I startPoint = FindPointInDigit(cell, new Vector2I(cell.Width / 2, cell.Height / 2));
            if (startPoint == null) return null;
            Rectangle digitBoundingBox = new Rectangle(startPoint.X, startPoint.Y, 1, 1);

            bool didExpandHorizontal = true;
            bool didExpandVertical = true;
            while (didExpandHorizontal || didExpandVertical)
            {
                didExpandHorizontal = false;
                didExpandVertical = false;
                for (int i = digitBoundingBox.Y; i < digitBoundingBox.Y + digitBoundingBox.Height; i++)
                {
                    if (digitBoundingBox.X > 0 && cell.GetPixel(digitBoundingBox.X - 1, i).GetBrightness() > 0.6f)
                    {
                        digitBoundingBox.X -= 1;
                        didExpandHorizontal = true;
                    }

                    if (digitBoundingBox.X + digitBoundingBox.Width < cell.Width - 1 && cell.GetPixel(digitBoundingBox.X + digitBoundingBox.Width, i).GetBrightness() > 0.6f)
                    {
                        digitBoundingBox.Width += 1;
                        didExpandHorizontal = true;
                    }

                    if (didExpandHorizontal) break;
                }

                for (int i = digitBoundingBox.X; i < digitBoundingBox.X + digitBoundingBox.Width; i++)
                {
                    if (digitBoundingBox.Y > 0 && cell.GetPixel(i, digitBoundingBox.Y - 1).GetBrightness() > 0.6f)
                    {
                        digitBoundingBox.Y -= 1;
                        didExpandVertical = true;
                    }

                    if (digitBoundingBox.Y + digitBoundingBox.Height < cell.Height - 1 && cell.GetPixel(i, digitBoundingBox.Y + digitBoundingBox.Height).GetBrightness() > 0.6f)
                    {
                        digitBoundingBox.Height += 1;
                        didExpandVertical = true;
                    }

                    if (didExpandVertical) break;
                }
            }

            return CentreDigit(cell, digitBoundingBox, border);
        }

        private Bitmap CentreDigit(Bitmap cell, Rectangle digitBoundingBox, int border)
        {
            // If the rectangle surrounding the digit is within 4px of the cell width, it is likely that the border was included in the rectangle
            // This happens when the digit touches the border, so to fix it, I shrink the rectangle by 4px and then recrop the cell (to centre it)
            bool isBorderIncluded = Math.Abs(cell.Width - digitBoundingBox.Width) + Math.Abs(cell.Height - digitBoundingBox.Height) < 4;
            if (isBorderIncluded) digitBoundingBox.Inflate(-4, -4);

            // Get the largest dimension of the rectangle
            // This is so that the source rectangle is square to prevent any stretching
            int maxOfWidthAndHeight = Math.Max(digitBoundingBox.Width, digitBoundingBox.Height); // Note: I add 1 so that the maxX and maxY pixels are included, otherwise there is a cutoff

            Bitmap centred = new Bitmap(cell.Width, cell.Height);
            using (Graphics g = Graphics.FromImage(centred))
            {
                // Fill the new image with black
                g.FillRectangle(new SolidBrush(System.Drawing.Color.Black), 0, 0, cell.Width, cell.Height);

                System.Drawing.Rectangle destRect = new System.Drawing.Rectangle(border, border, centred.Width - border * 2, centred.Height - border * 2); // Destination rectangle that is a square with a border of the specified width

                int srcX = digitBoundingBox.Center().X - maxOfWidthAndHeight / 2;
                int srcY = digitBoundingBox.Center().Y - maxOfWidthAndHeight / 2;
                Rectangle srcRect = new Rectangle(srcX, srcY, maxOfWidthAndHeight, maxOfWidthAndHeight); // Rectangle on the source image that is the smallest square around the digit
                g.DrawImage(cell, destRect, srcRect, GraphicsUnit.Pixel); // Copy across the image from the source rect to the dest rect
            }

            if (isBorderIncluded) return CropCellToDigit(centred);
            return centred;
        }

        private Vector2I FindPointInDigit(Bitmap cell, Vector2I start)
        {
            Common.Queue<Vector2I> toSearch = new Common.Queue<Vector2I>(28);
            List<Vector2I> alreadySearched = new List<Vector2I>();
            toSearch.Push(start);

            float percentageWidthForBorder = 1 / 3f;
            int borderToIgnore = (int)(cell.Width * percentageWidthForBorder);

            while (toSearch.Count > 0)
            {
                Vector2I current = toSearch.Pop();
                if (alreadySearched.Contains(current)) continue;
                if (current.X < 0 || current.Y < 0 || current.X >= cell.Width || current.Y >= cell.Height) return null;
                if (current.X < borderToIgnore || current.Y < borderToIgnore || current.X >= cell.Width - borderToIgnore || current.Y >= cell.Height - borderToIgnore)
                    return null;
                alreadySearched.Add(current);

                if (cell.GetPixel(current.X, current.Y).GetBrightness() > 0.6f) return current;
                else
                {
                    // Left, right, up, down
                    toSearch.Push(current + new Vector2I(-1, 0));
                    toSearch.Push(current + new Vector2I(1, 0));
                    toSearch.Push(current + new Vector2I(0, -1));
                    toSearch.Push(current + new Vector2I(0, 1));
                }
            }

            return null;
        }

        // Code from https://stackoverflow.com/questions/5945156/c-sharp-detect-rectangles-in-image
        // Modified to work as a function rather than just being in the Main() function
        // I also added a function to check multiple subtypes (adding in some extra ones so it worked better for me)
        // Then I added in some code to keep track of all the most extreme corners (tl, tr, bl, br) and expand these by 3px to make the corners lie on the line rather than next to it
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
                    if (IsAnySubtype(shapeChecker, cornerPoints, Accord.Math.Geometry.PolygonSubType.Parallelogram, Accord.Math.Geometry.PolygonSubType.Rectangle, Accord.Math.Geometry.PolygonSubType.Square))
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

        private double ClampToBounds(double value, double max, double min = 0)
        {
            return Math.Max(Math.Min(value, max), min);
        }

        private bool IsAnySubtype(SimpleShapeChecker shapeChecker, List<IntPoint> corners, params PolygonSubType[] subTypes)
        {
            PolygonSubType subType = shapeChecker.CheckPolygonSubType(corners);
            return subTypes.Contains(subType);
        }
    }
}
