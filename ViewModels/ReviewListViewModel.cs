using Database;
using Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace ViewModels
{
    /// <summary>
    /// View model to provide the data for the list of all review entries
    /// </summary>
    public class ReviewListViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<ReviewEntryViewModel> reviewEntries;

        // Contains all the entries
        public ObservableCollection<ReviewEntryViewModel> Entries
        {
            get => reviewEntries;
            private set
            {
                reviewEntries = value;
                Notify(nameof(Entries));
            }
        }

        // Provides the success rate of the user
        public float SuccessRate
        {
            get
            {
                if (reviewEntries.Count == 0) return 0; // Avoid divide by 0 error
                return reviewEntries.Sum((entry) => entry.SolvedSuccessfully ? 1 : 0) / (float)reviewEntries.Count * 100;
            }
        }

        private int selectedIndex = 0;
        public int SelectedIndex
        {
            get => selectedIndex;
            set
            {
                selectedIndex = value;

                // Work out whether the user selected newest or oldest
                bool isReversed = selectedIndex == 0;

                // Create a list from the ObservableCollection (as you can't directly sort ObservableCollections)
                List<ReviewEntryViewModel> entries = Entries.ToList();
                entries.Sort(new Comparison<ReviewEntryViewModel>(
                    (entry1, entry2) => entry1.Date.CompareTo(entry2.Date) * (isReversed ? -1 : 1)
                )); // Sort the list in the correct order

                // Set the ObservableCollection to have the items in the correct order
                for (int i = 0; i < entries.Count; i++) Entries[i] = entries[i];
            }
        }

        public ReviewListViewModel()
        {
            List<ReviewEntryModel> reviewEntryModels = SqliteDataAccess.GetReviewEntries();
            Entries = new ObservableCollection<ReviewEntryViewModel>(reviewEntryModels.Select((entryModel) => new ReviewEntryViewModel(entryModel)).ToList());
            Entries.CollectionChanged += ReviewEntries_CollectionChanged;
        }

        private void ReviewEntries_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Notify(nameof(Entries));
        }

        // Notifier for when a property changes
        public event PropertyChangedEventHandler PropertyChanged;
        private void Notify(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
