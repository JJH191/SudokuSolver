namespace Models
{
    // Delegate method to get called every time a cell changes
    public delegate void CellNumberModified(int oldValue, int newValue);

    /// <summary>
    /// A model to keep track of the contents of a cell
    /// </summary>
    public class CellModel
    {
        public event CellNumberModified CellNumberModifiedEvent; // Event to call all the delegate methods when a cell changes

        // The number a given cell
        private int number;
        public int Number
        {
            get => number;
            set
            {
                // If the number has changed, send the number changed event
                if (number != value)
                {
                    CellNumberModifiedEvent?.Invoke(number, value);
                    number = value;
                }
            }
        }

        // Keeps track of whether the given cell is valid in the sudoku
        public bool IsValid { get; set; }

        public CellModel(int number)
        {
            Number = number;
            IsValid = true;
        }
    }
}
