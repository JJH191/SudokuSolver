using System;

namespace Models
{
    public class ReviewEntryModel
    {
        public string ImagePath { get; private set; }
        public DateTime Date { get; private set; }
        public SudokuGridModel SudokuGrid { get; set; }

        public ReviewEntryModel(string imagePath, DateTime date)
        {
            ImagePath = imagePath;
            Date = date;
        }

        public bool WasSolvedSuccessfully() => SudokuGrid.IsValid();
    }
}
