using System.Drawing;
using Vapolia.Lib.Ui;
using Xamarin.Forms;

#if IOS
using Xamarin.Forms.Platform.iOS;
#endif
#if ANDROID
using Xamarin.Forms.Platform.Android;
#endif

namespace Vapolia.Lib.Effects;

public static class ElementExtensions
{
    public static RectangleF GetAbsoluteBounds(this PointEventArgs args)
        => (args.Element as VisualElement).GetAbsoluteBounds();
    
    public static PointF GetCoordinates(this PointEventArgs args)
        => (args.Element as VisualElement).GetCoordinates();
    
    public static RectangleF GetAbsoluteBounds(this VisualElement? view)
    {
        if(view == null)
            return RectangleF.Empty;
            
        var location = GetCoordinates(view);
        return new(location.X, location.Y, (float)view.Width, (float)view.Height);
    }

    public static System.Drawing.PointF GetCoordinates(this VisualElement element)
    {
#if ANDROID
        var nativeView = element.GetRenderer()?.View;
        if (nativeView == null)
            return System.Drawing.PointF.Empty;
        var density = nativeView.Context!.Resources!.DisplayMetrics!.Density;

        var locationWindow = new int[2];
        nativeView.GetLocationInWindow(locationWindow);
        var locationOfRootWindow = new int[2];
        nativeView.RootView?.FindViewById(Android.Resource.Id.Content)?.GetLocationInWindow(locationOfRootWindow);
            
        return new (locationWindow[0] / density, (locationWindow[1]-locationOfRootWindow[1]) / density);
#elif IOS
        var nativeView = element.GetRenderer()?.NativeView;
        if (nativeView == null)
            return System.Drawing.PointF.Empty;
        var rect = nativeView.Superview.ConvertPointToView(nativeView.Frame.Location, null);
        return new ((float)rect.X, (float)rect.Y);
#elif WINDOWS
        return new(0,0);
#else
        throw new NotSupportedException("Not supported on this platform");
#endif
    }
}