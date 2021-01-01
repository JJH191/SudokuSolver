using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using ViewModels;

namespace SudokuSolver
{
    /// <summary>
    /// Interaction logic for ReviewPage.xaml
    /// </summary>
    public partial class ReviewPage : Page
    {
        public ReviewEntryViewModel SelectedItem { get; set; }

        public ReviewPage()
        {
            InitializeComponent();
        }

        private void LstReviewList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            new ReviewEntryDetailsWindow(SelectedItem).Show(); 
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
