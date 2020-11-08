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
        private readonly SudokuImageViewModel viewModel;

        private Ellipse[] corners = new Ellipse[4];
        private int selected = -1;

        public ImageAdjustmentsPage(string sudokuPath)
        {
            InitializeComponent();
            viewModel = ((SudokuImageViewModel)DataContext);

            viewModel.BitmapImage = new BitmapImage(new Uri(sudokuPath));
            viewModel.Threshold = 0.5;

            for (int i = 0; i < corners.Length; i++)
            {
                corners[i] = new Ellipse { Width=15, Height=15, Fill=new SolidColorBrush(Colors.Green) };
                canvas.Children.Add(corners[i]);
                Canvas.SetTop(corners[i], i % 2 == 0 ? 100 : canvas.Height - 100);
                Canvas.SetLeft(corners[i], i / 2 == 0 ? 100 : canvas.Width - 100);
            }
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            viewModel.Grayscale();
        }
        
        private void canvas_MouseMoved(object sender, MouseEventArgs e)
        {
            if (selected == -1) return;
            Point mousePos = e.GetPosition(canvas);

            Canvas.SetLeft(corners[selected], Math.Max(Math.Min(mousePos.X, canvas.Width), 0) - corners[selected].Width / 2);
            Canvas.SetTop(corners[selected], Math.Max(Math.Min(mousePos.Y, canvas.Height), 0) - corners[selected].Height / 2);
            

            Debug.WriteLine(mousePos);
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
