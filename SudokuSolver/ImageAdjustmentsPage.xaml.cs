using Accord;
using Accord.Imaging;
using DigitClassifier;
using Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using ViewModels;
using ViewModels.Converters;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SudokuSolver
{
    /// <summary>
    /// Interaction logic for ImageAdjustmentsPage.xaml
    /// </summary>
    public partial class ImageAdjustmentsPage : Page
    {
        private readonly SudokuImageViewModel viewModel;
        private readonly QuadViewModel quadViewModel;

        private readonly string sudokuPath;
        private readonly double circleRadius = 7.5f;
        private int selected = -1;

        public ImageAdjustmentsPage(string sudokuPath)
        {
            InitializeComponent();
            viewModel = (SudokuImageViewModel)DataContext;

            this.sudokuPath = sudokuPath;
            viewModel.Image = new Bitmap(sudokuPath);
            viewModel.Threshold = 0.6;

            Vector2D[] detectedCorners = DetectCorners(viewModel.Image);
            quadViewModel = new QuadViewModel(detectedCorners);

            // Adds corners clockwise
            Ellipse[] corners = new Ellipse[4];
            for (int j = 0; j < 2; j++)
            {
                for (int i = 0; i < 2; i++)
                {
                    int index = j * 2 + i;
                    corners[index] = new Ellipse { Width = circleRadius * 2, Height = circleRadius * 2, Fill = new SolidColorBrush(new Colour(100, 255, 100, 200)) };
                    CvsImage.Children.Add(corners[index]);
                }
            }

            // Add bindings for the ellipses drawn at each corner
            IValueConverter converter = new CirclePositionCentreConverter(circleRadius);
            for (int i = 0; i < corners.Length; i++)
            {
                Binding xBinding = new Binding($"[{i}].X") { Source = quadViewModel, Mode = BindingMode.OneWay, Converter = converter };
                Binding yBinding = new Binding($"[{i}].Y") { Source = quadViewModel, Mode = BindingMode.OneWay, Converter = converter };
                corners[i].SetBinding(Canvas.LeftProperty, xBinding);
                corners[i].SetBinding(Canvas.TopProperty, yBinding);
            }

            CreateQuad();
        }

        // TODO (CLEANING): Move to another file?
        // Code from https://stackoverflow.com/questions/5945156/c-sharp-detect-rectangles-in-image
        // Modified to work as a function rather than just being in the Main() function
        // I also added a function to check multiple subtypes (adding in some extra ones so it worked better for me)
        // Then I added in some code to keep track of all the most extreme corners (tl, tr, bl, br) and expand these by 3px to make the corners lie on the line rather than next to it
        private Vector2D[] DetectCorners(Bitmap image)
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
            Accord.Math.Geometry.SimpleShapeChecker shapeChecker = new Accord.Math.Geometry.SimpleShapeChecker();

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

        private bool IsAnySubtype(Accord.Math.Geometry.SimpleShapeChecker shapeChecker, List<IntPoint> corners, params Accord.Math.Geometry.PolygonSubType[] subTypes)
        {
            Accord.Math.Geometry.PolygonSubType subType = shapeChecker.CheckPolygonSubType(corners);
            return subTypes.Contains(subType);
        }

        /// <summary>
        /// Create the quad that is rendered on the CvsImage
        /// </summary>
        private void CreateQuad()
        {
            for (int i = 0; i < quadViewModel.Length; i++)
            {
                Line line = new Line { Stroke = new SolidColorBrush(Colors.Fuchsia) };

                // Set all the bindings for each coordinate to the quad's corner positiopns
                line.SetBinding(Line.X1Property, new Binding($"[{i}].X") { Source = quadViewModel, Mode = BindingMode.OneWay });
                line.SetBinding(Line.Y1Property, new Binding($"[{i}].Y") { Source = quadViewModel, Mode = BindingMode.OneWay });
                line.SetBinding(Line.X2Property, new Binding($"[{(i + 1) % quadViewModel.Length}].X") { Source = quadViewModel, Mode = BindingMode.OneWay });
                line.SetBinding(Line.Y2Property, new Binding($"[{(i + 1) % quadViewModel.Length}].Y") { Source = quadViewModel, Mode = BindingMode.OneWay });

                CvsImage.Children.Add(line);
            }
        }

        /// <summary>
        /// Update the greyscale based on the new threshold
        /// </summary>
        private void SldThreshold_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            viewModel.Greyscale();
        }

        private void BtnEstimateCorners_Click(object sender, RoutedEventArgs e)
        {
            Vector2D[] corners = DetectCorners(viewModel.Image);// new Bitmap(viewModel.Image, new System.Drawing.Size((int)image.Width, (int)image.Height)));
            for (int i = 0; i < corners.Count(); i++)
                quadViewModel[i] = corners[i];
        }

        /// <summary>
        /// Update the selected corner position to the mouse position
        /// </summary>
        private void CvsImage_MouseMoved(object sender, MouseEventArgs e)
        {
            // If there isn't a corner currently being dragged, return
            if (selected == -1) return;
            System.Windows.Point mousePos = e.GetPosition(CvsImage);

            quadViewModel[selected] = new Vector2D(
                Math.Max(Math.Min(mousePos.X, CvsImage.Width), 0),
                Math.Max(Math.Min(mousePos.Y, CvsImage.Height), 0)
            );
        }

        /// <summary>
        /// Loops through all the corners to see if the mouse is on them. If it is, it sets the currently selected corner to the one clicked on
        /// </summary>
        private void CvsImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Get the current mouse position
            System.Windows.Point mousePos = e.GetPosition(CvsImage);

            // Loop through all the corners
            for (int i = 0; i < quadViewModel.Length; i++)
            {
                double distSquaredFromPoint = (new Vector2D(mousePos) - quadViewModel[i]).LengthSquared();

                // Check if the mouse position is inside the circle (using length squared as it is more efficient than length)
                if (distSquaredFromPoint <= circleRadius * circleRadius)
                {
                    // If the mouse is inside the circle, set the currently selected corner to the index of the one clicked on
                    selected = i;
                    return;
                }
            }
        }

        /// <summary>
        /// Reset selected corner
        /// </summary>
        private void CvsImage_MouseUp(object sender, MouseButtonEventArgs e)
        {
            // Reset selected so the corner will not continue to stick to the mouse
            selected = -1;
        }

        /// <summary>
        /// Go to the next page and pass the adjusted image
        /// </summary>
        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            Bitmap adjustedImage = viewModel.GetAdjustedImage(quadViewModel);

            if (adjustedImage == null) // Could not get the adjusted image as the corners were invalid
            {
                MessageBox.Show("Corners should form a square like shape. Make sure that the corners are with the image and try again", "Error - Invalid Corners");
                return;
            }

            adjustedImage = adjustedImage.Invert(); // Remove the borders of the sudoku and invert the colours so the image works with the neural network
            float cellSize = adjustedImage.Width / 9f; // Get the size of an individual cell

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
                    Bitmap cell = adjustedImage.Clone(new System.Drawing.Rectangle((int)x, (int)y, (int)cellSize, (int)cellSize), adjustedImage.PixelFormat);
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

            // Navigate to next page with the sudoku grid
            NavigationService.Navigate(new GridPage(sudoku, sudokuPath));
        }

        /// <summary>
        /// Go back to the welcome page
        /// </summary>
        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private Bitmap CropCellToDigit(Bitmap cell, int border = 10)
        {
            Vector2I startPoint = FindPointInDigit(cell, new Vector2I(cell.Width / 2, cell.Height / 2));
            if (startPoint == null) return null;
            System.Drawing.Rectangle rect = new System.Drawing.Rectangle(startPoint.X, startPoint.Y, 1, 1);

            bool didExpandHorizontal = true;
            bool didExpandVertical = true;
            while (didExpandHorizontal || didExpandVertical)
            {
                didExpandHorizontal = false;
                didExpandVertical = false;
                for (int i = rect.Y; i < rect.Y + rect.Height; i++){
                    if (rect.X > 0 && cell.GetPixel(rect.X - 1, i).GetBrightness() > 0.6f)
                    {
                        rect.X -= 1;
                        didExpandHorizontal = true; 
                    }

                    if (rect.X + rect.Width < cell.Width - 1 && cell.GetPixel(rect.X + rect.Width, i).GetBrightness() > 0.6f) 
                    {
                        rect.Width += 1;
                        didExpandHorizontal = true;
                    }

                    if (didExpandHorizontal) break;
                }

                for (int i = rect.X; i < rect.X + rect.Width; i++){
                    if (rect.Y > 0 && cell.GetPixel(i, rect.Y - 1).GetBrightness() > 0.6f) 
                    {
                        rect.Y -= 1;
                        didExpandVertical = true;
                    }

                    if (rect.Y + rect.Height < cell.Height - 1 && cell.GetPixel(i, rect.Y + rect.Height).GetBrightness() > 0.6f)
                    {
                        rect.Height += 1;
                        didExpandVertical = true;
                    }

                    if (didExpandVertical) break;
                }
            }

            // If the rectangle surrounding the digit is within 4px of the cell width, it is likely that the border was included in the rectangle
            // This happens when the digit touches the border, so to fix it, I shrink the rectangle by 4px and then recrop the cell (to centre it)
            bool isBorderIncluded = Math.Abs(cell.Width - rect.Width) + Math.Abs(cell.Height - rect.Height) < 4;
            if (isBorderIncluded) rect.Inflate(-4, -4);

            // Get the largest dimension of the rectangle
            // This is so that the source rectangle is square to prevent any stretching
            int maxOfWidthAndHeight = Math.Max(rect.Width, rect.Height); // Note: I add 1 so that the maxX and maxY pixels are included, otherwise there is a cutoff

            Bitmap centred = new Bitmap(cell.Width, cell.Height);
            using (Graphics g = Graphics.FromImage(centred))
            {
                // Fill the new image with black
                g.FillRectangle(new SolidBrush(System.Drawing.Color.Black), 0, 0, cell.Width, cell.Height);

                System.Drawing.Rectangle destRect = new System.Drawing.Rectangle(border, border, centred.Width - border * 2, centred.Height - border * 2); // Destination rectangle that is a square with a border of the specified width

                int srcX = rect.Center().X - maxOfWidthAndHeight / 2;
                int srcY = rect.Center().Y - maxOfWidthAndHeight / 2;
                System.Drawing.Rectangle srcRect = new System.Drawing.Rectangle(srcX, srcY, maxOfWidthAndHeight, maxOfWidthAndHeight); // Rectangle on the source image that is the smallest square around the digit
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
                    toSearch.Push(current + new Vector2I(-1,  0));
                    toSearch.Push(current + new Vector2I( 1,  0));
                    toSearch.Push(current + new Vector2I( 0, -1));
                    toSearch.Push(current + new Vector2I( 0,  1));
                }
            }

            return null;
        }
    }
}
