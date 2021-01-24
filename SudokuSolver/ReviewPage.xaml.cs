using Database;
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
        private readonly ReviewListViewModel reviewEntries;

        public ReviewPage()
        {
            InitializeComponent();

            reviewEntries = new ReviewListViewModel(SqliteDataAccess.GetReviewEntries());
            DataContext = reviewEntries;
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
