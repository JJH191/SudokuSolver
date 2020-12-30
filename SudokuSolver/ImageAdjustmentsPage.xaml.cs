using Accord;
using Accord.Imaging;
using DigitClassifier;
using HelperClasses;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ViewModels;

namespace SudokuSolver
{
    /// <summary>
    /// Interaction logic for ImageAdjustmentsPage.xaml
    /// </summary>
    public partial class ImageAdjustmentsPage : Page
    {
        private readonly SudokuImageViewModel viewModel;
        private readonly QuadViewModel quadViewModel;

        private readonly double circleRadius = 7.5f;
        private int selected = -1;

        public ImageAdjustmentsPage(string sudokuPath)
        {
            InitializeComponent();
            viewModel = ((SudokuImageViewModel)DataContext);

            viewModel.BitmapImage = new BitmapImage(new Uri(sudokuPath));
            viewModel.Threshold = 0.6;

            quadViewModel = new QuadViewModel(DetectCorners(new Bitmap(viewModel.TmpBitmap, new System.Drawing.Size((int)image.Width, (int)image.Height))));

            // Adds corners clockwise
            Ellipse[] corners = new Ellipse[4];
            for (int j = 0; j < 2; j++)
            {
                for (int i = 0; i < 2; i++)
                {
                    int index = j * 2 + i;
                    corners[index] = new Ellipse { Width = circleRadius * 2, Height = circleRadius * 2, Fill = new SolidColorBrush(new Colour(100, 255, 100, 200)) };
                    canvas.Children.Add(corners[index]);
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

        // TODO: not my code, Move to another file, Expand corners out a little (around 2px) as it only detects inside of line
        private Vector2D[] DetectCorners(Bitmap image)
        {
            // locating objects
            BlobCounter blobCounter = new BlobCounter
            {
                FilterBlobs = true,
                MinHeight = 30,
                MinWidth = 30,
                MaxHeight = 700,
                MaxWidth = 700
            };

            blobCounter.ProcessImage(image);
            Blob[] blobs = blobCounter.GetObjectsInformation();

            // check for rectangles
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

            // BUG: TODO: goes outside image bounds
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
        /// Create the quad that is rendered on the canvas
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

                canvas.Children.Add(line);
            }
        }

        /// <summary>
        /// Update the greyscale based on the new threshold
        /// </summary>
        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            viewModel.Grayscale();
        }

        private void Btn_EstimateCorners(object sender, RoutedEventArgs e)
        {
            //if (quadViewModel == null) return;
            Vector2D[] corners = DetectCorners(new Bitmap(viewModel.TmpBitmap, new System.Drawing.Size((int)image.Width, (int)image.Height)));
            for (int i = 0; i < corners.Count(); i++)
                quadViewModel[i] = corners[i];
        }

        /// <summary>
        /// Update the selected corner position to the mouse position
        /// </summary>
        private void Canvas_MouseMoved(object sender, MouseEventArgs e)
        {
            // If there isn't a corner currently being dragged, return
            if (selected == -1) return;
            System.Windows.Point mousePos = e.GetPosition(canvas);

            quadViewModel[selected] = new Vector2D(
                Math.Max(Math.Min(mousePos.X, canvas.Width), 0),
                Math.Max(Math.Min(mousePos.Y, canvas.Height), 0)
            );
        }

        /// <summary>
        /// Loops through all the corners to see if the mouse is on them. If it is, it sets the currently selected corner to the one clicked on
        /// </summary>
        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Get the current mouse position
            System.Windows.Point mousePos = e.GetPosition(canvas);

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
        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            // Reset selected so the corner will not continue to stick to the mouse
            selected = -1;
        }

        /// <summary>
        /// Go to the next page and pass the adjusted image
        /// </summary>
        private void Btn_Next(object sender, RoutedEventArgs e)
        {
            Bitmap adjustedImage = viewModel.GetAdjustedImage(quadViewModel, canvas.ActualWidth);
            adjustedImage = Invert(RemoveBorders(adjustedImage));
            adjustedImage.Save("CleanedImage.png");
            float cellSize = adjustedImage.Width / 9f;

            int[,] sudoku = new int[9, 9];
            NeuralNetworkDigitClassifier classifier = new NeuralNetworkDigitClassifier("neural_network.nn");

            for (int j = 0; j < 9; j++)
            {
                for (int i = 0; i < 9; i++)
                {
                    float x = cellSize * i;
                    float y = cellSize * j;

                    Bitmap cell = adjustedImage.Clone(new System.Drawing.Rectangle((int)x, (int)y, (int)cellSize, (int)cellSize), adjustedImage.PixelFormat);

                    //cell.Save($"{i},{j}.png");

                    float emptyThreshold = 0.02f;
                    if (cell.GetAverageBrightness() < emptyThreshold) sudoku[i, j] = -1;
                    else
                    {
                        int classifiedDigit = classifier.GetDigit(cell);

                        if (classifiedDigit == 0) classifiedDigit = 8; // TODO: work out what number is best to replace 0 with
                        sudoku[i, j] = classifiedDigit;
                    }
                }
            }

            // TODO: Navigate to next page with sudoku grid
            NavigationService.Navigate(new GridPage(sudoku));
        }

        // NOT MY CODE
        private Bitmap Invert(Bitmap image)
        {
            Bitmap result = new Bitmap(image);
            for (int y = 0; y <= (result.Height - 1); y++)
            {
                for (int x = 0; x <= (result.Width - 1); x++)
                {
                    Colour inv = result.GetPixel(x, y);
                    inv = new Colour(255, (255 - inv.R), (255 - inv.G), (255 - inv.B));
                    result.SetPixel(x, y, inv);
                }
            }

            return result;
        }

        // Optimise?
        private Bitmap RemoveBorders(Bitmap source)
        {
            Bitmap result = new Bitmap(source);
            for (int i = 0; i < result.Width; i += 5)
            {
                if (source.GetPixel(i, 0).GetBrightness() < 0.95f) result = result.FloodFill(new Vector2D(i, 0), tolerance: 1f); // Top
                if (source.GetPixel(i, result.Height - 1).GetBrightness() < 0.95f) result = result.FloodFill(new Vector2D(i, result.Height - 1), tolerance: 1f); // Bottom
            }

            for (int i = 0; i < result.Height; i += 5)
            {
                if (source.GetPixel(0, i).GetBrightness() < 0.95f) result = result.FloodFill(new Vector2D(0, i), tolerance: 1f); // Left
                if (source.GetPixel(result.Width - 1, i).GetBrightness() < 0.95f) result = result.FloodFill(new Vector2D(result.Width - 1, i), tolerance: 1f); // Right
            }

            return result;
        }
    }
}
