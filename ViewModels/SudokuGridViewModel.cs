using Models;
using System.ComponentModel;

namespace ViewModels
{
    /// <summary>
    /// View model to provide the data for the contents of the sudoku
    /// </summary>
    public class SudokuGridViewModel : INotifyPropertyChanged
    {
        private readonly SudokuGridModel sudokuGrid;
        private readonly CellViewModel[,] cells = new CellViewModel[9, 9]; // Keeps track of the data about all the cells in the grid
        private bool displayingErrors = false; // Keeps track of whether the errors are currently highlighted in red

        private bool hasClickedSolve = false; // Keeps track of whether the sudoku was solved by the computer
        public bool IsButtonShowingSave { get; private set; } = false; // Tells the UI whether the button should show 'save' or 'check'/'solve'

        // Tells the UI whether the button should show 'solve' or 'check'
        private string solveOrCheck;
        public string SolveOrCheck
        {
            get => solveOrCheck;
            private set
            {
                IsButtonShowingSave = value.ToLower() == "save";

                solveOrCheck = value;
                Notify(nameof(SolveOrCheck));
            }
        }

        // If the computer solved the sudoku, then disable the check button as this is for when the user solved the sudoku themselves
        public bool IsCheckButtonEnabled
        {
            get => !hasClickedSolve;
        }

        /// <summary>
        /// Clears errors and updates check/solve button when a cell is changed
        /// </summary>
        public void HandleCellNumberChanged(int oldValue, int newValue)
        {
            hasClickedSolve = false;
            ClearErrors();

            //if (sudokuGrid.IsFull()) SolveOrCheck = "Check";
            //else SolveOrCheck = "Solve";
            SolveOrCheck = sudokuGrid.IsFull() ? "Check" : "Solve";
            Notify(nameof(IsCheckButtonEnabled));
        }

        /// <summary>
        /// Create a new sudoku grid view model with the provided <paramref name="grid"/>
        /// </summary>
        /// <param name="grid">The contents of the sudoku</param>
        public SudokuGridViewModel(int[,] grid)
        {
            sudokuGrid = new SudokuGridModel(grid);
            SolveOrCheck = "Check"; // Sets the button text to 'check'

            // Fill the grid and add the events for when a cell is changed
            for (int j = 0; j < 9; j++)
            {
                for (int i = 0; i < 9; i++)
                {
                    sudokuGrid.Data[i, j].CellNumberModifiedEvent += HandleCellNumberChanged; // Add the event handler
                    cells[i, j] = new CellViewModel(sudokuGrid.Data[i, j]);
                    if (cells[i, j].Number == -1) SolveOrCheck = "Solve"; // If the cell is empty, the user has not solved the sudoku completely, so show the solve button
                }
            }
        }

        /// <summary>
        /// Create a new sudoku grid view model from the provided <paramref name="grid"/>
        /// </summary>
        public SudokuGridViewModel(SudokuGridModel grid)
        {
            sudokuGrid = grid;
            SolveOrCheck = "Check"; // Sets the button text to 'check'

            // Fill the grid and add the events for when a cell is changed
            for (int j = 0; j < 9; j++)
            {
                for (int i = 0; i < 9; i++)
                {
                    sudokuGrid.Data[i, j].CellNumberModifiedEvent += HandleCellNumberChanged; // Add the event handler
                    cells[i, j] = new CellViewModel(sudokuGrid.Data[i, j]);
                    if (cells[i, j].Number == -1) SolveOrCheck = "Solve"; // If the cell is empty, the user has not solved the sudoku completely, so show the solve button
                }
            }
        }

        /// <summary>
        /// Handles solving of the sudoku
        /// </summary>
        /// <returns>Whether the sudoku was solved successfully</returns>
        public bool Solve()
        {
            if (!sudokuGrid.Solve()) return false;
            else
            {
                UpdateCellsUI(); // Notify the UI that the cells have changed
                hasClickedSolve = true;
                Notify(nameof(IsCheckButtonEnabled));
                return true;
            }
        }

        /// <summary>
        /// Loops through all the cells and updates their UI
        /// </summary>
        private void UpdateCellsUI()
        {
            foreach (CellViewModel cell in cells) cell.NotifyChange(number: true, colour: true);
        }

        /// <summary>
        /// Works out if each cell is valid then updates the UI accordingly
        /// </summary>
        /// <returns>True if there are errors, false otherwise</returns>
        public bool DisplayErrors()
        {
            displayingErrors = true;

            bool hasErrors = false; // Starts with 0 errors then loops through each cell. If one is invalid, then set hasErrors to true
            for (int j = 0; j < 9; j++)
            {
                for (int i = 0; i < 9; i++)
                {
                    bool isValid = sudokuGrid.IsCellValid(i, j);
                    if (!isValid) hasErrors = true;

                    cells[i, j].SetIsValid(isValid);
                    cells[i, j].NotifyChange(number: true, colour: true);
                }
            }

            SolveOrCheck = "Save"; // Now the sudoku has been solved, the user can save it
            return hasErrors;
        }

        /// <summary>
        /// Stops showing the errors on the sudoku grid
        /// </summary>
        public void ClearErrors()
        {
            if (!displayingErrors) return;

            displayingErrors = false;
            for (int j = 0; j < 9; j++)
            {
                for (int i = 0; i < 9; i++)
                {
                    cells[i, j].SetIsValid(true);
                    cells[i, j].NotifyChange(colour: true);
                }
            }
        }

        /// <summary>
        /// Returns whether the grid is full or not
        /// </summary>
        /// <returns>True if the grid is filled, false if there is at least one empty cell</returns>
        public bool IsFull()
        {
            return sudokuGrid.IsFull();
        }

        /// <summary>
        /// Clears the sudoku grid
        /// </summary>
        public void Clear()
        {
            sudokuGrid.Clear();
            UpdateCellsUI();
        }

        /// <summary>
        /// Access a cell in the sudoku grid at a given <paramref name="index"/>
        /// </summary>
        /// <param name="index">The index of the cell in the format from <see cref="GetStringIndex(int, int)"/></param>
        /// <returns>The cell view model for the cell at index [i, j]</returns>
        public CellViewModel this[int i, int j]
        {
            get
            {
                return cells[i, j];
            }
        }

        public SudokuGridModel GetModel() => sudokuGrid;

        // Notifier for when a property changes
        public event PropertyChangedEventHandler PropertyChanged;
        private void Notify(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
