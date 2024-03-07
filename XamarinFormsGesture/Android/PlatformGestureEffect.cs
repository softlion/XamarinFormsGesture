using System;
using System.ComponentModel;
using Android.Util;
using Android.Views;
using Vapolia.Lib.Ui;
using Vapolia.Lib.Effects;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.Android;
using View = Android.Views.View;

[assembly: ResolutionGroupName("Vapolia")]
[assembly: ExportEffect(typeof(PlatformGestureEffect), nameof(PlatformGestureEffect))]

namespace Vapolia.Lib.Effects;

[Preserve (Conditional=true, AllMembers = true)]
public partial class PlatformGestureEffect : PlatformEffect
{
    private GestureDetector? gestureRecognizer;
    private readonly InternalGestureDetector tapDetector;
    private DisplayMetrics? displayMetrics;

    public static void Init()
    {
    }

    public PlatformGestureEffect()
    {
        tapDetector = new ()
        {
            SwipeThresholdInPoints = 40,
            TapAction = motionEvent =>
            {
                if (tapPointCommand != null && motionEvent != null)
                {
                    var x = motionEvent.GetX();
                    var y = motionEvent.GetY();
                    var point = PxToDp(new Point(x,y));
                    var args = new PointEventArgs(point, Element, Element.BindingContext);

                    if (tapPointCommand.CanExecute(args))
                        tapPointCommand.Execute(args);
                }

                if(tapCommand != null) {
                    if (tapCommand.CanExecute(commandParameter))
                        tapCommand.Execute(commandParameter);
                }
            },
            DoubleTapAction = motionEvent =>
            {
                if (doubleTapPointCommand != null && motionEvent != null) {
                    var x = motionEvent.GetX();
                    var y = motionEvent.GetY();
                    var point = PxToDp(new Point(x, y));
                    var args = new PointEventArgs(point, Element, Element.BindingContext);

                    if (doubleTapPointCommand.CanExecute(args))
                        doubleTapPointCommand.Execute(args);
                }

                if (doubleTapCommand != null) {
                    if (doubleTapCommand.CanExecute(commandParameter))
                        doubleTapCommand.Execute(commandParameter);
                }
            },
            SwipeLeftAction = _ =>
            {
                if (swipeLeftCommand != null) {
                    if (swipeLeftCommand.CanExecute(commandParameter))
                        swipeLeftCommand.Execute(commandParameter);
                }
            },
            SwipeRightAction = _ =>
            {
                if (swipeRightCommand != null) {
                    if (swipeRightCommand.CanExecute(commandParameter))
                        swipeRightCommand.Execute(commandParameter);
                }
            },
            SwipeTopAction = _ =>
            {
                if (swipeTopCommand != null) {
                    if (swipeTopCommand.CanExecute(commandParameter))
                        swipeTopCommand.Execute(commandParameter);
                }
            },
            SwipeBottomAction = _ =>
            {
                if (swipeBottomCommand != null) {
                    if (swipeBottomCommand.CanExecute(commandParameter))
                        swipeBottomCommand.Execute(commandParameter);
                }
            },
            PanAction = (initialDown, currentMove) =>
            {
                var continueGesture = true;
                    
                if (panPointCommand != null && currentMove != null)
                {
                    var x = currentMove.GetX();
                    var y = currentMove.GetY();
                    var point = PxToDp(new(x, y));

                    var status = currentMove.Action switch
                    {
                        MotionEventActions.Down => GestureStatus.Started,
                        MotionEventActions.Move => GestureStatus.Running,
                        MotionEventActions.Up => GestureStatus.Completed,
                        MotionEventActions.Cancel => GestureStatus.Canceled,
                        _ => GestureStatus.Canceled
                    };

                    var parameter = new PanEventArgs(status,point);
                    if (panPointCommand.CanExecute(parameter))
                        panPointCommand.Execute(parameter);
                    if (parameter.CancelGesture)
                        continueGesture = false;
                }

                if (panCommand != null) {
                    if (panCommand.CanExecute(commandParameter))
                        panCommand.Execute(commandParameter);
                }

                return continueGesture;
            },
            PinchAction = (initialDown, currentMove) =>
            {
                if (pinchCommand != null && currentMove != null)
                {
                    var origin0 = PxToDp(new Point(initialDown.GetX(0), initialDown.GetY(0)));
                    var origin1 = PxToDp(new Point(initialDown.GetX(1), initialDown.GetY(1)));
                    var current0 = PxToDp(new Point(currentMove.GetX(0), currentMove.GetY(0)));
                    var current1 = PxToDp(new Point(currentMove.GetX(1), currentMove.GetY(1)));
                        
                    var status = currentMove.Action switch
                    {
                        MotionEventActions.Down => GestureStatus.Started,
                        MotionEventActions.Move => GestureStatus.Running,
                        MotionEventActions.Up => GestureStatus.Completed,
                        MotionEventActions.Cancel => GestureStatus.Canceled,
                        _ => GestureStatus.Canceled
                    };

                    var parameters = new PinchEventArgs(status, (current0, current1), (origin0, origin1));
                    if (pinchCommand.CanExecute(parameters))
                        pinchCommand.Execute(parameters);
                }
            },
            LongPressAction = motionEvent =>
            {
                if (longPressPointCommand != null && motionEvent != null)
                {
                    var x = motionEvent.GetX();
                    var y = motionEvent.GetY();
                    var point = PxToDp(new Point(x, y));
                    var args = new PointEventArgs(point, Element, Element.BindingContext);

                    if (longPressPointCommand.CanExecute(args))
                        longPressPointCommand.Execute(args);
                }

                if (longPressCommand != null) {
                    if (longPressCommand.CanExecute(commandParameter))
                        longPressCommand.Execute(commandParameter);
                }
            },
        };
    }

