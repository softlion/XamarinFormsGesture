using System;
using System.ComponentModel;
using System.Windows.Input;
using Android.Util;
using Android.Views;
using Vapolia.Droid.Lib.Effects;
using Vapolia.Lib.Ui;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.Android;
using View = Android.Views.View;

#if MONOANDROID90
using Android.Support.V4.View;
#else
using AndroidX.Core.View;
#endif

[assembly: ResolutionGroupName("Vapolia")]
[assembly: ExportEffect(typeof(PlatformGestureEffect), nameof(PlatformGestureEffect))]

namespace Vapolia.Droid.Lib.Effects
{
    [Preserve (Conditional=true, AllMembers = true)]
    public class PlatformGestureEffect : PlatformEffect
    {
        private GestureDetectorCompat? gestureRecognizer;
        private readonly InternalGestureDetector tapDetector;
        private DisplayMetrics displayMetrics;
        private object commandParameter;

        /// <summary>
        /// Take a Point parameter
        /// Except panPointCommand which takes a (Point,GestureStatus) parameter (its a tuple) 
        /// </summary>
        private ICommand? tapPointCommand, panPointCommand, doubleTapPointCommand, longPressPointCommand;
        
        /// <summary>
        /// No parameter
        /// </summary>
        private ICommand? tapCommand, panCommand, doubleTapCommand, longPressCommand, swipeLeftCommand, swipeRightCommand, swipeTopCommand, swipeBottomCommand;

        /// <summary>
        /// 1 parameter: PinchEventArgs
        /// </summary>
        private ICommand pinchCommand;
        

        public static void Init()
        {
        }

