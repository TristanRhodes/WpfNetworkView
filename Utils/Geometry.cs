using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Utils
{
    public static class Geometry
    {
        public static Point GetRelativeCentre(double x, double y, double width, double height)
        {
            return new Point(x + (width / 2), y + (height / 2));
        }

        public static Point GetPointAtDistanceOnLine(double x1, double y1, double x2, double y2, double d)
        {
            //Guard catch - maths returns int.Min for X and Y when points are equal.
            if (x1 == x2 && y1 == y2)
                return new Point(x1, y1);

            //See Here: http://math.stackexchange.com/questions/25286/2d-coordinates-of-a-point-along-a-line-based-on-d-and-m-where-am-i-messing
            //And Here: http://stackoverflow.com/questions/12550365/calculate-a-point-along-the-line-a-b-at-a-given-distance-from-a

            // a. calculate the vector from o to g:
            double vectorX = x2 - x1;
            double vectorY = y2 - y1;

            // b. calculate the length:
            double magnitude = Math.Sqrt(vectorX * vectorX + vectorY * vectorY);

            // c. normalize the vector to unit length:
            vectorX /= magnitude;
            vectorY /= magnitude;

            // d. calculate and Draw the new vector, which is x1y1 + vxvy * (mag + distance).
            var point = new Point(
                (int)(((double)vectorX * (magnitude + d)) + 0.5), // x = col
                (int)(((double)vectorY * (magnitude + d)) + 0.5) // y = row
            );

            return point;
        }

        public static double GetLengthOfLine(double x1, double y1, double x2, double y2)
        {
            var x = x2 - x1;
            var y = y2 - y1;

            return Math.Sqrt(x * x + y * y);
        }

        public static double GetAngleBetweenTwoPoints(double x1, double y1, double x2, double y2)
        {
            var dY = y2 - y1;
            var dX = x2 - x1;

            return Math.Atan2(dY, dX);// *180d / Math.PI;
        }

        public static Point? GetIntersectionOfTwoLines(
            double x1, double y1,
            double x2, double y2,
            double x3, double y3,
            double x4, double y4)
        {
            //Determine Line 1
            var a1 = y2 - y1;
            var b1 = x1 - x2;
            var c1 = a1 * x1 + b1 * y1;

            //Determine Line 2
            var a2 = y4 - y3;
            var b2 = x3 - x4;
            var c2 = a2 * x3 + b2 * y3;

            //Get Delta
            double delta = a1 * b2 - a2 * b1;

            //If 0 - we are parallel. Return nothing.
            if (delta == 0)
                return null;

            //Calculate Positions
            var x = (b2 * c1 - b1 * c2) / delta;
            var y = (a1 * c2 - a2 * c1) / delta;

            //Round positions (Trailing very small number causing hit test issues on horizontal lines)
            x = Math.Round(x, 4);
            y = Math.Round(y, 4);

            //Return calculated point.
            return new Point(x, y);
        }

        public static bool IsIntersectionOnLine(Point lineFrom, Point lineTo, Point intersection)
        {
            bool insideXBound = false;
            bool insideYBound = false;

            //Inside X Bounds Check
            if (lineFrom.X < lineTo.X)
            {
                insideXBound =
                    lineFrom.X <= intersection.X &&
                    intersection.X <= lineTo.X;
            }
            else
            {
                insideXBound = 
                    lineTo.X <= intersection.X &&
                    intersection.X <= lineFrom.X;
            }

            //Inside Y Bounds Check
            if (lineFrom.Y < lineTo.Y)
            {
                insideYBound =
                    lineFrom.Y <= intersection.Y &&
                    intersection.Y <= lineTo.Y;
            }
            else
            {
                insideYBound =
                    lineTo.Y <= intersection.Y &&
                    intersection.Y <= lineFrom.Y;
            }

            //Return Result
            return insideXBound & insideYBound;
        }

        public static Point GetEllipseIntersections(double width, double height, double x, double y)
        {
            //Get Major and Minor Axess
            var a = width / 2;
            var b = height / 2;

            //Get Intersection
            var ix = ((a * b) / Math.Sqrt(((a * a) * (y * y)) + ((b * b) * (x * x)))) * x;
            var iy = ((a * b) / Math.Sqrt(((a * a) * (y * y)) + ((b * b) * (x * x)))) * y;


            return new Point(-ix, -iy);
        }
    }
}
