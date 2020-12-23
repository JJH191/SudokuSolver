using Accord;
using System;
using System.Drawing;

namespace ViewModels
{
    /// <summary>
    /// Simple wrapper that that supports simple maths and implicit conversion to System.Drawing.Point, System.Windows.Point and Accord.IntPoint
    /// </summary>
    public class PointPos : IEquatable<PointPos>
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
        public PointPos(System.Drawing.Point p)
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

        /// <summary>
        /// Create a PointPos with the same coordinates as <paramref name="p"/>
        /// </summary>
        /// <param name="p"></param>
        public PointPos(IntPoint p)
        {
            X = p.X;
            Y = p.Y;
        }
        #endregion

        #region Implicit Conversions
        // Implicit conversion from Point to PointPos
        public static implicit operator PointPos(System.Drawing.Point p) => new PointPos(p);
        public static implicit operator PointPos(System.Windows.Point p) => new PointPos(p);
        public static implicit operator PointPos(IntPoint p) => new PointPos(p);

        // Implicit conversion from PointPos to Point
        public static implicit operator System.Drawing.Point(PointPos p) => new System.Drawing.Point((int)p.X, (int)p.Y);
        public static implicit operator System.Windows.Point(PointPos p) => new System.Windows.Point((int)p.X, (int)p.Y);
        public static implicit operator IntPoint(PointPos p) => new IntPoint((int)p.X, (int)p.Y);
        #endregion

        #region Maths Operations
        // Addition between PointPos and Point
        public static PointPos operator +(PointPos p1, PointPos p2) => new PointPos(p1.X + p2.X, p1.Y + p2.Y);
        public static PointPos operator +(PointPos p1, System.Drawing.Point p2) => new PointPos(p1.X + p2.X, p1.Y + p2.Y);
        public static PointPos operator +(System.Drawing.Point p1, PointPos p2) => new PointPos(p1.X + p2.X, p1.Y + p2.Y);
        public static PointPos operator +(PointPos p1, System.Windows.Point p2) => new PointPos(p1.X + p2.X, p1.Y + p2.Y);
        public static PointPos operator +(System.Windows.Point p1, PointPos p2) => new PointPos(p1.X + p2.X, p1.Y + p2.Y);

        // Subtraction between Vector and Point
        public static PointPos operator -(PointPos p1, PointPos p2) => new PointPos(p1.X - p2.X, p1.Y - p2.Y);
        public static PointPos operator -(PointPos p1, System.Drawing.Point p2) => new PointPos(p1.X - p2.X, p1.Y - p2.Y);
        public static PointPos operator -(System.Drawing.Point p1, PointPos p2) => new PointPos(p1.X - p2.X, p1.Y - p2.Y);
        public static PointPos operator -(PointPos p1, System.Windows.Point p2) => new PointPos(p1.X - p2.X, p1.Y - p2.Y);
        public static PointPos operator -(System.Windows.Point p1, PointPos p2) => new PointPos(p1.X - p2.X, p1.Y - p2.Y);

        public static PointPos operator *(PointPos p, double value) => new PointPos(p.X * value, p.Y * value);

        // Length calcuations using pythagoras
        public double LengthSquared() => X * X + Y * Y;
        public double Length() => Math.Sqrt(LengthSquared());

        // NOT MY CODE
        public double CrossProduct(PointPos other) => X * other.Y - Y * other.X;
        #endregion

        public bool Equals(PointPos other)
        {
            return (this.X == other.X && this.Y == other.Y);
        }
    }
}
