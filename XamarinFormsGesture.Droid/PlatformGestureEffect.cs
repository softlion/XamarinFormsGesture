// Do not remove this notice
// Copyright (c)2016 Vapolia. All rights reserved.
// Usage licence automatically acquired by Vapolia's customers when added to their product's source code under a contract signed with Vapolia.

using System;
using System.ComponentModel;
using System.Windows.Input;
using Android.Support.V4.View;
using Android.Util;
using Android.Views;
using Vapolia.Droid.Lib.Effects;
using Vapolia.Lib.Ui;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.Android;
using View = Android.Views.View;

[assembly: ResolutionGroupName("Vapolia")]
[assembly: ExportEffect(typeof(PlatformGestureEffect), nameof(PlatformGestureEffect))]
[assembly: LinkerSafe]

namespace Vapolia.Droid.Lib.Effects
{
    [Preserve (Conditional=true, AllMembers = true)]
    public class PlatformGestureEffect : PlatformEffect
    {
        private GestureDetectorCompat gestureRecognizer;
        private readonly InternalGestureDetector tapDetector;
        private Command<Point> tapCommand2, panCommand;
        private ICommand tapCommand, swipeLeftCommand, swipeRightCommand, swipeTopCommand, swipeBottomCommand;
        private DisplayMetrics displayMetrics;

        public static void Init()
        {
        }

        public PlatformGestureEffect()
        {
            tapDetector = new InternalGestureDetector
            {
                TapAction = motionEvent =>
                {
                    var command = tapCommand2;
                    if (command != null)
                    {
                        var x = motionEvent.GetX();
                        var y = motionEvent.GetY();

                        var point = PxToDp(new Point(x,y));
                        if (command.CanExecute(point))
                            command.Execute(point);
                    }
                    var handler = tapCommand;
                    if (handler?.CanExecute(null) == true)
                        handler.Execute(null);
                },
                SwipeLeftAction = motionEvent =>
                {
                    var handler = swipeLeftCommand;
                    if (handler?.CanExecute(null) == true)
                        handler.Execute(null);
                },
                SwipeRightAction = motionEvent =>
                {
                    var handler = swipeRightCommand;
                    if (handler?.CanExecute(null) == true)
                        handler.Execute(null);
                },
                SwipeTopAction = motionEvent =>
                {
                    var handler = swipeTopCommand;
                    if (handler?.CanExecute(null) == true)
                        handler.Execute(null);
                },
                SwipeBottomAction = motionEvent =>
                {
                    var handler = swipeBottomCommand;
                    if (handler?.CanExecute(null) == true)
                        handler.Execute(null);
                },
                PanAction = (initialDown, currentMove) =>
                {
                    var command = panCommand;
                    if (command != null)
                    {
                        var x0 = initialDown.GetX();
                        var y0 = initialDown.GetY();
                        var x = currentMove.GetX();
                        var y = currentMove.GetY();

                        var point = PxToDp(new Point(x-x0, y-y0));
                        if (command.CanExecute(point))
                            command.Execute(point);
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
            tapCommand2 = Gesture.GetTapCommand2(Element);
            swipeLeftCommand = Gesture.GetSwipeLeftCommand(Element);
            swipeRightCommand = Gesture.GetSwipeRightCommand(Element);
            swipeTopCommand = Gesture.GetSwipeTopCommand(Element);
            swipeBottomCommand = Gesture.GetSwipeBottomCommand(Element);
            panCommand = Gesture.GetPanCommand(Element);
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
        }

        sealed class InternalGestureDetector : GestureDetector.SimpleOnGestureListener
        {
            private const int SwipeThresholdInPoints = 40;

            public Action<MotionEvent> TapAction { get; set; }
            public Action<MotionEvent> SwipeLeftAction { get; set; }
            public Action<MotionEvent> SwipeRightAction { get; set; }
            public Action<MotionEvent> SwipeTopAction { get; set; }
            public Action<MotionEvent> SwipeBottomAction { get; set; }
            public Action<MotionEvent, MotionEvent> PanAction { get; set; }

            public float Density { get; set; }

            public override bool OnSingleTapUp(MotionEvent e)
            {
                TapAction?.Invoke(e);
                return true;
            }

            public override bool OnScroll(MotionEvent initialDown, MotionEvent currentMove, float distanceX, float distanceY)
            {
                PanAction?.Invoke(initialDown, currentMove);
                return true;
            }

            public override bool OnFling(MotionEvent e1, MotionEvent e2, float velocityX, float velocityY)
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
