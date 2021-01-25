namespace Models
{
    public delegate void CellNumberModified(int oldValue, int newValue);
    public class CellModel
    {
        public event CellNumberModified CellNumberModifiedEvent;

        private int number;
        public int Number
        {
            get => number;
            set
            {
                if (number != value)
                {
                    CellNumberModifiedEvent?.Invoke(number, value);
                    number = value;
                }
            }
        }
        public bool IsValid { get; set; }

        public CellModel(int number)
        {
            Number = number;
            IsValid = true;
        }
    }
}
