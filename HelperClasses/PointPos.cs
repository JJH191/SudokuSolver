using Accord;
using System;

namespace HelperClasses
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
        public static PointPos operator *(PointPos p1, PointPos p2) => new PointPos(p1.X * p2.X, p1.Y * p2.Y);

        // Length calcuations using pythagoras
        public double LengthSquared() => X * X + Y * Y;
        public double Length() => Math.Sqrt(LengthSquared());

        /// <summary>
        /// Calculates the angle with the given options
        /// By default, it measures the anticlockwise angle from the positive x-axis
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="direction"></param>
        /// <returns>The angle of the vector in radians</returns>
        public double Angle(Axis axis = Axis.X, Direction direction = Direction.ANTICLOCKWISE)
        {
            double x = X;
            double y = Y;

            if (axis == Axis.NEG_X) x *= -1;
            if (axis == Axis.NEG_Y) y *= -1;

            double angle = Math.Atan2(y, x);
            if (axis == Axis.Y || axis == Axis.NEG_Y) angle -= Math.PI / 2;
            if (direction == Direction.CLOCKWISE) angle *= -1;

            return NormaliseAngle(angle);
        }

        /// <summary>
        /// Convert the angle to being between 0 and 2pi
        /// </summary>
        /// <param name="angle">Angle to normalise in radians</param>
        /// <returns>The normalised angle</returns>
        private double NormaliseAngle(double angle)
        {
            double twoPi = Math.PI * 2;

            angle %= twoPi; // Make sure the magnitude of the angle is not greater than 2pi
            angle += twoPi; // Make sure the angle is between 0 and 4pi
            return angle % twoPi; // Make sure the angle is between 0 and 2pi
        }

        public double CrossProduct(PointPos other) => X * other.Y - Y * other.X;
        #endregion

        /// <summary>
        /// Value equality between this point and another
        /// </summary>
        /// <param name="other">The other point to check against</param>
        /// <returns>True if the values are equal, false if not</returns>
        public bool Equals(PointPos other) => X == other.X && Y == other.Y;

        public enum Axis { X, Y, NEG_X, NEG_Y }
        public enum Direction { CLOCKWISE, ANTICLOCKWISE }
    }
}
