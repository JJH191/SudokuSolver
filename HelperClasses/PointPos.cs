using System;
using System.Drawing;

namespace ViewModels
{
    /// <summary>
    /// Simple wrapper that that supports simple maths and implicit conversion to System.Drawing.Point and System.Windows.Point 
    /// </summary>
    public class PointPos
    {
        public double X { get; set; }
        public double Y { get; set; }

        #region Constructors
        /// <summary>
        /// Create a PointPos with the coordinates (<paramref name="x"/>, <paramref name="y"/>)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public PointPos(double x, double y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Create a PointPos with the same coordinates as <paramref name="p"/>
        /// </summary>
        /// <param name="p"></param>
        public PointPos(Point p)
        {
            X = p.X;
            Y = p.Y;
        }

        /// <summary>
        /// Create a PointPos with the same coordinates as <paramref name="p"/>
        /// </summary>
        /// <param name="p"></param>
        public PointPos(System.Windows.Point p)
        {
            X = p.X;
            Y = p.Y;
        }
        #endregion

        #region Implicit Conversions
        // Implicit conversion from Point to PointPos
        public static implicit operator PointPos(Point p) => new PointPos(p);
        public static implicit operator PointPos(System.Windows.Point p) => new PointPos(p);

        // Implicit conversion from PointPos to Point
        public static implicit operator Point(PointPos p) => new Point((int)p.X, (int)p.Y);
        public static implicit operator System.Windows.Point(PointPos p) => new System.Windows.Point((int)p.X, (int)p.Y);
        #endregion

        #region Maths Operations
        // Addition between PointPos and Point
        public static PointPos operator +(PointPos p1, PointPos p2) => new PointPos(p1.X + p2.X, p1.Y + p2.Y);
        public static PointPos operator +(PointPos p1, Point p2) => new PointPos(p1.X + p2.X, p1.Y + p2.Y);
        public static PointPos operator +(Point p1, PointPos p2) => new PointPos(p1.X + p2.X, p1.Y + p2.Y);
        public static PointPos operator +(PointPos p1, System.Windows.Point p2) => new PointPos(p1.X + p2.X, p1.Y + p2.Y);
        public static PointPos operator +(System.Windows.Point p1, PointPos p2) => new PointPos(p1.X + p2.X, p1.Y + p2.Y);

        // Subtraction between Vector and Point
        public static PointPos operator -(PointPos p1, PointPos p2) => new PointPos(p1.X - p2.X, p1.Y - p2.Y);
        public static PointPos operator -(PointPos p1, Point p2) => new PointPos(p1.X - p2.X, p1.Y - p2.Y);
        public static PointPos operator -(Point p1, PointPos p2) => new PointPos(p1.X - p2.X, p1.Y - p2.Y);
        public static PointPos operator -(PointPos p1, System.Windows.Point p2) => new PointPos(p1.X - p2.X, p1.Y - p2.Y);
        public static PointPos operator -(System.Windows.Point p1, PointPos p2) => new PointPos(p1.X - p2.X, p1.Y - p2.Y);

        // Length calcuations using pythagoras
        public double LengthSquared() => X * X + Y * Y;
        public double Length() => Math.Sqrt(LengthSquared());
        #endregion
    }
}
