using Database;
using Models;
using System.Collections.Generic;
using System.Linq;

namespace ViewModels
{
    /// <summary>
    /// View model to provide the data for the list of all review entries
    /// </summary>
    public class ReviewListViewModel
    {
        private readonly List<ReviewEntryViewModel> reviewEntries;

        // Contains all the entries
        public List<ReviewEntryViewModel> Entries {
            get => reviewEntries;
            private set => Entries = value;
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

        public ReviewListViewModel(List<ReviewEntryModel> reviewEntries)
        {
            this.reviewEntries = reviewEntries.Select((entryModel) => new ReviewEntryViewModel(entryModel)).ToList(); 
        }
    }
}
