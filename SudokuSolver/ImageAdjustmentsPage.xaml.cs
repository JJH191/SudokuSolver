using DigitClassifier;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
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
        private readonly QuadViewModel quadViewModel = new QuadViewModel();

        private readonly double circleRadius = 7.5f;
        private int selected = -1;

        public ImageAdjustmentsPage(string sudokuPath)
        {
            InitializeComponent();
            viewModel = ((SudokuImageViewModel)DataContext);

            viewModel.BitmapImage = new BitmapImage(new Uri(sudokuPath));
            viewModel.Threshold = 0.5;

            // Adds corners clockwise
            Ellipse[] corners = new Ellipse[4];
            for (int j = 0; j < 2; j++)
            {
                for (int i = 0; i < 2; i++)
                {
                    int index = j * 2 + i;
                    corners[index] = new Ellipse { Width = circleRadius * 2, Height = circleRadius * 2, Fill = new SolidColorBrush(System.Windows.Media.Color.FromArgb(100, 255, 100, 200)) };
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

        /// <summary>
        /// Create the quad that is rendered on the canvas
        /// </summary>
        private void CreateQuad()
        {
            for (int i = 0; i < quadViewModel.Length; i++)
            {
                Line line = new Line
                {
                    Stroke = new SolidColorBrush(Colors.Fuchsia)
                };

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
        
        /// <summary>
        /// Update the selected corner position to the mouse position
        /// </summary>
        private void Canvas_MouseMoved(object sender, MouseEventArgs e)
        {
            // If there isn't a corner currently being dragged, return
            if (selected == -1) return;
            System.Windows.Point mousePos = e.GetPosition(canvas);

            quadViewModel[selected] = new PointPos(
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
                double distSquaredFromPoint = (new PointPos(mousePos) - quadViewModel[i]).LengthSquared();

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
            float cellSize = adjustedImage.Width / 9f;

            int[,] sudoku = new int[9,9];
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
                    //Debug.WriteLine(GetAverageColor(cell).R);
                    if (GetAverageColor(cell).R < emptyThreshold * 255) sudoku[i, j] = -1;
                    else sudoku[i, j] = classifier.GetDigit(cell);
                }
            }

            // TODO: Navigate to next page with sudoku grid
        }
        
        // NOT MY CODE
        private Bitmap Invert(Bitmap image)
        {
            Bitmap result = new Bitmap(image);
            for (int y = 0; (y <= (result.Height - 1)); y++)
            {
                for (int x = 0; (x <= (result.Width - 1)); x++)
                {
                    System.Drawing.Color inv = result.GetPixel(x, y);
                    inv = System.Drawing.Color.FromArgb(255, (255 - inv.R), (255 - inv.G), (255 - inv.B));
                    result.SetPixel(x, y, inv);
                }
            }

            return result;
        }

        private Bitmap RemoveBorders(Bitmap source)
        {
            Bitmap result = new Bitmap(source);
            for (int i = 0; i < result.Width; i++) result = FloodFill(result, i, 0); // Top
            for (int i = 0; i < result.Width; i++) result = FloodFill(result, i, result.Height - 1); // Bottom
            for (int i = 0; i < result.Height; i++) result = FloodFill(result, 0, i); // Left
            for (int i = 0; i < result.Height; i++) result = FloodFill(result, result.Width - 1, i); // Right

            return result;
        }

        // NOT MY CODE!
        private Bitmap FloodFill(Bitmap image, int x, int y)
        {
            Stack<PointPos> pixels = new Stack<PointPos>();
            pixels.Push(new PointPos(x, y));

            while (pixels.Count > 0)
            {
                PointPos a = pixels.Pop();
                if (a.X < image.Width && a.X > -1 && a.Y < image.Height && a.Y > -1)
                {
                    System.Drawing.Color pixelCol = image.GetPixel((int)a.X, (int)a.Y);
                    if (pixelCol.R + pixelCol.G + pixelCol.B != 255 * 3)
                    {
                        image.SetPixel((int)a.X, (int)a.Y, System.Drawing.Color.White);
                        pixels.Push(new PointPos(a.X - 1, a.Y));
                        pixels.Push(new PointPos(a.X + 1, a.Y));
                        pixels.Push(new PointPos(a.X, a.Y - 1));
                        pixels.Push(new PointPos(a.X, a.Y + 1));
                    }
                }
            }
            return image;
        }

        // NOT MY CODE
        public static System.Drawing.Color GetAverageColor(Bitmap bmp)
        {
            //Used for tally
            int r = 0;
            int g = 0;
            int b = 0;

            int total = 0;

            for (int x = 0; x < bmp.Width; x++)
            {
                for (int y = 0; y < bmp.Height; y++)
                {
                    System.Drawing.Color clr = bmp.GetPixel(x, y);

                    r += clr.R;
                    g += clr.G;
                    b += clr.B;

                    total++;
                }
            }

            //Calculate average
            r /= total;
            g /= total;
            b /= total;

            return System.Drawing.Color.FromArgb(r, g, b);
        }
    }
}
