using HelperClasses;
using Models;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Data;

namespace ViewModels
{
    public class QuadViewModel : INotifyPropertyChanged
    {
        private readonly QuadModel quad;

        public QuadViewModel(PointPos[] quad)
        {
            //quad = new QuadModel(new PointPos[] {
            //    new PointPos(100, 100),
            //    new PointPos(100, 200),
            //    new PointPos(200, 200),
            //    new PointPos(200, 100),
            //});
            this.quad = new QuadModel(quad);
        }

        public Point this[int index]
        {
            get
            {
                return quad[index];
            }
            set
            {
                quad[index] = value;
                Notify(Binding.IndexerName);
            }
        }
        public int Length => 4;

        public QuadModel GetModel() => quad;

        public event PropertyChangedEventHandler PropertyChanged;
        private void Notify(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
