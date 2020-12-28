using System;

namespace Models
{
    public class SudokuGridModel
    { 
        public int[,] Data { get; private set; }

        public SudokuGridModel(int [,] data)
        {
            Data = data;
        }

        //public SudokuGridModel()
        //{
        //    Data = new int[9, 9];
        //    Clear();
        //}

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

        public bool Solve()
        {
            if (!IsValid()) return false;

            int[,] solvedGrid = (int[,])Data.Clone();
            if (SolveSudokuHelper(ref solvedGrid))
            {
                Data = solvedGrid;
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool SolveSudokuHelper(ref int[,] grid)
        {
            for (int j = 0; j < 9; j++)
            {
                for (int i = 0; i < 9; i++)
                {
                    if (grid[i, j] == -1)
                    {
                        for (int n = 1; n < 10; n++)
                        {
                            if (IsNumberValid(grid, n, i, j))
                            {
                                grid[i, j] = n;
                                if (!SolveSudokuHelper(ref grid))
                                {
                                    grid[i, j] = -1;
                                }
                                else
                                {
                                    if (IsFull(grid)) return true;
                                }
                            }
                        }
                        return false;
                    }
                }
            }
            return true;
        }

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

        private bool IsNumberValid(int[,] sudoku, int number, int x, int y)
        {
            // Check row
            for (int i = 0; i < 9; i++)
                if (i != x && sudoku[i, y] != -1 && sudoku[i, y] == number) return false;

            // Check column
            for (int i = 0; i < 9; i++)
                if (i != y && sudoku[x, i] != -1 && sudoku[x, i] == number) return false;

            // Check 3x3 group of cells
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

        private bool IsValid()
        {
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
