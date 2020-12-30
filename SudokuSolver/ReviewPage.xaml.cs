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
        public ReviewPage()
        {
            InitializeComponent();
        }

        private void ListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //MessageBox.Show(((ReviewEntryViewModel)LstReviewList.SelectedItem).Image.UriSource.LocalPath);
            new ReviewEntryDetailsWindow((ReviewEntryViewModel)LstReviewList.SelectedItem).Show();
        }

        private void Btn_Back(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
