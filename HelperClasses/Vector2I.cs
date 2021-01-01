﻿using Accord;
using System;

namespace HelperClasses
{
    /// <summary>
    /// Simple wrapper that that supports simple maths and implicit conversion to System.Drawing.Point, System.Windows.Point and Accord.IntPoint
    /// </summary>
    public class Vector2I : IEquatable<Vector2I>
    {
        public int X { get; set; }
        public int Y { get; set; }

        #region Constructors
        /// <summary>
        /// Create a Vector with the coordinates (<paramref name="x"/>, <paramref name="y"/>)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Vector2I(int x, int y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Create a Vector with the same coordinates as <paramref name="p"/>
        /// </summary>
        /// <param name="p"></param>
        public Vector2I(System.Drawing.Point p)
        {
            X = p.X;
            Y = p.Y;
        }

        /// <summary>
        /// Create a Vector with the same coordinates as <paramref name="p"/>
        /// </summary>
        /// <param name="p"></param>
        public Vector2I(System.Windows.Point p)
        {
            X = (int)p.X;
            Y = (int)p.Y;
        }

        /// <summary>
        /// Create a Vector with the same coordinates as <paramref name="p"/>
        /// </summary>
        /// <param name="p"></param>
        public Vector2I(IntPoint p)
        {
            X = p.X;
            Y = p.Y;
        }
        #endregion

        #region Implicit Conversions
        // Implicit conversion from Point to Vector
        public static implicit operator Vector2I(System.Drawing.Point p) => new Vector2I(p);
        public static implicit operator Vector2I(System.Windows.Point p) => new Vector2I(p);
        public static implicit operator Vector2I(IntPoint p) => new Vector2I(p);

        // Implicit conversion from Vector to Point
        public static implicit operator System.Drawing.Point(Vector2I p) => new System.Drawing.Point((int)p.X, (int)p.Y);
        public static implicit operator System.Windows.Point(Vector2I p) => new System.Windows.Point((int)p.X, (int)p.Y);
        public static implicit operator IntPoint(Vector2I p) => new IntPoint((int)p.X, (int)p.Y);
        #endregion

        #region Maths Operations
        // Addition between Vector and Point
        public static Vector2I operator +(Vector2I p1, Vector2I p2) => new Vector2I(p1.X + p2.X, p1.Y + p2.Y);
        public static Vector2I operator +(Vector2I p1, System.Drawing.Point p2) => new Vector2I(p1.X + p2.X, p1.Y + p2.Y);
        public static Vector2I operator +(System.Drawing.Point p1, Vector2I p2) => new Vector2I(p1.X + p2.X, p1.Y + p2.Y);
        public static Vector2I operator +(Vector2I p1, System.Windows.Point p2) => new Vector2I((int)(p1.X + p2.X), (int)(p1.Y + p2.Y));
        public static Vector2I operator +(System.Windows.Point p1, Vector2I p2) => new Vector2I((int)(p1.X + p2.X), (int)(p1.Y + p2.Y));

        // Subtraction between Vector and Point
        public static Vector2I operator -(Vector2I p1, Vector2I p2) => new Vector2I(p1.X - p2.X, p1.Y - p2.Y);
        public static Vector2I operator -(Vector2I p1, System.Drawing.Point p2) => new Vector2I(p1.X - p2.X, p1.Y - p2.Y);
        public static Vector2I operator -(System.Drawing.Point p1, Vector2I p2) => new Vector2I(p1.X - p2.X, p1.Y - p2.Y);
        public static Vector2I operator -(Vector2I p1, System.Windows.Point p2) => new Vector2I((int)(p1.X - p2.X), (int)(p1.Y - p2.Y));
        public static Vector2I operator -(System.Windows.Point p1, Vector2I p2) => new Vector2I((int)(p1.X - p2.X), (int)(p1.Y - p2.Y));

        public static Vector2I operator *(Vector2I p, int value) => new Vector2I(p.X * value, p.Y * value);
        public static Vector2I operator *(Vector2I p1, Vector2I p2) => new Vector2I(p1.X * p2.X, p1.Y * p2.Y);

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
            // Get the x and y values
            double x = X;
            double y = Y;

            // If the axis is negative, multiply that component of the vector by -1 (to invert it)
            if (axis == Axis.NEG_X) x *= -1;
            if (axis == Axis.NEG_Y) y *= -1;

            // Get the angle
            double angle = Math.Atan2(y, x);

            // If it is the Y axis, subtract 0.5pi
            if (axis == Axis.Y || axis == Axis.NEG_Y) angle -= Math.PI / 2;
            if (direction == Direction.CLOCKWISE) angle *= -1; // If the angle needs to be clockwise, multiply it by -1 to flip it

            // Normalise the angle between 0 and 2pi
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

        /// <summary>
        /// Get the scalar cross product of this vector and <paramref name="other"/>
        /// </summary>
        /// <param name="other">The vector to cross product with</param>
        /// <returns></returns>
        public double CrossProduct(Vector2I other) => X * other.Y - Y * other.X;
        #endregion

        /// <summary>
        /// Value equality between this point and another
        /// </summary>
        /// <param name="other">The other point to check against</param>
        /// <returns>True if the values are equal, false if not</returns>
        public bool Equals(Vector2I other) => X == other.X && Y == other.Y;

        // Options for getting the angle
        public enum Axis { X, Y, NEG_X, NEG_Y }
        public enum Direction { CLOCKWISE, ANTICLOCKWISE }
    }
}
