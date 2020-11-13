using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ViewModels;

namespace Models
{
    public class QuadModel
    {
        public QuadModel(PointPos[] points)
        {
            Points = points;
        }

        public PointPos this[int index]
        {
            get => Points[index];
            set => Points[index] = value;
        }

        public PointPos[] Points { get; } = new PointPos[4];
        public int Length { get => Points.Length; }
    }
}
