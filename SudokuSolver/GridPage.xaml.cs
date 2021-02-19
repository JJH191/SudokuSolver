using Database;
using System.Diagnostics;
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
        private readonly SudokuGridViewModel sudokuGrid; // View model containing all the info about the cells
        private readonly string imagePath; // The path to the image of this duoku

        public GridPage(int[,] sudoku, string imagePath)
        {
            InitializeComponent();

            this.imagePath = imagePath;
            sudokuGrid = new SudokuGridViewModel(sudoku);
            DataContext = sudokuGrid;
            Process.Start(imagePath);
        }

        /// <summary>
        /// Navigates back to the main menu which is 2 pages back
        /// </summary>
        private void NagivateToMainMenu()
        {
            NavigationService.GoBack();
            NavigationService.GoBack();
        }

        /// <summary>
        /// Copies the image to ./images so the user can move/rename/delete the original image without issues
        /// The images are named in sequential order to avoid any potential name collisions
        /// </summary>
        /// <param name="imagePath">The path to the original image</param>
        /// <returns>The new path that it was saved at</returns>
        private string SaveCopyOfImage(string imagePath)
        {
            string newPath = $"./images/{GetNextImageIndex("./images")}.png";
            new Bitmap(imagePath).Save(newPath);
            return newPath;
        }

        /// <summary>
        /// Works out the current max image index in the folder for storing the images and then adds 1 to it
        /// If there are no images, it returns 1
        /// </summary>
        /// <param name="directory">The directory to save the image to</param>
        /// <returns>The next index to be used for the image name</returns>
        private int GetNextImageIndex(string directory)
        {
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory); // Create the directory if it doesn't exist to avoid errors

            // Sort the images by index (removing extension from the filename) and get the maximum or default (0)
            int mostRecent = Directory.GetFiles(directory).Select((file) =>
            {
                if (!int.TryParse(GetFileNameWithoutExtension(file), out int fileNumber)) return int.MinValue;
                return fileNumber;
            }).OrderByDescending(number => number).FirstOrDefault();

            // Add one to the current maximum index
            return mostRecent + 1;
        }

        /// <summary>
        /// Gets the file name from a path, removing extensions and parent directories
        /// </summary>
        /// <param name="file">The path to strip down to a file name</param>
        /// <returns>The file name without an extension</returns>
        private string GetFileNameWithoutExtension(string file)
        {
            int start = file.Replace('/', '\\').LastIndexOf("\\") + 1;
            int end = file.LastIndexOf(".");

            return file.Substring(start, end - start);
        }

        #region Button Events
        /// <summary>
        /// Handles the outcome of an attempt to solve the sudoku
        /// </summary>
        private void BtnSolveSudoku_Click(object sender, RoutedEventArgs e)
        {
            Stopwatch s = new Stopwatch();
            s.Start();
            if (!sudokuGrid.IsFull()) // Solve
            {
                if (!sudokuGrid.Solve()) // If the solve fails, show an error message
                    MessageBox.Show("Could not solve the sudoku. Check that the numbers are correct", "Error solving", MessageBoxButton.OK, MessageBoxImage.Error);
                else
                {
                    s.Stop();
                    Trace.WriteLine($"Solving took {s.ElapsedMilliseconds}ms");
                }
            }
            else // Check or save the sudoku
            {
                if (!sudokuGrid.IsButtonShowingSave) // Check
                {
                    bool errors = sudokuGrid.DisplayErrors(); // Show all the invalid cells in red
                    if (!errors) MessageBox.Show("You made no mistakes, well done!", "Congratulations!"); // Congratulate the user
                }
                else // Save
                {
                    SqliteDataAccess.Save(sudokuGrid.GetModel(), SaveCopyOfImage(imagePath)); // Save the sudoku to the database to be reviewed in the home section
                    MessageBox.Show("Sudoku saved!", "Saved", MessageBoxButton.OK, MessageBoxImage.Information); // Give feedback to the user

                    NagivateToMainMenu();
                }
            }
        }

        /// <summary>
        /// Goes back to the previous page
        /// </summary>
        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        /// <summary>
        /// Goes back to the home page
        /// </summary>
        private void BtnHome_Click(object sender, RoutedEventArgs e)
        {
            NagivateToMainMenu();
        }
        #endregion
    }
}
