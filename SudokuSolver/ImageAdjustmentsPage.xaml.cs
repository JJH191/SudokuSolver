using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ViewModels;

namespace SudokuSolver
{
    /// <summary>
    /// Interaction logic for ImageAdjustmentsPage.xaml
    /// </summary>
    public partial class ImageAdjustmentsPage : Page
    {
        private readonly ImageAdjustmentViewModel viewModel;

        private readonly Path quad;
        private readonly Ellipse[] corners = new Ellipse[4];
        private int selected = -1;

        public ImageAdjustmentsPage(string sudokuPath)
        {
            InitializeComponent();
            viewModel = ((ImageAdjustmentViewModel)DataContext);

            viewModel.BitmapImage = new BitmapImage(new Uri(sudokuPath));
            viewModel.Threshold = 0.5;


            // Adds corners clockwise
            for (int j = 0; j < 2; j++)
            {
                for (int i = 0; i < 2; i++)
                {
                    int index = j * 2 + i;
                    corners[index] = new Ellipse { Width = 15, Height = 15, Fill = new SolidColorBrush(Color.FromArgb(100, 0, 0, 0)) };
                    corners[index].SetBinding(Canvas.LeftProperty, new Binding() { Source = viewModel.Corners[i].X });
                    canvas.Children.Add(corners[index]);

                    Canvas.SetLeft(corners[index], (j ^ i) == 0 ? 100 : canvas.Width - 100);
                    Canvas.SetTop(corners[index], j == 0 ? 100 : canvas.Height - 100);
                }
            }

            quad = new Path { Fill = new SolidColorBrush(Colors.Wheat), Stroke = new SolidColorBrush(Colors.Fuchsia), Visibility=Visibility.Visible};
            canvas.Children.Add(quad);
            UpdateQuad();
        }

        private void UpdateQuad()
        {
            PathGeometry pathGeometry = new PathGeometry();

            for (int i = 0; i < corners.Length; i++)
            {
                double circleRadius = corners[i].Width / 2;
                double x1 = Canvas.GetLeft(corners[i]) + circleRadius;
                double y1 = Canvas.GetTop(corners[i]) + circleRadius;

                double x2 = Canvas.GetLeft(corners[(i + 1) % corners.Length]) + circleRadius;
                double y2 = Canvas.GetTop(corners[(i + 1) % corners.Length]) + circleRadius;
                pathGeometry.AddGeometry(new LineGeometry(new Point(x1, y1), new Point(x2, y2)));
            }

            quad.Data = pathGeometry;
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            viewModel.Grayscale();
        }
        
        private void canvas_MouseMoved(object sender, MouseEventArgs e)
        {
            if (selected == -1) return;
            Point mousePos = e.GetPosition(canvas);

            //Canvas.SetLeft(corners[selected], Math.Max(Math.Min(mousePos.X, canvas.Width), 0) - corners[selected].Width / 2);
            viewModel.Corners[selected].X = (int)(Math.Max(Math.Min(mousePos.X, canvas.Width), 0) - corners[selected].Width / 2);
            Canvas.SetTop(corners[selected], Math.Max(Math.Min(mousePos.Y, canvas.Height), 0) - corners[selected].Height / 2);

            UpdateQuad();
        }

        private void canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point mousePos = e.GetPosition(canvas);
            for (int i = 0; i < corners.Length; i++)
            {
                Ellipse corner = corners[i];
                double left = Canvas.GetLeft(corner);
                double right = left + corner.Width;
                if (mousePos.X >= left && mousePos.X <= right)
                {
                    double top = Canvas.GetTop(corner);
                    double bottom = top + corner.Height;
                    if (mousePos.Y >= top && mousePos.Y <= bottom)
                    {
                        selected = i;
                        return;
                    }
                }
            }
        }

        private void canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            selected = -1;
        }
    }
}
