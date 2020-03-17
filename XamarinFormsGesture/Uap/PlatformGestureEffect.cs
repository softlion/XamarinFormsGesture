using System;
using System.ComponentModel;
using System.Windows.Input;
using Windows.UI.Input;
using Windows.UI.Xaml.Input;
using Vapolia.Uw.Lib.Effects;
using Vapolia.Lib.Ui;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.UWP;

[assembly: ResolutionGroupName("Vapolia")]
[assembly: ExportEffect(typeof(PlatformGestureEffect), nameof(PlatformGestureEffect))]

namespace Vapolia.Uw.Lib.Effects
{
    [Preserve (Conditional=true, AllMembers = true)]
    public class PlatformGestureEffect : PlatformEffect
    {
        readonly Windows.UI.Input.GestureRecognizer detector;
        Command<Point> tapCommand2, panCommand, doubleTapCommand, longPressCommand;
        ICommand tapCommand, swipeLeftCommand, swipeRightCommand, swipeTopCommand, swipeBottomCommand;
        int swipeThresholdInPoints = 40;

        public static void Init()
        {
        }

        public PlatformGestureEffect()
        {
            detector = new Windows.UI.Input.GestureRecognizer
            {
                GestureSettings = 
                    GestureSettings.Tap 
                    | GestureSettings.Drag 
                    | GestureSettings.ManipulationTranslateInertia 
                    | GestureSettings.DoubleTap
                    | GestureSettings.Hold
                    | GestureSettings.HoldWithMouse,
                ShowGestureFeedback = false,
                //CrossSlideHorizontally = true
            };

            detector.Dragging += (sender, args) => TriggerCommand(panCommand, new Point(args.Position.X, args.Position.Y));

            detector.Tapped += (sender, args) =>
            {
                if (args.TapCount == 1)
                {
                    TriggerCommand(tapCommand, null);
                    TriggerCommand(tapCommand2, new Point(args.Position.X, args.Position.Y));
                }
                else if (args.TapCount == 2)
                    TriggerCommand(doubleTapCommand, new Point(args.Position.X, args.Position.Y));
            };

            detector.Holding += (sender, args) =>
            {
                if(args.HoldingState == HoldingState.Started)
                    TriggerCommand(longPressCommand, new Point(args.Position.X, args.Position.Y));
            };

            //Never called. Don't know why.
            detector.ManipulationInertiaStarting += (sender, args) =>
            {
                var isHorizontalSwipe = Math.Abs(args.Delta.Translation.Y) < swipeThresholdInPoints;
                var isVerticalSwipe = Math.Abs(args.Delta.Translation.X) < swipeThresholdInPoints;
                if (isHorizontalSwipe || isVerticalSwipe)
                {
                    if (isHorizontalSwipe)
                    {
                        var isLeftSwipe = args.Delta.Translation.X < 0;
                        TriggerCommand(isLeftSwipe ? swipeLeftCommand : swipeRightCommand, null);
                    }
                    else
                    {
                        var isTopSwipe = args.Delta.Translation.Y < 0;
                        TriggerCommand(isTopSwipe ? swipeTopCommand : swipeBottomCommand, null);
                    }
                }
            };

            //detector.CrossSliding += (sender, args) =>
            //{
            //    args.CrossSlidingState == CrossSlidingState.Started
            //};
        }

        private void TriggerCommand(ICommand command, object parameter)
        {
            if(command?.CanExecute(parameter) == true)
                command.Execute(parameter);
        }

        protected override void OnElementPropertyChanged(PropertyChangedEventArgs args)
        {
            tapCommand = Gesture.GetTapCommand(Element);
            tapCommand2 = Gesture.GetTapCommand2(Element);
            doubleTapCommand = Gesture.GetDoubleTapCommand(Element);
            longPressCommand = Gesture.GetLongPressCommand(Element);

            swipeLeftCommand = Gesture.GetSwipeLeftCommand(Element);
            swipeRightCommand = Gesture.GetSwipeRightCommand(Element);
            swipeTopCommand = Gesture.GetSwipeTopCommand(Element);
            swipeBottomCommand = Gesture.GetSwipeBottomCommand(Element);
            panCommand = Gesture.GetPanCommand(Element);

            swipeThresholdInPoints = Gesture.GetSwipeThreshold(Element);
        }

        protected override void OnAttached()
        {
            var control = Control ?? Container;

            control.PointerMoved += ControlOnPointerMoved;
            control.PointerPressed += ControlOnPointerPressed;
            control.PointerReleased += ControlOnPointerReleased;
            control.PointerCanceled += ControlOnPointerCanceled;
            control.PointerCaptureLost += ControlOnPointerCanceled;
            //control.PointerExited += ControlOnPointerCanceled;

            OnElementPropertyChanged(new PropertyChangedEventArgs(String.Empty));
        }

        protected override void OnDetached()
        {
            var control = Control ?? Container;
            control.PointerMoved -= ControlOnPointerMoved;
            control.PointerPressed -= ControlOnPointerPressed;
            control.PointerReleased -= ControlOnPointerReleased;
            control.PointerCanceled -= ControlOnPointerCanceled;
            control.PointerCaptureLost -= ControlOnPointerCanceled;
            //control.PointerExited -= ControlOnPointerCanceled;
        }

        private void ControlOnPointerPressed(object sender, PointerRoutedEventArgs pointerRoutedEventArgs)
        {
            detector.CompleteGesture();
            detector.ProcessDownEvent(pointerRoutedEventArgs.GetCurrentPoint(Control ?? Container));
            pointerRoutedEventArgs.Handled = true;
        }

        private void ControlOnPointerMoved(object sender, PointerRoutedEventArgs pointerRoutedEventArgs)
        {
            detector.ProcessMoveEvents(pointerRoutedEventArgs.GetIntermediatePoints(Control ?? Container));
            pointerRoutedEventArgs.Handled = true;
        }

        private void ControlOnPointerCanceled(object sender, PointerRoutedEventArgs args)
        {
            detector.CompleteGesture();
            args.Handled = true;
        }

        private void ControlOnPointerReleased(object sender, PointerRoutedEventArgs pointerRoutedEventArgs)
        {
            detector.ProcessUpEvent(pointerRoutedEventArgs.GetCurrentPoint(Control ?? Container));
            pointerRoutedEventArgs.Handled = true;
        }
    }
}
