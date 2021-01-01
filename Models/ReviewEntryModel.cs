using HelperClasses;
using System;
using System.Collections.Generic;

namespace Models
{
    public class ReviewEntryModel
    {
        // TODO: Remove temporary code
        private static readonly Random r = new Random();

        public string ImagePath { get; private set; }
        public DateTime Date { get; private set; }
        private readonly SudokuGridModel sudokuGrid;

        public ReviewEntryModel()
        {
            ImagePath = $@"C:\Users\Jacob\Downloads\sudoku{r.Next(2, 5)}.jpg";
            Date = DateTime.Now.AddDays(-r.NextDouble() * 10000);
        }

        // TODO: Return locations of errors 
        public List<Vector2I> GetErrors() => new List<Vector2I>();

        public bool WasSolvedSuccessfully() => true;// sudokuGrid.IsValid(); 
    }
}
