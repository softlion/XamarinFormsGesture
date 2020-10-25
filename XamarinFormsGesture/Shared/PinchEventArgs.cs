using Xamarin.Forms;

namespace Vapolia.Lib.Ui
{
    public class PinchEventArgs
    {
        public GestureStatus Status { get; }
        (Point Point1, Point Point2) CurrentPoint { get; }
        (Point Point1, Point Point2) StartingPoint { get; }
        public Point Center { get; }
        public double Scale { get; }
        /// <summary>
        /// Angle in radians
        /// </summary>
        public double Rotation { get; }

        public PinchEventArgs(GestureStatus status, (Point Point1, Point Point2) currentPoint, (Point Point1,Point Point2) startingPoint)
        {
            Status = status;
            CurrentPoint = currentPoint;
            StartingPoint = startingPoint;
            
            Center = startingPoint.Point1.Add(startingPoint.Point2).Divide(2);

            var initialDistance = startingPoint.Point1.Distance2(startingPoint.Point2);
            var currentDistance = currentPoint.Point1.Distance2(currentPoint.Point2);
            Scale = currentDistance/initialDistance;

            Rotation = currentPoint.AngleWithHorizontal() - startingPoint.AngleWithHorizontal();
        }
    }
}
