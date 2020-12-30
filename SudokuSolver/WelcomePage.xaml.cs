using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace SudokuSolver
{
    /// <summary>
    /// Interaction logic for WelcomePage.xaml
    /// </summary>
    public partial class WelcomePage : Page
    {
        public WelcomePage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Prompt the user to select their sudoku image then navigate to the next page
        /// </summary>
        private void Btn_Solve(object sender, RoutedEventArgs e)
        {
            // Prompt the user to select the sudoku image
            OpenFileDialog fileDialog = new OpenFileDialog { Filter = "Image (*.png, *.jpg)|*.png;*.jpg" };

            // If the user hasn't selected a file, return
            if (fileDialog.ShowDialog() == false) return;

            // Navigate to image adjustments page and pass the image path
            NavigationService.Navigate(new ImageAdjustmentsPage(fileDialog.FileName));
        }

        private void Btn_Review(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ReviewPage());
        }
    }
}
