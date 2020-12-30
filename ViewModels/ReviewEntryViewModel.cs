using System;
using System.Drawing;
using System.Windows.Media.Imaging;

namespace ViewModels
{
    public class ReviewEntryViewModel
    {
        private static readonly Random r = new Random();

        public Bitmap Image { get; private set; }
        public DateTime Date { get; private set; }
        public bool SolvedSuccessfully { get; private set; }

        public ReviewEntryViewModel()
        {
            Image = new Bitmap($@"C:\Users\Jacob\Downloads\sudoku{r.Next(2, 5)}.jpg");
            Date = DateTime.Now;//.AddDays(-r.NextDouble() * 10000);
            SolvedSuccessfully = r.NextDouble() > 0.5;
        }
    }
}
