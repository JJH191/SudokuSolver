using Models;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Data;

namespace ViewModels
{
    /// <summary>
    /// View model to provide the data for the quad that marks the outline of the sudoku
    /// </summary>
    // Not my code
    public class SudokuGridViewModel : INotifyPropertyChanged
    {
        private readonly SudokuGridModel sudokuGrid;

        /// <summary>
        /// Create a new sudoku grid model with the provided <paramref name="grid"/>
        /// </summary>
        /// <param name="grid">The contents of the sudoku</param>
        public SudokuGridViewModel(int[,] grid)
        {
            sudokuGrid = new SudokuGridModel(grid);
        }

        /// <summary>
        /// Handles solving of the sudoku
        /// </summary>
        public void Solve()
        {
            // TODO: Response to success/fail
            if (!sudokuGrid.Solve()) Debug.WriteLine("Did not solve");
            else Notify(Binding.IndexerName);
        }

        /// <summary>
        /// Clears the sudoku grid
        /// </summary>
        public void Clear()
        {
            sudokuGrid.Clear();
            Notify(Binding.IndexerName);
        }

        /// <summary>
        /// Splits a string index into two integers
        /// </summary>
        /// <param name="index">The string index</param>
        /// <param name="i">The first index of the cell</param>
        /// <param name="j">The second index of the cell</param>
        private void SplitIndex(string index, out int i, out int j)
        {
            string[] ij = index.Split('-'); // Split the index into i and j
            if (ij.Length != 2) throw new ArgumentException("Invalid index"); // Should have two parts

            // Convert strings to integers
            i = int.Parse(ij[0]);
            j = int.Parse(ij[1]);
        }

        /// <summary>
        /// Get the string equivalent for an index - helps with binding in WPF
        /// </summary>
        /// <param name="i">First index</param>
        /// <param name="j">Second index</param>
        /// <returns></returns>
        public static string GetStringIndex(int i, int j)
        {
            return i.ToString() + "-" + j.ToString();
        }

        #region Array Methods
        /// <summary>
        /// Access a cell in the sudoku grid at a given <paramref name="index"/>
        /// </summary>
        /// <param name="index">The index of the cell in the format from <see cref="GetStringIndex(int, int)"/></param>
        /// <returns></returns>
        public string this[string index]
        {
            get
            {
                SplitIndex(index, out int i, out int j);
                if (sudokuGrid.Data[i, j] == -1) return "";
                return sudokuGrid.Data[i, j].ToString();
            }
            set
            {
                SplitIndex(index, out int i, out int j);
                if (value.Trim().Length == 0) sudokuGrid.Data[i, j] = -1;
                else sudokuGrid.Data[i, j] = int.Parse(value);
                Notify(Binding.IndexerName);
            }
        }

        /// <summary>
        /// Access a cell in the sudoku grid at the index [<paramref name="i"/>, <paramref name="j"/>]
        /// </summary>
        /// <param name="i">The first index of the cell</param>
        /// <param name="j">The second index of the cell</param>
        /// <returns></returns>
        public int this[int i, int j]
        {
            get => sudokuGrid.Data[i, j]; // Get the cell
            set
            {
                sudokuGrid.Data[i, j] = value; // Set the value
                Notify(Binding.IndexerName); // Notify the UI of a change
            }
        }
        #endregion

        // Notifier for when a property changes
        public event PropertyChangedEventHandler PropertyChanged;
        private void Notify(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
