using System;
using Xamarin.Forms;

namespace Vapolia.Lib.Ui;

    public class PinchEventArgs
    {
        public GestureStatus Status { get; }
        public (Point Point1, Point Point2) CurrentPoints { get; }
        public (Point Point1, Point Point2) StartingPoints { get; }
        public Point Center { get; }
        public double Scale { get; }
        public double RotationRadians { get; }
        public double RotationDegrees => RotationRadians * 180 / Math.PI;


        public PinchEventArgs(GestureStatus status, (Point Point1, Point Point2) currentPoints, (Point Point1,Point Point2) startingPoints)
        {
            Status = status;
            CurrentPoints = currentPoints;
            StartingPoints = startingPoints;
            
            Center = startingPoints.Point1.Add(startingPoints.Point2).Divide(2);

            var initialDistance = startingPoints.Point1.Distance2(startingPoints.Point2);
            var currentDistance = currentPoints.Point1.Distance2(currentPoints.Point2);
            Scale = initialDistance > double.Epsilon ? currentDistance/initialDistance : 1;

            RotationRadians = currentPoints.AngleWithHorizontal() - startingPoints.AngleWithHorizontal();
        }
    }

