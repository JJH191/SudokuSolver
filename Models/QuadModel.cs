using HelperClasses;
using System.Collections.Generic;
using System.Linq;

namespace Models
{
    public class QuadModel
    {
        private PointPos[] points = new PointPos[4];
        public int Length { get => points.Length; }

        public QuadModel(PointPos[] points)
        {
            this.points = points;
        }

        public PointPos this[int index]
        {
            get => points[index];
            set => points[index] = value;
        }

        /// <summary>
        /// Checks if a corner of the quad is valid (it is invalid if the point is inside the three other corners)
        /// </summary>
        /// <param name="index">The index of the corner to check</param>
        /// <returns>True if the point is valid and false if it is not</returns>
        public bool IsCornerValid(int index)
        {
            // TODO: implement code
            return true;
        }

        public PointPos[] GetClockwisePoints()
        {
            return MakeClockwise(points);
        }

        private PointPos[] MakeClockwise(PointPos[] points)
        {
            var corners = points.ToList();
            corners.Sort((corner1, corner2) => corner1.LengthSquared().CompareTo(corner2.LengthSquared()));

            PointPos tl = corners.First();
            PointPos currentVertex;

            // If top left point is not on the hull, take the left most point to be the top left
            if (PointInTriangle(tl, corners[1], corners[2], corners[3]))
            {
                corners.Sort((corner1, corner2) => corner1.X.CompareTo(corner2.X));
                tl = corners.First();
            }

            currentVertex = tl;
            List<PointPos> result = new List<PointPos> { currentVertex };

            int index = 2;
            int nextIndex = -1;
            PointPos nextVertex = corners[1];

            while (true)
            {
                PointPos checking = corners[index];
                PointPos a = nextVertex - currentVertex;
                PointPos b = checking - currentVertex;
                double cross = a.CrossProduct(b);

                if (cross < 0)
                {
                    nextVertex = checking;
                    nextIndex = index;
                }

                index++;
                if (index == corners.Count)
                {
                    if (nextVertex == tl)
                    {
                        break;
                    }
                    else
                    {
                        result.Add(nextVertex);
                        currentVertex = nextVertex;
                        index = 0;
                        nextVertex = tl;
                    }
                }
            }

            return result.ToArray();
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
