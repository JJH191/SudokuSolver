using HelperClasses;
using System;
using System.Collections.Generic;

namespace Models
{
    /// <summary>
    /// A model to keep track of the corners of the sudoku
    /// </summary>
    public class QuadModel
    {
        private readonly Vector2D[] points = new Vector2D[4];

        /// <summary>
        /// Creates a new quad model with the points specified
        /// </summary>
        /// <param name="points">The corners of the quad to start with</param>
        public QuadModel(Vector2D[] points)
        {
            if (points.Length != 4) throw new ArgumentException("There should be 4 points provided");
            this.points = points;
        }

        /// <summary>
        /// Get a specific point of the quad at the given <paramref name="index"/>
        /// </summary>
        /// <param name="index">The index of the point to get</param>
        /// <returns>The point at <paramref name="index"/> from the quad</returns>
        public Vector2D this[int index]
        {
            get => points[index]; // Get the point
            set => points[index] = value; // Set the point
        }

        // TODO: optimise by using cross product instead of angle
        /// <summary>
        /// Gets the convex hull of the points in a clockwise order
        /// NOTE: If one point is inside the others, it will not be included in the hull, so the hull will only contain 3 points
        /// </summary>
        /// <returns>The corners of the quad in clockwise order</returns>
        public Vector2D[] GetClockwiseHull()
        {
            // This algorithm works by starting with the top left point
            // With this top left point, it will loop through all the other points and find the relative vector to them
            // It will then calculate the angle between this relative vector and the y-axis
            // The smallest angle will be the next point in the hull, going clockwise
            // Once the top left point is reached again, the hull is complete

            List<Vector2D> cornersLeft = new List<Vector2D>(points); // Keeps track of the corners left to add to the hull

            // Get the top left point and start with that
            Vector2D topLeft = GetTopLeftPointOnHull(cornersLeft);
            Vector2D current = topLeft;
            cornersLeft.Remove(current);

            // The hull of the quad's points in a clockwise order, starting with the top left
            List<Vector2D> clockwiseHull = new List<Vector2D> { current };

            // Loop until no corners left to add
            while (cornersLeft.Count > 0)
            {
                // Start with smallest angle as 2pi (the maximum  possible angle) and no smallest angle corner
                Vector2D smallestAngleCorner = null;
                double smallestAngle = Math.PI * 2;

                // Loop through the remaining corners and find the one with the smallest angle between it and the current corner
                foreach (Vector2D corner in cornersLeft)
                {
                    // Get the angle, clockwise, from negative y-axis (negative y is used here because angle expects up to be positive, but images take down as positive)
                    double angle = (corner - current).Angle(axis: Vector2D.Axis.NEG_Y, direction: Vector2D.Direction.CLOCKWISE);
                    if (angle < smallestAngle)
                    {
                        smallestAngle = angle;
                        smallestAngleCorner = corner;
                    }
                }

                // If we have reached the start again, the hull is complete, so exit
                // Since we start with the top left, we do not want to check top left with itself
                if (current != topLeft)
                {
                    double angleToStart = (topLeft - current).Angle(axis: Vector2D.Axis.NEG_Y, direction: Vector2D.Direction.CLOCKWISE);
                    if (angleToStart < smallestAngle) break;
                }

                clockwiseHull.Add(smallestAngleCorner); // Add the new point to the hull
                cornersLeft.Remove(smallestAngleCorner); // Remove the point from cornersLeft so it isn't checked again
                current = smallestAngleCorner; // Change the current point to continue the hull from there
            }

            return clockwiseHull.ToArray();
        }

        /// <summary>
        /// Gets the top left point. If this point is not on the hull (inside the other points), the left-most point will be used
        /// </summary>
        /// <param name="corners">The points to find the top left of</param>
        /// <returns>The top left point of the given points</returns>
        private Vector2D GetTopLeftPointOnHull(List<Vector2D> corners)
        {
            // Start with null top left point and largest possible distance
            Vector2D topLeft = null;
            double shortestDistSquared = double.MaxValue;

            // Loop through all points to find the one with the shortest distance to (0, 0)
            foreach (Vector2D p in corners)
            {
                // Using length squared as it is quicker to calculate than length (does not require square root)
                double lengthSquared = p.LengthSquared();
                if (lengthSquared < shortestDistSquared)
                {
                    topLeft = p;
                    shortestDistSquared = lengthSquared;
                }
            }

            // Get all the points other than the top left
            List<Vector2D> pointsExcludingTopleft = new List<Vector2D>(corners);
            pointsExcludingTopleft.Remove(topLeft);

            // If the top left point is inside the others (and therefore inside the hull), take the left-most point to ensure the point is on the hull
            // This is necessary for GetClockwiseHull to work correctly
            if (PointInTriangle2(topLeft, pointsExcludingTopleft))
            {
                // Loop through and find the left-most point
                foreach (Vector2D p in corners)
                {
                    if (p.X < topLeft.X)
                        topLeft = p;
                }
            }

            return topLeft;
        }

        private bool PointInTriangle2(Vector2D point, List<Vector2D> trianglePoints)
        {
            if (trianglePoints.Count != 3) throw new ArgumentException("trianglePoints should contain 3 points");
            return PointInTriangle(point, trianglePoints[0], trianglePoints[1], trianglePoints[2]);
        }

        // NOT MY CODE
        private double AreaOfTriangle(Vector2D p1, Vector2D p2, Vector2D p3)
        {
            return (p1.X - p3.X) * (p2.Y - p3.Y) - (p2.X - p3.X) * (p1.Y - p3.Y);
        }

        // NOT MY CODE
        private bool PointInTriangle(Vector2D pt, Vector2D v1, Vector2D v2, Vector2D v3)
        {
            double d1, d2, d3;
            bool has_neg, has_pos;

            d1 = AreaOfTriangle(pt, v1, v2);
            d2 = AreaOfTriangle(pt, v2, v3);
            d3 = AreaOfTriangle(pt, v3, v1);

            has_neg = (d1 < 0) || (d2 < 0) || (d3 < 0);
            has_pos = (d1 > 0) || (d2 > 0) || (d3 > 0);

            return !(has_neg && has_pos);
        }
    }
}
