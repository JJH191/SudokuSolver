using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using ViewModels.Converters;

namespace SudokuSolver
{
    /// <summary>
    /// Interaction logic for SudokuGrid.xaml
    /// </summary>
    public partial class SudokuGrid : UserControl, INotifyPropertyChanged
    {
        private SudokuGridViewModel sudokuGrid;
        public SudokuGridViewModel DataGrid 
        { 
            get => sudokuGrid;
            set
            {
                sudokuGrid = value;
                DataContext = sudokuGrid;

                for (int j = 0; j < 9; j++)
                {
                    for (int i = 0; i < 9; i++)
                    {
                        GrdSudokuGrid.Children.Add(GetCell(i, j));
                    }
                }
                //Notify(nameof(DataGrid));
            }
        }

        public static readonly DependencyProperty DataGridProperty =
            DependencyProperty.Register("DataGrid", typeof(SudokuGridViewModel), typeof(SudokuGrid),
            new PropertyMetadata(new SudokuGridViewModel(new int[9,9]), DataGridPropertyChanged));

        private void DataGridPropertyChanged(SudokuGridViewModel sudokuGrid)
        {
            DataGrid = sudokuGrid;
        }

        private static void DataGridPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SudokuGrid)d).DataGridPropertyChanged((SudokuGridViewModel)e.NewValue);
        }

        public SudokuGrid()
        {
            InitializeComponent();
        }

        #region Generating Grid
        UIElement GetCell(int i, int j)
        {
            Grid grid = new Grid();
            grid.SetBinding(BackgroundProperty, new Binding("Colour") { Source = sudokuGrid[i, j], Converter = new ColourToSolidColourBrush() });

            SetGridPosition(grid, i, j); // Set grid position

            TextBox textBox = new TextBox();
            textBox.SetBinding(TextBox.TextProperty, new Binding("Number") { Source = sudokuGrid[i, j], Converter = new IntToCellString(), UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
            grid.Children.Add(textBox);

            // Make transparent
            textBox.Background = new SolidColorBrush(Colors.Transparent);
            VerticalAlignment = VerticalAlignment.Stretch;
            textBox.BorderThickness = new Thickness(0);

            // Align text to center
            textBox.HorizontalAlignment = HorizontalAlignment.Stretch;
            textBox.VerticalAlignment = VerticalAlignment.Center;
            textBox.TextAlignment = TextAlignment.Center;

            // Text size
            textBox.FontSize = 20;

            return grid;
        }

        private void SetGridPosition(UIElement elem, int i, int j)
        {
            Grid.SetColumn(elem, i);
            Grid.SetRow(elem, j);
        }
        #endregion

        // Notifier for when a property changes
        public event PropertyChangedEventHandler PropertyChanged;
        private void Notify(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
