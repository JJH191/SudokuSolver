using System;
using System.Drawing;

namespace ViewModels
{
    /// <summary>
    /// View model to provide the data for an entry in the review section
    /// </summary>
    public class ReviewEntryViewModel
    {
        // TODO: Remove temporary code
        private static readonly Random r = new Random();

        public Bitmap Image { get; private set; }
        public DateTime Date { get; private set; }
        public bool SolvedSuccessfully { get; private set; }

        public ReviewEntryViewModel()
        {
            Image = new Bitmap($@"C:\Users\Jacob\Downloads\sudoku{r.Next(2, 5)}.jpg");
            Date = DateTime.Now.AddDays(-r.NextDouble() * 10000);
            SolvedSuccessfully = r.NextDouble() > 0.5;
        }
    }
}
