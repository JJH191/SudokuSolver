using Common;
using System;
using System.Collections.Generic;

namespace Models
{
    /// <summary>
    /// A class to store the contents of a sudoku grid, including functionality such as solving and checking if the board is valid
    /// </summary>
    public class SudokuGridModel
    {
        // The contents of the sudoku grid
        public CellModel[,] Data { get; private set; }

        /// <summary>
        /// Create a new sudoku grid with the provided <paramref name="data"/>
        /// </summary>
        /// <param name="data">Data to fill the sudoku grid with</param>
        public SudokuGridModel(int[,] data)
        {
            Data = new CellModel[9, 9];
            for (int j = 0; j < data.GetLength(1); j++)
            {
                for (int i = 0; i < data.GetLength(0); i++)
                {
                    Data[i, j] = new CellModel(data[i, j]);
                }
            }
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
                    Data[i, j].Number = -1;
                    Data[i, j].IsValid = true;
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

            CellModel[,] solvedGrid = (CellModel[,])Data.Clone(); // Copy the grid so that it can be reset if the solve was unsuccessful
            if (SolveSudokuHelper(ref solvedGrid)) // Call the recursive solving function
            {
                Data = solvedGrid; // Set the data to be the new solved grid
                return true; // Solved successfully
            }
            else
            {
                return false; // Did not solve
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
        private bool SolveSudokuHelper(ref CellModel[,] grid)
        {
            // Loop through all the cells
            for (int j = 0; j < 9; j++)
            {
                for (int i = 0; i < 9; i++)
                {
                    // If the cell is empty
                    if (grid[i, j].Number == -1)
                    {
                        // Try all the numbers from 1 to 9 in the cell
                        for (int n = 1; n < 10; n++)
                        {
                            // Check if the guess is valid in the given position
                            if (IsNumberValid(grid, n, i, j))
                            {
                                // If it is valid, set the cell to the new number and try solving the rest
                                grid[i, j].Number = n;

                                // If the rest could not be solved, set the cell back to empty
                                if (!SolveSudokuHelper(ref grid)) grid[i, j].Number = -1;
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
        private bool IsFull(CellModel[,] sudoku)
        {
            // Loop through all the cells, if any is empty, the grid is not full so return false
            for (int i = 0; i < sudoku.GetLength(0); i++)
            {
                for (int j = 0; j < sudoku.GetLength(1); j++)
                {
                    if (sudoku[i, j].Number == -1) return false;
                }
            }

            // No cell was empty, so the grid is full
            return true;
        }

        public bool IsFull()
        {
            return IsFull(Data);
        }

        /// <summary>
        /// Checks if a number would be valid in a given position
        /// </summary>
        /// <param name="sudoku">Sudoku grid to insert the number into</param>
        /// <param name="number">The number to insert</param>
        /// <param name="x">The x position of the cell to insert the number into</param>
        /// <param name="y">The y position of the cell to insert the number into</param>
        /// <returns>True if the number is valid in the given position, false otherwise</returns>
        private bool IsNumberValid(CellModel[,] sudoku, int number, int x, int y)
        {
            // Check rows
            for (int i = 0; i < 9; i++)
                if (i != x && sudoku[i, y].Number != -1 && sudoku[i, y].Number == number) return false; // If the number matches any other number in the row, the number is not valid - return false

            // Check columns
            for (int i = 0; i < 9; i++)
                if (i != y && sudoku[x, i].Number != -1 && sudoku[x, i].Number == number) return false; // If the number matches any other number in the column, the number is not valid - return false

            // Check 3x3 groups of cells (We only need to check the four corners as the others are covered by checking the rows and columns
            int groupTop = (int)Math.Floor(y / 3f) * 3; // Get the position of the top of the 3x3 grid containing the cell (x, y)
            for (int j = groupTop; j < groupTop + 3; j++) // Loop through all y values within the 3x3 group
            {
                if (j == y) continue; // Skip if we are checking against the same y value (this has already been checked by row checks)
                int groupLeft = (int)Math.Floor(x / 3f) * 3; // Get the position of the left of the 3x3 grid containing the cell (x, y)
                for (int i = groupLeft; i < groupLeft + 3; i++) // Loop through all x values within the 3x3 group
                {
                    if (i == x || sudoku[i, j].Number == -1) continue; // Skip if we are checking against the same x value (this has already been checked by column checks)
                    if (sudoku[i, j].Number == number) return false; // If the number matches any other number in the group, the number is not valid - return false
                }
            }

            // The number is valid - return true
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
                    if (!IsNumberValid(Data, Data[i, j].Number, i, j)) return false;
                }
            }

            // No cells were invalid
            return true;
        }

        public List<Vector2I> GetErrors()
        {
            List<Vector2I> errors = new List<Vector2I>();
        
            // Loop through all the cells and see if the number in that cell is valid
            for (int j = 0; j < 9; j++)
            {
                for (int i = 0; i < 9; i++)
                {
                    if (!IsNumberValid(Data, Data[i, j].Number, i, j)) errors.Add(new Vector2I(i, j));
                }
            }
        
            return errors;
        }

        public bool IsCellValid(int i, int j)
        {
            return IsNumberValid(Data, Data[i, j].Number, i, j);
        }
    }
}
