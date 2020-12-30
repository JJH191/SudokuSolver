using System.Collections.Generic;
using System.Linq;

namespace ViewModels
{
    public class ReviewListViewModel
    {
        public List<ReviewEntryViewModel> List { get; private set; }

        public float SuccessRate { get => List.Sum((entry) => entry.SolvedSuccessfully ? 1 : 0) * 100 / (float)List.Count; }

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
                new ReviewEntryViewModel()
            };
        }
    }
}
