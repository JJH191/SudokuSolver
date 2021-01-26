using Common;
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

        // Based on pseudocode from wikipedia: https://en.wikipedia.org/wiki/Gift_wrapping_algorithm
        // Using suggestion of cross-product for angles from tutorial at https://www.youtube.com/watch?v=YNyULRrydVI&t
        /// <summary>
        /// Gets the convex hull of the points in a clockwise order
        /// NOTE: If one point is inside the others, or the corners form an hourglass, the hull is invalid, so this will return null
        /// </summary>
        /// <returns>The corners of the quad in clockwise order or null if invalid</returns>
        public Vector2D[] GetClockwiseHull()
        {
            // Get the top left point that is definitely on the hull
            Vector2D current = GetTopLeftPointOnHull(points); // Keeps track of the current point on the hull
            Vector2D endpoint; // Keeps track of the current best point to add to the hull

            // Keeps track of the hull in a clockwise order
            List<Vector2D> clockwiseHull = new List<Vector2D>();

            do
            {
                clockwiseHull.Add(current); // Add the point to the hull
                endpoint = points[0]; // Start with first point in the unsorted points list

                // Loop through all the points and find the next one on the hull (this is the one that is the greatest 'left turn' from the current point on the hull)
                for (int i = 0; i < points.Length; i++)
                {
                    Vector2D currentToEndpoint = endpoint - current; // Vector from the current point on the hull to the current endpoint
                    Vector2D checkingToEndpoint = endpoint - points[i]; // Vector from the current point being checked to the current endpoint

                    // If the endpoint is the current point or the checking point is to the left of the line currentToEndpoint, that is the new endpoint
                    if (endpoint == current || checkingToEndpoint.CrossProduct(currentToEndpoint) < 0)
                        endpoint = points[i];
                }

                current = endpoint; // Set the current point to be the endpoint (next point on the hull)
            } while (endpoint != clockwiseHull[0]); // Break out once we reach the start point

            // Check that the hull is a valid quad and return it if it is
            if (!IsHullValid(clockwiseHull.ToArray())) return null;
            return clockwiseHull.ToArray();
        }

        private bool IsHullValid(Vector2D[] hull)
        {
            // If the number of points is 3, one point is inside the triangle formed by the other 3
            if (hull.Length != 4) return false;
            if (IsHourglassShape(hull)) return false;
            return true;
        }

        /// <summary>
        /// Check if the corners are arranged in an hourglass shape
        /// </summary>
        /// <param name="hull">The hull to check the original corners against</param>
        /// <returns>True if the corners form an hourglass, otherwise false</returns>
        private bool IsHourglassShape(Vector2D[] hull)
        {
            // If the points are valid, their indices will alternate between odd and even e.g. 0, 1, 2, 3 or 0, 3, 2, 1
            // However, if there is an odd index followed by another odd (or the same for even), this means that the quad is in an hourglass shape
            bool isEven = Array.IndexOf(points, hull[0]) % 2 == 0;
            for (int i = 1; i < hull.Length; i++)
            {
                bool isNextEven = Array.IndexOf(points, hull[i]) % 2 == 0;
                if (isNextEven == isEven) return true;
                isEven = isNextEven;
            }

            return false;
        }

        /// <summary>
        /// Gets the top left point. If this point is not on the hull (inside the other points), the left-most point will be used
        /// </summary>
        /// <param name="corners">The points to find the top left of</param>
        /// <returns>The top left point of the given points</returns>
        private Vector2D GetTopLeftPointOnHull(Vector2D[] corners)
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
            if (PointInTriangle(topLeft, pointsExcludingTopleft))
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

        private bool PointInTriangle(Vector2D point, List<Vector2D> trianglePoints)
        {
            if (trianglePoints.Count != 3) throw new ArgumentException("trianglePoints should contain 3 points");
            return PointInTriangle(point, trianglePoints[0], trianglePoints[1], trianglePoints[2]);
        }

        // Code from https://stackoverflow.com/questions/2049582/how-to-determine-if-a-point-is-in-a-2d-triangle
        private double Sign(Vector2D p1, Vector2D p2, Vector2D p3)
        {
            return (p1.X - p3.X) * (p2.Y - p3.Y) - (p2.X - p3.X) * (p1.Y - p3.Y);
        }

        // Code from https://stackoverflow.com/questions/2049582/how-to-determine-if-a-point-is-in-a-2d-triangle
        private bool PointInTriangle(Vector2D pt, Vector2D v1, Vector2D v2, Vector2D v3)
        {
            double d1, d2, d3;
            bool has_neg, has_pos;

            d1 = Sign(pt, v1, v2);
            d2 = Sign(pt, v2, v3);
            d3 = Sign(pt, v3, v1);

            has_neg = (d1 < 0) || (d2 < 0) || (d3 < 0);
            has_pos = (d1 > 0) || (d2 > 0) || (d3 > 0);

            return !(has_neg && has_pos);
        }
    }
}
