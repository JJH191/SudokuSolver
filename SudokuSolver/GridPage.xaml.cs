using Database;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ViewModels;

namespace SudokuSolver
{
    /// <summary>
    /// Interaction logic for GridPage.xaml
    /// </summary>
    public partial class GridPage : Page
    {
        private readonly SudokuGridViewModel sudokuGrid;
        private readonly string imagePath;

        public GridPage(int[,] sudoku, string imagePath)
        {
            InitializeComponent();

            this.imagePath = imagePath;
            sudokuGrid = new SudokuGridViewModel(sudoku);
            DataContext = sudokuGrid;
        }

        private void BtnSolveSudoku_Click(object sender, RoutedEventArgs e)
        {

            if (!sudokuGrid.IsFull()) // Solve
            {
                if (!sudokuGrid.Solve())
                {
                    MessageBox.Show("Could not solve the sudoku.\nThis is most likely because one or more numbers are incorrect.", "Error solving");
                }
            }
            else // Check or save
            {
                if (!sudokuGrid.IsButtonShowingSave)// Check
                {
                    bool errors = sudokuGrid.DisplayErrors();
                    if (!errors) MessageBox.Show("You made no mistakes, well done!", "Congratulations!");
                }
                else // Save
                {
                    SqliteDataAccess.Save(sudokuGrid.GetModel(), SaveCopyOfImage(imagePath));
                    MessageBox.Show("Sudoku saved!", "Saved");

                    // Go back to main menu
                    NavigationService.GoBack();
                    NavigationService.GoBack();
                }
            }
        }

        private string SaveCopyOfImage(string imagePath)
        {
            string newPath = $"./images/{GetNextImageIndex("./images")}.png";
            new Bitmap(imagePath).Save(newPath);
            return newPath;
        }

        private int GetNextImageIndex(string directory)
        {
            int mostRecent = Directory.GetFiles(directory).Select((file) =>
            {
                if (!int.TryParse(GetFileNameWithoutExtension(file), out int fileNumber)) return int.MinValue;
                return fileNumber;
            }).OrderByDescending(number => number).FirstOrDefault();

            return mostRecent + 1;
        }

        private string GetFileNameWithoutExtension(string file)
        {
            int start = file.Replace('/', '\\').LastIndexOf("\\") + 1;
            int end = file.LastIndexOf(".");

            return file.Substring(start, end - start);
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
