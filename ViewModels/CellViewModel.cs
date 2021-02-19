using Common;
using Models;
using System.ComponentModel;
using System.Windows.Media;

namespace ViewModels
{
    /// <summary>
    /// View model to provide the data for each cell in the sudoku grid
    /// </summary>
    public class CellViewModel : INotifyPropertyChanged
    {
        private readonly CellModel cell;

        // Provides the number that the cell contains
        public int Number
        {
            get => cell.Number;
            set
            {
                cell.Number = value;
                Notify(nameof(Number));
            }
        }

        // Provides the colour for the cell (transparent if the cell is valid, or red if it's not)
        public Colour Colour
        {
            get => cell.IsValid ? (Colour)Colors.Transparent : new Colour(100, 255, 0, 0);
        }
        /// <summary>
        /// Sets the state of the cell to valid or invalid depending on whether it is correct
        /// </summary>
        /// <param name="isValid">Whether the cell is valid or not</param>
        public void SetIsValid(bool isValid)
        {
            cell.IsValid = isValid;
            Notify(nameof(Colour));
        }

        public CellViewModel(CellModel cell)
        {
            this.cell = cell;
        }

        public CellModel GetModel() => cell;

        /// <summary>
        /// Update the UI of the cell
        /// </summary>
        /// <param name="number">Whether the number should be updated</param>
        /// <param name="colour">Whether the colour should be updated</param>
        public void NotifyChange(bool number = false, bool colour = false)
        {
            if (number) Notify(nameof(Number));
            if (colour) Notify(nameof(Colour));
        }

        // Notifier for when a property changes
        public event PropertyChangedEventHandler PropertyChanged;
        private void Notify(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
