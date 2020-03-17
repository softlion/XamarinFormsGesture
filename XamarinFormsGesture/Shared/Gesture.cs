using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace Vapolia.Lib.Ui
{
    /// <summary>
    /// Attached properties for gesture
    /// </summary>
    /// <example>
    ///   &lt;Grid Gesture.TapCommand="{Binding GridTappedCommand}"&gt;
    ///      ...
    ///   &lt;/Grid&gt;
    /// </example>
    [Preserve (Conditional=true, AllMembers = true)]
    public static class Gesture
    {
        public static readonly BindableProperty TapCommandProperty = BindableProperty.CreateAttached("TapCommand", typeof(ICommand), typeof(Gesture), null, propertyChanged: CommandChanged);
        public static readonly BindableProperty TapCommand2Property = BindableProperty.CreateAttached("TapCommand2", typeof(Command<Point>), typeof(Gesture), null, propertyChanged: CommandChanged);
        public static readonly BindableProperty DoubleTapCommandProperty = BindableProperty.CreateAttached("DoubleTapCommand", typeof(Command<Point>), typeof(Gesture), null, propertyChanged: CommandChanged);
        public static readonly BindableProperty PanCommandProperty = BindableProperty.CreateAttached("PanCommand", typeof(Command<Point>), typeof(Gesture), null, propertyChanged: CommandChanged);
        public static readonly BindableProperty SwipeLeftCommandProperty = BindableProperty.CreateAttached("SwipeLeftCommand", typeof(ICommand), typeof(Gesture), null, propertyChanged: CommandChanged);
        public static readonly BindableProperty SwipeRightCommandProperty = BindableProperty.CreateAttached("SwipeRightCommand", typeof(ICommand), typeof(Gesture), null, propertyChanged: CommandChanged);
        public static readonly BindableProperty SwipeTopCommandProperty = BindableProperty.CreateAttached("SwipeTopCommand", typeof(ICommand), typeof(Gesture), null, propertyChanged: CommandChanged);
        public static readonly BindableProperty SwipeBottomCommandProperty = BindableProperty.CreateAttached("SwipeBottomCommand", typeof(ICommand), typeof(Gesture), null, propertyChanged: CommandChanged);
        /// <summary>
        /// Android only: min distance to trigger a swipe
        /// </summary>
        public static readonly BindableProperty SwipeThresholdProperty = BindableProperty.CreateAttached("SwipeThreshold", typeof(int), typeof(Gesture), 40, propertyChanged: CommandChanged);
        /// <summary>
        /// Android only for now
        /// </summary>
        public static readonly BindableProperty LongPressCommandProperty = BindableProperty.CreateAttached("LongPressCommand", typeof(Command<Point>), typeof(Gesture), null, propertyChanged: CommandChanged);


        public static ICommand GetTapCommand(BindableObject view) => (ICommand)view.GetValue(TapCommandProperty);
        public static Command<Point> GetTapCommand2(BindableObject view) => (Command<Point>)view.GetValue(TapCommand2Property);
        public static Command<Point> GetDoubleTapCommand(BindableObject view) => (Command<Point>)view.GetValue(DoubleTapCommandProperty);
        public static void SetTapCommand(BindableObject view, ICommand value) => view.SetValue(TapCommandProperty, value);
        public static void SetTapCommand2(BindableObject view, Command<Point> value) => view.SetValue(TapCommand2Property, value);
        public static void SetDoubleTapCommand(BindableObject view, Command<Point> value) => view.SetValue(DoubleTapCommandProperty, value);
        public static Command<Point> GetPanCommand(BindableObject view) => (Command<Point>)view.GetValue(PanCommandProperty);
        public static void SetPanCommand(BindableObject view, Command<Point> value) => view.SetValue(PanCommandProperty, value);
        public static ICommand GetSwipeLeftCommand(BindableObject view) => (ICommand)view.GetValue(SwipeLeftCommandProperty);
        public static void SetSwipeLeftCommand(BindableObject view, ICommand value) => view.SetValue(SwipeLeftCommandProperty, value);
        public static ICommand GetSwipeRightCommand(BindableObject view) => (ICommand)view.GetValue(SwipeRightCommandProperty);
        public static void SetSwipeRightCommand(BindableObject view, ICommand value) => view.SetValue(SwipeRightCommandProperty, value);
        public static ICommand GetSwipeTopCommand(BindableObject view) => (ICommand)view.GetValue(SwipeTopCommandProperty);
        public static void SetSwipeTopCommand(BindableObject view, ICommand value) => view.SetValue(SwipeTopCommandProperty, value);
        public static ICommand GetSwipeBottomCommand(BindableObject view) => (ICommand)view.GetValue(SwipeBottomCommandProperty);
        public static void SetSwipeBottomCommand(BindableObject view, ICommand value) => view.SetValue(SwipeBottomCommandProperty, value);
        public static int GetSwipeThreshold(BindableObject view) => (int)view.GetValue(SwipeThresholdProperty);
        public static void SetSwipeThreshold(BindableObject view, int value) => view.SetValue(SwipeThresholdProperty, value);
        public static Command<Point> GetLongPressCommand(BindableObject view) => (Command<Point>)view.GetValue(LongPressCommandProperty);
        public static void SetLongPressCommand(BindableObject view, Command<Point> value) => view.SetValue(LongPressCommandProperty, value);

        private static GestureEffect GetOrCreateEffect(View view)
        {
            var effect = (GestureEffect)view.Effects.FirstOrDefault(e => e is GestureEffect);
            if (effect == null)
            {
                effect = new GestureEffect();
                view.Effects.Add(effect);
            }
            return effect;
        }

        private static void CommandChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is View view)
                GetOrCreateEffect(view);
        }
        
        class GestureEffect : RoutingEffect
        {
            public GestureEffect() : base("Vapolia.PlatformGestureEffect")
            {
            }
        }
    }
}