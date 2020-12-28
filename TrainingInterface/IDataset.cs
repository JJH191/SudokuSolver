namespace TrainingInterface
{
    public interface IDataset
    {
        InputData[] GetData();
        void Shuffle();
    }
}
