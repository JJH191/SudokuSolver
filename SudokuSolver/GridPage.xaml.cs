using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using ViewModels;

namespace SudokuSolver
{
    /// <summary>
    /// Interaction logic for GridPage.xaml
    /// </summary>
    public partial class GridPage : Page
    {
        public SudokuGridViewModel sudokuGrid;// = new SudokuGridViewModel();

        public GridPage(int[,] sudoku)
        {
            sudokuGrid = new SudokuGridViewModel(sudoku);
            InitializeComponent();

            for (int j = 0; j < 9; j++)
            {
                for (int i = 0; i < 9; i++)
                {
                    GrdSudokuGrid.Children.Add(GetCell(i, j));
                }
            }
        }

        #region Generating Grid
        TextBox GetCell(int i, int j)
        {
            TextBox tb = new TextBox();
            tb.SetBinding(TextBox.TextProperty, GetBindingForCell(i, j)); // Set binding
            SetGridPosition(tb, i, j); // Set grid position

            // Make transparent
            tb.Background = new SolidColorBrush(Colors.Transparent);
            tb.BorderThickness = new Thickness(0);

            // Align text to center
            tb.HorizontalAlignment = HorizontalAlignment.Stretch;
            tb.VerticalAlignment = VerticalAlignment.Center;
            tb.TextAlignment = TextAlignment.Center;

            // Text size
            tb.FontSize = 20;

            tb.TextChanged += ValidateInput;

            return tb;
        }

        private static readonly Regex numberRegex = new Regex("[1-9]+");
        private void ValidateInput(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;

            // If user has tried to input more than 1 character, remove it and set cursor position back
            if (textBox.Text.Length > 0)
            {
                textBox.Text = textBox.Text.Substring(0, 1);
                textBox.CaretIndex = 1;
            }

            // If the user has tried to input anything but a number, don't allow it
            if (!numberRegex.IsMatch(textBox.Text)) textBox.Text = "";
        }

        BindingBase GetBindingForCell(int i, int j)
        {
            string binding = $"[{SudokuGridViewModel.GetStringIndex(i, j)}]";
            return new Binding(binding) { Source = sudokuGrid };
        }

        private void SetGridPosition(UIElement elem, int i, int j)
        {
            Grid.SetColumn(elem, i);
            Grid.SetRow(elem, j);
        }
        #endregion

        private void BtnSolveSudoku_Click(object sender, RoutedEventArgs e)
        {
            sudokuGrid.Solve();
        }

        private void BtnClearSudoku_Click(object sender, RoutedEventArgs e)
        {
            sudokuGrid.Clear();
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
