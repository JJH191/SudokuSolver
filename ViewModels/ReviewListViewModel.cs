using System.Collections.Generic;
using System.Linq;

namespace ViewModels
{
    /// <summary>
    /// View model to provide the data for the list of all review entries
    /// </summary>
    public class ReviewListViewModel
    {
        // Contains all the entries
        public List<ReviewEntryViewModel> List { get; private set; }

        // Provides the success rate of the user
        public float SuccessRate { get => List.Sum((entry) => entry.SolvedSuccessfully ? 1 : 0) * 100 / (float)List.Count; }

        // TODO: Remove temporary code
        public ReviewListViewModel()
        {
            List = new List<ReviewEntryViewModel> {
                new ReviewEntryViewModel(),
                new ReviewEntryViewModel(),
                new ReviewEntryViewModel(),
                new ReviewEntryViewModel(),
                new ReviewEntryViewModel(),
                new ReviewEntryViewModel(),
                new ReviewEntryViewModel(),
                new ReviewEntryViewModel(),
                new ReviewEntryViewModel(),
                new ReviewEntryViewModel(),
                new ReviewEntryViewModel(),
                new ReviewEntryViewModel(),
                new ReviewEntryViewModel(),
                new ReviewEntryViewModel(),
                new ReviewEntryViewModel(),
                new ReviewEntryViewModel(),
                new ReviewEntryViewModel()
            };
        }
    }
}
