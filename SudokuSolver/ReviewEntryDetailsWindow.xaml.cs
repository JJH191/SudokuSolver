using System.Windows;
using ViewModels;

namespace SudokuSolver
{
    /// <summary>
    /// Interaction logic for ReviewEntryDetailsWindow.xaml
    /// </summary>
    public partial class ReviewEntryDetailsWindow : Window
    {
        public ReviewEntryDetailsWindow(ReviewEntryViewModel reviewEntry)
        {
            InitializeComponent();
            DataContext = reviewEntry;
        }
    }
}
