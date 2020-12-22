using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
        private void BtnSelectSudokuImage(object sender, RoutedEventArgs e)
        {
            // Prompt the user to select the sudoku image
            OpenFileDialog fileDialog = new OpenFileDialog { Filter = "Image (*.png, *.jpg)|*.png;*.jpg" };
            fileDialog.ShowDialog();

            // Navigate to image adjustments page and pass the image path
            NavigationService.Navigate(new ImageAdjustmentsPage(fileDialog.FileName));
        }
    }
}
