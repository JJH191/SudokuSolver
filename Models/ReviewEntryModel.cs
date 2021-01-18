using Common;
using System;
using System.Collections.Generic;

namespace Models
{
    public class ReviewEntryModel
    {
        // TODO: Remove temporary code
        //private static readonly Random r = new Random();

        public string ImagePath { get; private set; }
        public DateTime Date { get; private set; }
        public SudokuGridModel SudokuGrid { get; set; }

        public ReviewEntryModel(string imagePath, DateTime date)
        {
            ImagePath = imagePath;
            Date = date;
        }

        // TODO: Return locations of errors 
        //public List<Vector2I> GetErrors() => SudokuGrid.GetErrors();

        public bool WasSolvedSuccessfully() => SudokuGrid.IsValid();
    }
}
