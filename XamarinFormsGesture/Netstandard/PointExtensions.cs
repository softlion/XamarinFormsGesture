using System;
using Xamarin.Forms;

namespace Vapolia.Lib.Ui
{
    internal static class PointExtensions
    {
        public static Point Add(this Point pt1, Point pt2)
            => new Point(pt1.X + pt2.X, pt1.Y + pt2.Y);

        public static Point Substract(this Point pt1, Point pt2)
            => new Point(pt1.X - pt2.X, pt1.Y - pt2.Y);

        public static double AngleWithHorizontal(this (Point Point1, Point Point2) line)
        {
            var vector = line.Point2.Substract(line.Point1);
            return Math.Atan2(vector.Y, vector.X);
        }

        public static Point Divide(this Point pt, double value)
            => new Point(pt.X /value, pt.Y/value);

        internal static double Distance2(this Point pt1, Point pt2)
        {
            var dx = pt2.X - pt1.X;
            var dy = pt2.Y - pt1.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }
    }
}