using Xamarin.Forms;

namespace Vapolia.Lib.Ui
{
    public class PanEventArgs
    {
        public GestureStatus Status { get; }
        public Point Point { get; }
        public bool CancelGesture { get; set; }

        public PanEventArgs(GestureStatus status, Point point)
        {
            Status = status;
            Point = point;
        }
    }
}