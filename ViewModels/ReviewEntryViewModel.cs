using Models;
using System;
using System.Drawing;

namespace ViewModels
{
    /// <summary>
    /// View model to provide the data for an entry in the review section
    /// </summary>
    public class ReviewEntryViewModel
    {
        private readonly ReviewEntryModel model;
        private readonly SudokuGridViewModel sudokuGridViewModel;

        public ReviewEntryViewModel(ReviewEntryModel model)
        {
            this.model = model;
            sudokuGridViewModel = new SudokuGridViewModel(model.SudokuGrid);
            sudokuGridViewModel.DisplayErrors();
        }

        private Bitmap cachedImage;
        public Bitmap Image { 
            get
            {
                if (cachedImage == null) cachedImage = new Bitmap(model.ImagePath);
                return cachedImage;
            }
        }

        public DateTime Date { get => model.Date; } 
        public bool SolvedSuccessfully { get => model.WasSolvedSuccessfully(); }
        public SudokuGridViewModel SudokuGrid { get => sudokuGridViewModel; }
    }
}
