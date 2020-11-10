using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Models
{
    public class QuadModel
    {
        private readonly Point[] points = new Point[4];

        public Point this[int index]
        {
            get => points[index];
            set => points[index] = value;
        }

        public Point[] Points { get => points; }
        public int Length { get => points.Length; }
    }
}
