using System;

namespace Models
{
    /// <summary>
    /// A model to keep track of a singular review entry in the review section
    /// </summary>
    public class ReviewEntryModel
    {
        public string ImagePath { get; private set; } // The path of the original image of the sudoku
        public DateTime Date { get; private set; } // The date the sudoku was checked
        public SudokuGridModel SudokuGrid { get; set; } // The values of the sudoku

        public ReviewEntryModel(string imagePath, DateTime date)
        {
            ImagePath = imagePath;
            Date = date;
        }

        // True if the sudoku is valid (and therefore solved successfully)
        public bool WasSolvedSuccessfully() => SudokuGrid.IsValid();
    }
}
