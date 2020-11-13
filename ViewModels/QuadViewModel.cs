using Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace ViewModels
{
    public class QuadViewModel : INotifyPropertyChanged
    {
        private readonly QuadModel quad;

        public QuadViewModel()
        {
            quad = new QuadModel(new PointPos[] { 
                new PointPos(100, 100), 
                new PointPos(100, 200), 
                new PointPos(200, 200), 
                new PointPos(200, 100), 
            });

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

        public event PropertyChangedEventHandler PropertyChanged;
        private void Notify(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
