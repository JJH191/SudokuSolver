namespace Models
{
    public delegate void CellNumberModified();
    public class CellModel
    {
        public event CellNumberModified CellNumberModifiedEvent;

        private int number;
        public int Number { 
            get => number; 
            set
            {
                if (number != value)
                {
                    number = value;
                    CellNumberModifiedEvent?.Invoke();
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