        public PlatformGestureEffect()
        {
            tapDetector = new InternalGestureDetector
            {
                SwipeThresholdInPoints = 40,
                TapAction = motionEvent =>
                {
                    if (tapPointCommand != null)
                    {
                        var x = motionEvent.GetX();
                        var y = motionEvent.GetY();

                        var point = PxToDp(new Point(x,y));
                        if (tapPointCommand.CanExecute(point))
                            tapPointCommand.Execute(point);
                    }

                    if(tapCommand != null) {
                        if (tapCommand.CanExecute(commandParameter))
                            tapCommand.Execute(commandParameter);
                    }
                },
                DoubleTapAction = motionEvent =>
                {
                    if (doubleTapPointCommand != null) {
                        var x = motionEvent.GetX();
                        var y = motionEvent.GetY();

                        var point = PxToDp(new Point(x, y));
                        if (doubleTapPointCommand.CanExecute(point))
                            doubleTapPointCommand.Execute(point);
                    }

                    if (doubleTapCommand != null) {
                        if (doubleTapCommand.CanExecute(commandParameter))
                            doubleTapCommand.Execute(commandParameter);
                    }
                },
                SwipeLeftAction = motionEvent =>
                {
                    if (swipeLeftCommand != null) {
                        if (swipeLeftCommand.CanExecute(commandParameter))
                            swipeLeftCommand.Execute(commandParameter);
                    }
                },
                SwipeRightAction = motionEvent =>
                {
                    if (swipeRightCommand != null) {
                        if (swipeRightCommand.CanExecute(commandParameter))
                            swipeRightCommand.Execute(commandParameter);
                    }
                },
                SwipeTopAction = motionEvent =>
                {
                    if (swipeTopCommand != null) {
                        if (swipeTopCommand.CanExecute(commandParameter))
                            swipeTopCommand.Execute(commandParameter);
                    }
                },
                SwipeBottomAction = motionEvent =>
                {
                    if (swipeBottomCommand != null) {
                        if (swipeBottomCommand.CanExecute(commandParameter))
                            swipeBottomCommand.Execute(commandParameter);
                    }
                },
                PanAction = (initialDown, currentMove) =>
                {
                    if (panPointCommand != null)
                    {
                        // var x0 = initialDown.GetX();
                        // var y0 = initialDown.GetY();
                        var x = currentMove.GetX();
                        var y = currentMove.GetY();
                        // var point = PxToDp(new Point(x-x0, y-y0));
                        var point = PxToDp(new Point(x, y));

                        var status = currentMove.Action switch
                        {
                            MotionEventActions.Down => GestureStatus.Started,
                            MotionEventActions.Move => GestureStatus.Running,
                            MotionEventActions.Up => GestureStatus.Completed,
                            MotionEventActions.Cancel => GestureStatus.Canceled,
                            _ => GestureStatus.Canceled
                        };

                        var parameter = (point,status);
                        if (panPointCommand.CanExecute(parameter))
                            panPointCommand.Execute(parameter);
                    }

                    if (panCommand != null) {
                        if (panCommand.CanExecute(commandParameter))
                            panCommand.Execute(commandParameter);
                    }
                },
                PinchAction = (initialDown, currentMove) =>
                {
                    if (pinchCommand != null)
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

                        var commandParameter = new PinchEventArgs(status, (current0, current1), (origin0, origin1));
                        if (pinchCommand.CanExecute(commandParameter))
                            pinchCommand.Execute(commandParameter);
                    }
                },
                LongPressAction = motionEvent =>
                {
                    if (longPressPointCommand != null)
                    {
                        var x = motionEvent.GetX();
                        var y = motionEvent.GetY();

                        var point = PxToDp(new Point(x, y));
                        if (longPressPointCommand.CanExecute(point))
                            longPressPointCommand.Execute(point);
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
            point.X = point.X / displayMetrics.Density;
            point.Y = point.Y / displayMetrics.Density;
            return point;
        }

        protected override void OnElementPropertyChanged(PropertyChangedEventArgs args)
        {
            tapCommand = Gesture.GetTapCommand(Element);
            panCommand = Gesture.GetPanCommand(Element);
            pinchCommand = Gesture.GetPinchCommand(Element);
            tapDetector.IsPanImmediate = Gesture.GetIsPanImmediate(Element);
            doubleTapCommand = Gesture.GetDoubleTapCommand(Element);
            longPressCommand = Gesture.GetLongPressCommand(Element);

            swipeLeftCommand = Gesture.GetSwipeLeftCommand(Element);
            swipeRightCommand = Gesture.GetSwipeRightCommand(Element);
            swipeTopCommand = Gesture.GetSwipeTopCommand(Element);
            swipeBottomCommand = Gesture.GetSwipeBottomCommand(Element);

            tapPointCommand = Gesture.GetTapPointCommand(Element);
            panPointCommand = Gesture.GetPanPointCommand(Element);
            doubleTapPointCommand = Gesture.GetDoubleTapPointCommand(Element);
            longPressPointCommand = Gesture.GetLongPressPointCommand(Element);

            tapDetector.SwipeThresholdInPoints = Gesture.GetSwipeThreshold(Element);
            commandParameter = Gesture.GetCommandParameter(Element);
        }

        protected override void OnAttached()
        {
            var control = Control ?? Container;

            var context = control.Context;
            displayMetrics = context.Resources.DisplayMetrics;
            tapDetector.Density = displayMetrics.Density;

            if (gestureRecognizer == null)
                gestureRecognizer = new GestureDetectorCompat(context, tapDetector);
            control.Touch += ControlOnTouch;
            control.Clickable = true;

            OnElementPropertyChanged(new PropertyChangedEventArgs(String.Empty));
        }

        private void ControlOnTouch(object sender, View.TouchEventArgs touchEventArgs)
        {
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

        sealed class InternalGestureDetector : GestureDetector.SimpleOnGestureListener
        {
            public int SwipeThresholdInPoints { get; set; }
            public bool IsPanImmediate { get; set; }

            public Action<MotionEvent>? TapAction { get; set; }
            public Action<MotionEvent>? DoubleTapAction { get; set; }
            public Action<MotionEvent>? SwipeLeftAction { get; set; }
            public Action<MotionEvent>? SwipeRightAction { get; set; }
            public Action<MotionEvent>? SwipeTopAction { get; set; }
            public Action<MotionEvent>? SwipeBottomAction { get; set; }
            public Action<MotionEvent, MotionEvent>? PanAction { get; set; }
            public Action<MotionEvent, MotionEvent>? PinchAction { get; set; }
            public Action<MotionEvent>? LongPressAction { get; set; }

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
                if (e!=null && IsPanImmediate && e.PointerCount == 1)
                    PanAction?.Invoke(e, e);
                return false;
            }

            public override bool OnScroll(MotionEvent? initialDown, MotionEvent? currentMove, float distanceX, float distanceY)
            {
                if (initialDown != null)
                {
                    if(initialDown.PointerCount == 1)
                        PanAction?.Invoke(initialDown, currentMove);
                    else if(initialDown.PointerCount == 2)
                        PinchAction?.Invoke(initialDown, currentMove);
                }
                return true;
            }

            public override bool OnFling(MotionEvent? e1, MotionEvent? e2, float velocityX, float velocityY)
            {
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
}
