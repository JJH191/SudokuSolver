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

		/// <summary>
		/// Creates a new review entry view model with the provided model and displays the errors
		/// </summary>
		public ReviewEntryViewModel(ReviewEntryModel model)
		{
			this.model = model;
			SudokuGrid = new SudokuGridViewModel(model.SudokuGrid);
			SudokuGrid.DisplayErrors();
		}

		// Provides the image for the review entry and caches it once it has been loaded the first time
		private Bitmap cachedImage;
		public Bitmap Image
		{
			get
			{
				// If there isn't a cached image, load it
				if (cachedImage == null) cachedImage = new Bitmap(model.ImagePath);
				return cachedImage;
			}
		}

		public DateTime Date { get => model.Date; } // Provides the date that the sudoku was solved
		public bool SolvedSuccessfully { get => model.WasSolvedSuccessfully(); } // Whether the sudoku was solved correctly by the user
		public SudokuGridViewModel SudokuGrid { get; } // Provides the grid of the sudoku
	}
}
