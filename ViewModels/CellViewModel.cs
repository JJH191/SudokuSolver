using Common;
using Models;
using System.ComponentModel;
using System.Windows.Media;

namespace ViewModels
{
    public class CellViewModel : INotifyPropertyChanged
    {
        private readonly CellModel cell;

        public int Number
        {
            get => cell.Number;
            set
            {
                cell.Number = value;
                Notify(nameof(Number));
            }
        }

        public Colour Colour
        {
            get => cell.IsValid ? (Colour)Colors.Transparent : new Colour(100, 255, 0, 0);
        }

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

        public void NotifyChange(bool number = true, bool colour = true)
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
