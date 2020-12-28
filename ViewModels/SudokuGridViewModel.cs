using Models;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Data;

namespace ViewModels
{
    // Not my code
    public class SudokuGridViewModel : INotifyPropertyChanged
    {
        private readonly SudokuGridModel sudokuGrid;// = new SudokuGridModel();

        //public SudokuGridViewModel()
        //{
        //}

        public SudokuGridViewModel(int[,] grid)
        {
            sudokuGrid = new SudokuGridModel(grid);
        }

        public static string GetStringIndex(int i, int j)
        {
            return i.ToString() + "-" + j.ToString();
        }

        #region Public Methods
        public void Solve()
        {
            if (!sudokuGrid.Solve()) Debug.WriteLine("Did not solve");
            else Notify(Binding.IndexerName);
        }

        public void Clear()
        {
            sudokuGrid.Clear();
            Notify(Binding.IndexerName);
        }
        #endregion

        #region Array Methods
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

        public int this[int i, int j]
        {
            get { return sudokuGrid.Data[i, j]; }
            set
            {
                sudokuGrid.Data[i, j] = value;
                Notify(Binding.IndexerName);
            }
        }

        public static implicit operator int[,](SudokuGridViewModel sudokuGrid)
        {
            return sudokuGrid.sudokuGrid.Data;
        }
        #endregion

        #region Private Methods
        private void SplitIndex(string index, out int i, out int j)
        {
            var parts = index.Split('-');
            if (parts.Length != 2)
                throw new ArgumentException("The provided index is not valid");

            i = int.Parse(parts[0]);
            j = int.Parse(parts[1]);
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        private void Notify(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
