using Common;
using Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Data;
using System.Windows.Media;

namespace ViewModels
{
    /// <summary>
    /// View model to provide the data for the contents of the sudoku
    /// </summary>
    public class SudokuGridViewModel : INotifyPropertyChanged
    {
        private readonly SudokuGridModel sudokuGrid;
        private readonly CellViewModel[,] cells = new CellViewModel[9, 9];
        private bool displayingErrors = false;

        private bool hasClickedSolve = false;
        public bool IsButtonShowingSave { get; private set; } = false;

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

        public bool IsCheckButtonEnabled
        {
            get => !hasClickedSolve;
        }

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
        /// Create a new sudoku grid model with the provided <paramref name="grid"/>
        /// </summary>
        /// <param name="grid">The contents of the sudoku</param>
        public SudokuGridViewModel(int[,] grid)
        {
            sudokuGrid = new SudokuGridModel(grid);
            SolveOrCheck = "Check";

            for (int j = 0; j < 9; j++)
            {
                for (int i = 0; i < 9; i++)
                {
                    sudokuGrid.Data[i, j].CellNumberModifiedEvent += HandleCellNumberChanged;
                    cells[i, j] = new CellViewModel(sudokuGrid.Data[i, j]);
                    if (cells[i, j].Number == -1) SolveOrCheck = "Solve";
                }
            }
        }

        public SudokuGridViewModel(SudokuGridModel grid)
        {
            sudokuGrid = grid;
            SolveOrCheck = "Check";

            for (int j = 0; j < 9; j++)
            {
                for (int i = 0; i < 9; i++)
                {
                    sudokuGrid.Data[i, j].CellNumberModifiedEvent += HandleCellNumberChanged;
                    cells[i, j] = new CellViewModel(sudokuGrid.Data[i, j]);
                    if (cells[i, j].Number == -1) SolveOrCheck = "Solve";
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
                UpdateCellsUI();
                hasClickedSolve = true;
                Notify(nameof(IsCheckButtonEnabled));
                return true;
            }
        }

        private void UpdateCellsUI()
        {
            foreach (CellViewModel cell in cells) cell.NotifyChange();
        }

        public bool DisplayErrors()
        {
            displayingErrors = true;

            bool hasErrors = false;
            for (int j = 0; j < 9; j++)
            {
                for (int i = 0; i < 9; i++)
                {
                    bool isValid = sudokuGrid.IsCellValid(i, j);
                    if (!isValid) hasErrors = true;

                    cells[i, j].SetIsValid(isValid);
                    cells[i, j].NotifyChange();
                }
            }

            SolveOrCheck = "Save";
            return hasErrors;
        }

        public void ClearErrors()
        {
            if (!displayingErrors) return;

            displayingErrors = false;
            for (int j = 0; j < 9; j++)
            {
                for (int i = 0; i < 9; i++)
                {
                    cells[i, j].SetIsValid(true);
                    cells[i, j].NotifyChange(number: false);
                }
            }
        }

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
        /// <returns></returns>
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
