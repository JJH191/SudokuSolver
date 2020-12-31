namespace TrainingInterface
{
    /// <summary>
    /// Interface providing a method to get the data from a dataset or shuffle it
    /// </summary>
    public interface IDataset
    {
        InputData[] GetData();
        void Shuffle();
    }
}