    private Point PxToDp(Point point)
    {
        point.X /= displayMetrics.Density;
        point.Y /= displayMetrics.Density;
        return point;
    }

    protected override void OnAttached()
    {
        var control = Control ?? Container;

        var context = control.Context;
        displayMetrics = context.Resources.DisplayMetrics;
        tapDetector.Density = displayMetrics.Density;

        if (gestureRecognizer == null)
            gestureRecognizer = new ExtendedGestureDetector(context, tapDetector);
        control.Touch += ControlOnTouch;
        control.Clickable = true;

        OnElementPropertyChanged(new PropertyChangedEventArgs(String.Empty));
    }

    private void ControlOnTouch(object? sender, View.TouchEventArgs touchEventArgs)
    {
        if( touchEventArgs.Event != null)
            gestureRecognizer?.OnTouchEvent(touchEventArgs.Event);
        touchEventArgs.Handled = false;
    }

    protected override void OnDetached()
    {
        var control = Control ?? Container;
        control.Touch -= ControlOnTouch;

        var g = gestureRecognizer;
        gestureRecognizer = null;
        g?.Dispose();
        displayMetrics = null;
    }

    sealed class ExtendedGestureDetector : GestureDetector
    {
        private readonly IExtendedGestureListener? myGestureListener;

        [Preserve]
        protected ExtendedGestureDetector(IntPtr javaRef, Android.Runtime.JniHandleOwnership transfer) : base(javaRef, transfer)
        { 
        }

        public ExtendedGestureDetector(Android.Content.Context context, GestureDetector.IOnGestureListener listener) : base(context, listener)
        { 
            if (listener is IExtendedGestureListener my)
                myGestureListener = my;
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            if (myGestureListener != null && e.Action == MotionEventActions.Up)
                myGestureListener.OnUp(e);
            return base.OnTouchEvent(e);
        }
    }


    interface IExtendedGestureListener
    {
        void OnUp(MotionEvent? e);
    }

    sealed class InternalGestureDetector : GestureDetector.SimpleOnGestureListener, IExtendedGestureListener
    {
        private MotionEvent? pinchInitialDown;

        public int SwipeThresholdInPoints { get; set; }
        public bool IsPanImmediate { get; set; }
        public bool IsPinchImmediate { get; set; }

        public Action<MotionEvent?>? TapAction { get; set; }
        public Action<MotionEvent?>? DoubleTapAction { get; set; }
        public Action<MotionEvent>? SwipeLeftAction { get; set; }
        public Action<MotionEvent>? SwipeRightAction { get; set; }
        public Action<MotionEvent>? SwipeTopAction { get; set; }
        public Action<MotionEvent>? SwipeBottomAction { get; set; }
        public Func<MotionEvent, MotionEvent?, bool>? PanAction { get; set; }
        public Action<MotionEvent, MotionEvent?>? PinchAction { get; set; }
        public Action<MotionEvent?>? LongPressAction { get; set; }

        public float Density { get; set; }

        public override bool OnDoubleTap(MotionEvent? e)
        {
            DoubleTapAction?.Invoke(e);
            return true;
        }


        public override bool OnSingleTapUp(MotionEvent? e)
        {
            TapAction?.Invoke(e);
            return true;
        }

        public override void OnLongPress(MotionEvent? e)
        {
            LongPressAction?.Invoke(e);
        }

        public override bool OnDown(MotionEvent? e)
        {
            if (e!=null && IsPanImmediate && e.PointerCount == 1 && PanAction != null)
                return PanAction.Invoke(e, e);

            if (e != null && IsPinchImmediate && e.PointerCount == 2 && PinchAction != null)
            {
                PinchAction?.Invoke(e, e);
                return true;
            }

            return false;
        }

        public void OnUp(MotionEvent? e)
        {
            if(e != null)
                PanAction?.Invoke(e, e);
                
            pinchInitialDown = null;
        }

        public override bool OnScroll(MotionEvent? initialDown, MotionEvent? currentMove, float distanceX, float distanceY)
        {
            if (initialDown != null && currentMove != null)
            {
                //Switch to pinch
                if (pinchInitialDown == null && initialDown.PointerCount == 1 && currentMove.PointerCount == 2)
                    pinchInitialDown = MotionEvent.Obtain(currentMove);
                    
                if(currentMove.PointerCount == 1 && PanAction != null)
                    return PanAction.Invoke(initialDown, currentMove);

                if (currentMove.PointerCount == 2 && PinchAction != null && pinchInitialDown != null)
                {
                    PinchAction.Invoke(pinchInitialDown, currentMove);
                    return true;
                }
            }
            return false;
        }

        public override bool OnFling(MotionEvent? e1, MotionEvent? e2, float velocityX, float velocityY)
        {
            if (e1 == null || e2 == null)
                return false;

            var dx = e2.RawX - e1.RawX;
            var dy = e2.RawY - e1.RawY;
            if (Math.Abs(dx) > SwipeThresholdInPoints * Density)
            {
                if(dx>0)
                    SwipeRightAction?.Invoke(e2);
                else
                    SwipeLeftAction?.Invoke(e2);
            }
            else if (Math.Abs(dy) > SwipeThresholdInPoints * Density)
            {
                if (dy > 0)
                    SwipeBottomAction?.Invoke(e2);
                else
                    SwipeTopAction?.Invoke(e2);
            }
            return true;
        }
    }
}