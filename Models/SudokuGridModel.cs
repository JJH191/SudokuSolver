using System;

namespace Models
{
    /// <summary>
    /// A class to store the contents of a sudoku grid, including functionality such as solving and checking if the board is valid
    /// </summary>
    public class SudokuGridModel
    { 
        public int[,] Data { get; private set; }

        /// <summary>
        /// Create a new sudoku grid with the provided <paramref name="data"/>
        /// </summary>
        /// <param name="data">Data to fill the sudoku grid with</param>
        public SudokuGridModel(int [,] data)
        {
            Data = data;
        }

        /// <summary>
        /// Sets all values in the grid to -1 (empty)
        /// </summary>
        public void Clear()
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    Data[i, j] = -1;
                }
            }
        }

        /// <summary>
        /// Solves the sudoku recursively
        /// </summary>
        /// <returns>Whether the solve was successfull</returns>
        public bool Solve()
        {
            if (!IsValid()) return false; // If the board is invalid to begin with, the solve was unsuccessful

            int[,] solvedGrid = (int[,])Data.Clone(); // Copy the grid so that it can be reset if the solve was unsuccessful
            if (SolveSudokuHelper(ref solvedGrid)) // Call the recursive solving function
            {
                Data = solvedGrid;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Helper function for the solver which does the recursion
        /// A helper function is used as it means that a copy of the grid can be
        /// used to prevent modifying the actual grid, allowing us to return to
        /// the original grid if the solve was unsuccessful
        /// </summary>
        /// <param name="grid">The grid to solve</param>
        /// <returns>Whether it was solvable</returns>
        private bool SolveSudokuHelper(ref int[,] grid)
        {
            // Loop through all the cells
            for (int j = 0; j < 9; j++)
            {
                for (int i = 0; i < 9; i++)
                {
                    // If the cell is empty
                    if (grid[i, j] == -1)
                    {
                        // Try all the numbers from 1 to 9 in the cell
                        for (int n = 1; n < 10; n++)
                        {
                            // Check if the guess is valid in the given position
                            if (IsNumberValid(grid, n, i, j))
                            {
                                // If it is valid, set the cell to the new number and try solving the rest
                                grid[i, j] = n;

                                // If the rest could not be solved, set the cell back to empty
                                if (!SolveSudokuHelper(ref grid)) grid[i, j] = -1;
                                // If the rest could be solved, and the grid is now complete, the sudoku is solved, so return true
                                else if (IsFull(grid)) return true; 
                            }
                        }

                        // If none of the numbers worked in the cell, some cell before was invalid
                        // So return false to tell the previous call that the guess was wrong
                        return false;
                    }
                }
            }

            // If all the cells have been filled successfully, return true
            return true;
        }

        /// <summary>
        /// Checks if the grid is full
        /// </summary>
        /// <param name="sudoku">Sudoku grid to check if it's full</param>
        /// <returns>True if grid is full, false if not</returns>
        private bool IsFull(int[,] sudoku)
        {
            for (int i = 0; i < sudoku.GetLength(0); i++)
            {
                for (int j = 0; j < sudoku.GetLength(1); j++)
                {
                    if (sudoku[i, j] == -1) return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Checks if a number would be valid in a given position
        /// </summary>
        /// <param name="sudoku">Sudoku grid to insert the number into</param>
        /// <param name="number">The number to insert</param>
        /// <param name="x">The x position of the cell to insert the number into</param>
        /// <param name="y">The y position of the cell to insert the number into</param>
        /// <returns>True if the number is valid in the given position, false otherwise</returns>
        private bool IsNumberValid(int[,] sudoku, int number, int x, int y)
        {
            // Check rows
            for (int i = 0; i < 9; i++)
                if (i != x && sudoku[i, y] != -1 && sudoku[i, y] == number) return false;

            // Check columns
            for (int i = 0; i < 9; i++)
                if (i != y && sudoku[x, i] != -1 && sudoku[x, i] == number) return false;

            // Check 3x3 groups of cells
            int groupTop = (int)Math.Floor(y / 3f) * 3;
            for (int j = groupTop; j < groupTop + 3; j++)
            {
                if (j == y) continue;
                int groupLeft = (int)Math.Floor(x / 3f) * 3;
                for (int i = groupLeft; i < groupLeft + 3; i++)
                {
                    if (i == x || sudoku[i, j] == -1) continue;
                    if (sudoku[i, j] == number) return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Checks if the whole sudoku grid is valid
        /// </summary>
        /// <returns>True if the grid is valid, false if not</returns>
        public bool IsValid()
        {
            // Loop through all the cells and see if the number in that cell is valid
            for (int j = 0; j < 9; j++)
            {
                for (int i = 0; i < 9; i++)
                {
                    if (!IsNumberValid(Data, Data[i, j], i, j)) return false;
                }
            }
            return true;
        }
    }
}
