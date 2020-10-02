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
        /// <summary>
        /// Android only for now
        /// </summary>
        public static readonly BindableProperty LongPressCommandProperty = BindableProperty.CreateAttached("LongPressCommand", typeof(ICommand), typeof(Gesture), null, propertyChanged: CommandChanged);
        public static readonly BindableProperty TapCommandProperty = BindableProperty.CreateAttached("TapCommand", typeof(ICommand), typeof(Gesture), null, propertyChanged: CommandChanged);
        public static readonly BindableProperty DoubleTapCommandProperty = BindableProperty.CreateAttached("DoubleTapCommand", typeof(ICommand), typeof(Gesture), null, propertyChanged: CommandChanged);
        public static readonly BindableProperty PanCommandProperty = BindableProperty.CreateAttached("PanCommand", typeof(ICommand), typeof(Gesture), null, propertyChanged: CommandChanged);
        
        public static readonly BindableProperty SwipeLeftCommandProperty = BindableProperty.CreateAttached("SwipeLeftCommand", typeof(ICommand), typeof(Gesture), null, propertyChanged: CommandChanged);
        public static readonly BindableProperty SwipeRightCommandProperty = BindableProperty.CreateAttached("SwipeRightCommand", typeof(ICommand), typeof(Gesture), null, propertyChanged: CommandChanged);
        public static readonly BindableProperty SwipeTopCommandProperty = BindableProperty.CreateAttached("SwipeTopCommand", typeof(ICommand), typeof(Gesture), null, propertyChanged: CommandChanged);
        public static readonly BindableProperty SwipeBottomCommandProperty = BindableProperty.CreateAttached("SwipeBottomCommand", typeof(ICommand), typeof(Gesture), null, propertyChanged: CommandChanged);

        /// <summary>
        /// Android only for now
        /// </summary>
        public static readonly BindableProperty LongPressPointCommandProperty = BindableProperty.CreateAttached("LongPressPointCommand", typeof(Command<Point>), typeof(Gesture), null, propertyChanged: CommandChanged);
        public static readonly BindableProperty TapPointCommandProperty = BindableProperty.CreateAttached("TapPointCommand", typeof(Command<Point>), typeof(Gesture), null, propertyChanged: CommandChanged);
        public static readonly BindableProperty DoubleTapPointCommandProperty = BindableProperty.CreateAttached("DoubleTapPointCommand", typeof(Command<Point>), typeof(Gesture), null, propertyChanged: CommandChanged);
        public static readonly BindableProperty PanPointCommandProperty = BindableProperty.CreateAttached("PanPointCommand", typeof(Command<Point>), typeof(Gesture), null, propertyChanged: CommandChanged);
       
        /// <summary>
        /// Android only: min distance to trigger a swipe
        /// </summary>
        public static readonly BindableProperty SwipeThresholdProperty = BindableProperty.CreateAttached("SwipeThreshold", typeof(int), typeof(Gesture), 40, propertyChanged: CommandChanged);
        public static readonly BindableProperty CommandParameterProperty = BindableProperty.CreateAttached("CommandParameter", typeof(object), typeof(Gesture), null);

        public static ICommand GetLongPressCommand(BindableObject view) => (ICommand)view.GetValue(LongPressCommandProperty);
        public static ICommand GetTapCommand(BindableObject view) => (ICommand)view.GetValue(TapCommandProperty);
        public static ICommand GetDoubleTapCommand(BindableObject view) => (ICommand)view.GetValue(DoubleTapCommandProperty);
        public static ICommand GetPanCommand(BindableObject view) => (ICommand)view.GetValue(PanCommandProperty);
        public static ICommand GetSwipeLeftCommand(BindableObject view) => (ICommand)view.GetValue(SwipeLeftCommandProperty);
        public static ICommand GetSwipeRightCommand(BindableObject view) => (ICommand)view.GetValue(SwipeRightCommandProperty);
        public static ICommand GetSwipeTopCommand(BindableObject view) => (ICommand)view.GetValue(SwipeTopCommandProperty);
        public static ICommand GetSwipeBottomCommand(BindableObject view) => (ICommand)view.GetValue(SwipeBottomCommandProperty);
        public static Command<Point> GetLongPressPointCommand(BindableObject view) => (Command<Point>)view.GetValue(LongPressPointCommandProperty);
        public static Command<Point> GetTapPointCommand(BindableObject view) => (Command<Point>)view.GetValue(TapPointCommandProperty);
        public static Command<Point> GetDoubleTapPointCommand(BindableObject view) => (Command<Point>)view.GetValue(DoubleTapPointCommandProperty);
        public static Command<Point> GetPanPointCommand(BindableObject view) => (Command<Point>)view.GetValue(PanPointCommandProperty);

        public static void SetLongPressCommand(BindableObject view, ICommand value) => view.SetValue(LongPressCommandProperty, value);
        public static void SetTapCommand(BindableObject view, ICommand value) => view.SetValue(TapCommandProperty, value);
        public static void SetDoubleTapCommand(BindableObject view, ICommand value) => view.SetValue(DoubleTapCommandProperty, value);
        public static void SetPanCommand(BindableObject view, ICommand value) => view.SetValue(PanCommandProperty, value);
        public static void SetSwipeLeftCommand(BindableObject view, ICommand value) => view.SetValue(SwipeLeftCommandProperty, value);
        public static void SetSwipeRightCommand(BindableObject view, ICommand value) => view.SetValue(SwipeRightCommandProperty, value);
        public static void SetSwipeTopCommand(BindableObject view, ICommand value) => view.SetValue(SwipeTopCommandProperty, value);
        public static void SetSwipeBottomCommand(BindableObject view, ICommand value) => view.SetValue(SwipeBottomCommandProperty, value);
        public static void SetLongPressPointCommand(BindableObject view, Command<Point> value) => view.SetValue(LongPressPointCommandProperty, value);
        public static void SetTapPointCommand(BindableObject view, Command<Point> value) => view.SetValue(TapPointCommandProperty, value);
        public static void SetDoubleTapPointCommand(BindableObject view, Command<Point> value) => view.SetValue(DoubleTapPointCommandProperty, value);
        public static void SetPanPointCommand(BindableObject view, Command<Point> value) => view.SetValue(PanPointCommandProperty, value);

        public static int GetSwipeThreshold(BindableObject view) => (int)view.GetValue(SwipeThresholdProperty);
        public static void SetSwipeThreshold(BindableObject view, int value) => view.SetValue(SwipeThresholdProperty, value);

        public static object GetCommandParameter(BindableObject view) => view.GetValue(CommandParameterProperty);
        public static void SetCommandParameter(BindableObject view, object value) => view.SetValue(CommandParameterProperty, value);

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