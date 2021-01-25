using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using ViewModels;
using ViewModels.Converters;

namespace SudokuSolver
{
    /// <summary>
    /// Interaction logic for SudokuGrid.xaml
    /// </summary>
    public partial class SudokuGrid : UserControl, INotifyPropertyChanged
    {
        private int size = 200;
        public int Size
        {
            get => size;
            set
            {
                size = value;
                Notify(nameof(TextFontSize));
                Notify(nameof(MarginSize));
                Notify(nameof(Size));
            }
        }

        // Font size is 1/20th of the size of the grid
        public double TextFontSize
        {
            get => size / 20;
        }

        // Margin size is 1/180th of the size of the grid
        public int MarginSize
        {
            get => size / 180;
        }

        private SudokuGridViewModel sudokuGrid;
        public SudokuGridViewModel DataGrid
        {
            get => sudokuGrid;
            set
            {
                sudokuGrid = value;
                Notify(nameof(DataGrid));

                // Do not fill the grid unless all properties have been initialised
                // The initial grid filling will be called after InitializeComponent()
                // Any further changes of DataGrid will be handled here
                if (hasInitialised) FillGrid();
            }
        }

        private bool enabled = true;
        public bool Enabled
        {
            get => enabled;
            set
            {
                enabled = value;
                Notify(nameof(Enabled));
            }
        }

        private bool hasInitialised = false;
        private void FillGrid()
        {
            GrdSudokuGrid.Children.Clear();
            for (int j = 0; j < 9; j++)
            {
                for (int i = 0; i < 9; i++)
                {
                    GrdSudokuGrid.Children.Add(GetCell(i, j));
                }
            }
        }

        public static readonly DependencyProperty DataGridProperty =
            DependencyProperty.Register(nameof(DataGrid), typeof(SudokuGridViewModel), typeof(SudokuGrid),
            new PropertyMetadata(new SudokuGridViewModel(new int[9, 9]), DataGridPropertyChanged));

        private void DataGridPropertyChanged(SudokuGridViewModel sudokuGrid)
        {
            DataGrid = sudokuGrid;
        }

        private static void DataGridPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SudokuGrid)d).DataGridPropertyChanged((SudokuGridViewModel)e.NewValue);
        }

        public static readonly DependencyProperty SizeProperty =
            DependencyProperty.Register(nameof(Size), typeof(int), typeof(SudokuGrid),
            new PropertyMetadata(1, SizePropertyChanged));

        private void SizePropertyChanged(int size)
        {
            Size = size;
        }

        private static void SizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SudokuGrid)d).SizePropertyChanged((int)e.NewValue);
        }

        public static readonly DependencyProperty EnabledProperty =
            DependencyProperty.Register(nameof(Enabled), typeof(bool), typeof(SudokuGrid),
            new PropertyMetadata(true, EnabledPropertyChanged));

        private void EnabledPropertyChanged(bool enabled)
        {
            Enabled = enabled;
        }

        private static void EnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SudokuGrid)d).EnabledPropertyChanged((bool)e.NewValue);
        }

        // OnInitialized is used instead of constructor as it allows me to call functions after all initialisation has been completed
        // This means I can access the Enabled property after it has been set to choose textbox or textblock
        protected override void OnInitialized(EventArgs e)
        {
            InitializeComponent();
            base.OnInitialized(e);

            FillGrid();
            hasInitialised = true;
        }

        #region Generating Grid
        UIElement GetCell(int i, int j)
        {
            Grid grid = new Grid(); // Container for the textboxes so the background colour can fill the whole cell
            if (DataGrid == null || DataGrid[i, j] == null) return grid; // Check for null - return empty cell if null

            grid.SetBinding(BackgroundProperty, new Binding("Colour") { Source = DataGrid[i, j], Converter = new ColourToSolidColourBrush() });

            // If Enabled, use a textbox, otherwise a textblock
            // This means that the user can only edit the grid if Enabled is true
            // I am using a textblock for when Enabled is false as it loads a lot faster than textbox, which is essential for the review section
            FrameworkElement text = Enabled ? (FrameworkElement)new TextBox() : new TextBlock();
            text.SetBinding(Enabled ? TextBox.TextProperty : TextBlock.TextProperty, new Binding("Number") { Source = DataGrid[i, j], Converter = new IntToCellString(), UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });

            if (text is TextBox textBox)
            {
                // Make transparent
                textBox.Background = new SolidColorBrush(Colors.Transparent);
                textBox.BorderThickness = new Thickness(0);

                // Align text to center
                textBox.HorizontalAlignment = HorizontalAlignment.Stretch;
                textBox.VerticalAlignment = VerticalAlignment.Center;
                textBox.TextAlignment = TextAlignment.Center;
            }
            else if (text is TextBlock textBlock)
            {
                // Align text to center
                textBlock.HorizontalAlignment = HorizontalAlignment.Stretch;
                textBlock.VerticalAlignment = VerticalAlignment.Center;
                textBlock.TextAlignment = TextAlignment.Center;
            }

            // Text size
            text.SetBinding(FontSizeProperty, new Binding("TextFontSize") { Source = this, Mode = BindingMode.OneWay });

            // Enable/disable editing of grid without greying out
            Binding enabledBinding = new Binding("Enabled") { Source = this };
            text.SetBinding(IsHitTestVisibleProperty, enabledBinding);
            text.SetBinding(FocusableProperty, enabledBinding);

            SetGridPosition(grid, i, j); // Set position in sudoku grid
            grid.Children.Add(text);

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
