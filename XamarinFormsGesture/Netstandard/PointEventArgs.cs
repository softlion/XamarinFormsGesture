using Xamarin.Forms;

namespace Vapolia.Lib.Ui;

public class PointEventArgs
{
    public Point Point { get; }
    public Element Element { get; }
    public object BindingContext { get; }

    public PointEventArgs(Point Point, Element Element, object BindingContext)
    {
        this.Point = Point;
        this.Element = Element;
        this.BindingContext = BindingContext;
    }
}
