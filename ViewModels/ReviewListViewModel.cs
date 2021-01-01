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
        private readonly List<ReviewEntryModel> reviewEntries;

        // Contains all the entries
        public List<ReviewEntryViewModel> Entries { 
            get => reviewEntries.Select((entryModel) => new ReviewEntryViewModel(entryModel)).ToList(); // Convert ReviewEntryModel to ReviewEntryViewModel
            private set => Entries = value;
        }

        // Provides the success rate of the user
        public float SuccessRate { get => reviewEntries.Sum((entry) => entry.WasSolvedSuccessfully() ? 1 : 0) / reviewEntries.Count * 100; }

        // TODO: Remove temporary code
        public ReviewListViewModel()
        {
            reviewEntries = new List<ReviewEntryModel> {
                new ReviewEntryModel(),
                new ReviewEntryModel(),
                new ReviewEntryModel(),
                new ReviewEntryModel(),
                new ReviewEntryModel(),
                new ReviewEntryModel(),
                new ReviewEntryModel(),
                new ReviewEntryModel(),
                new ReviewEntryModel(),
                new ReviewEntryModel(),
                new ReviewEntryModel(),
                new ReviewEntryModel(),
                new ReviewEntryModel(),
                new ReviewEntryModel(),
                new ReviewEntryModel(),
                new ReviewEntryModel(),
                new ReviewEntryModel()
            };
        }
    }
}
