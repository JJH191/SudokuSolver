using HelperClasses;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Models
{
    /// <summary>
    /// A model to keep track of the corners of the sudoku
    /// </summary>
    public class QuadModel
    {
        private readonly PointPos[] points = new PointPos[4];

        /// <summary>
        /// Creates a new quad model with the points specified
        /// </summary>
        /// <param name="points">The corners of the quad to start with</param>
        public QuadModel(PointPos[] points)
        {
            this.points = points;
        }

        /// <summary>
        /// Get a specific point of the quad at the given <paramref name="index"/>
        /// </summary>
        /// <param name="index">The index of the point to get</param>
        /// <returns></returns>
        public PointPos this[int index]
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
        public PointPos[] GetClockwiseHull()
        {
            // This algorithm works by starting with the top left point
            // With this top left point, it will loop through all the other points and find the relative vector to them
            // It will then calculate the angle between this relative vector and the y-axis
            // The smallest angle will be the next point in the hull, going clockwise
            // Once the top left point is reached again, the hull is complete

            List<PointPos> cornersLeft = new List<PointPos>(points); // Keeps track of the corners left to add to the hull

            // Get the top left point and start with that
            PointPos topLeft = GetTopLeftPointOnHull(cornersLeft);
            PointPos current = topLeft; 
            //cornersLeft.Remove(current);

            // The hull of the quad's points in a clockwise order, starting with the top left
            List<PointPos> clockwiseHull = new List<PointPos> { current };

            // Loop until no corners left to add
            while (cornersLeft.Count > 0)
            {
                // Set the smallest angle to the angle between the top left and first corner in cornersLeft
                PointPos smallestAnglePos = cornersLeft[0];

                // Get the angle, clockwise, from negative y-axis (negative y is used here because angle expects up to be positive, but images take down as positive)
                double smallestAngle = (smallestAnglePos - current).Angle(axis: PointPos.Axis.NEG_Y, direction: PointPos.Direction.CLOCKWISE); 

                // Loop through the remaining corners and find the one with the smallest angle between it and the current corner
                foreach (PointPos corner in cornersLeft)
                {
                    double angle = (corner - current).Angle(axis: PointPos.Axis.NEG_Y, direction: PointPos.Direction.CLOCKWISE);
                    if (angle < smallestAngle)
                    {
                        smallestAngle = angle;
                        smallestAnglePos = corner;
                    }
                }

                // If we have reached the start again, the hull is complete, so exit
                if (smallestAnglePos == topLeft) break;

                clockwiseHull.Add(smallestAnglePos); // Add the new point to the hull
                cornersLeft.Remove(smallestAnglePos); // Remove the point from cornersLeft so it isn't checked again
                current = smallestAnglePos; // Change the current point to continue the hull from there
            }

            return clockwiseHull.ToArray();
        }

        /// <summary>
        /// Gets the top left point. If this point is not on the hull (inside the other points), the left-most point will be used
        /// </summary>
        /// <param name="corners">The points to find the top left of</param>
        /// <returns>The top left point of the given points</returns>
        private PointPos GetTopLeftPointOnHull(List<PointPos> corners)
        {
            PointPos topLeft = corners[0];

            foreach (PointPos p in corners)
            {
                if (p.LengthSquared() < topLeft.LengthSquared())
                    topLeft = p;
            }

            List<PointPos> pointsExcludingTopleft = new List<PointPos>(corners);
            pointsExcludingTopleft.Remove(topLeft);

            // If the top left point is inside the others (and therefore inside the hull), take the left-most point to ensure the point is on the hull
            if (PointInTriangle2(topLeft, pointsExcludingTopleft))
            {
                foreach (PointPos p in corners)
                {
                    if (p.X < topLeft.X)
                        topLeft = p;
                }
            }

            return topLeft;
        }

        private bool PointInTriangle2(PointPos point, List<PointPos> trianglePoints)
        {
            if (trianglePoints.Count != 3) throw new ArgumentException("trianglePoints should contain 3 points");
            return PointInTriangle(point, trianglePoints[0], trianglePoints[1], trianglePoints[2]);
        }

        // NOT MY CODE
        private double AreaOfTriangle(PointPos p1, PointPos p2, PointPos p3)
        {
            return (p1.X - p3.X) * (p2.Y - p3.Y) - (p2.X - p3.X) * (p1.Y - p3.Y);
        }

        // NOT MY CODE
        private bool PointInTriangle(PointPos pt, PointPos v1, PointPos v2, PointPos v3)
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
