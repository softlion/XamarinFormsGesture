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
        int swipeThresholdInPoints = 40;
        private object commandParameter;
        
        /// <summary>
        /// Take a Point parameter
        /// Except panPointCommand which takes a (Point,GestureStatus) parameter (its a tuple) 
        /// </summary>
        private ICommand tapPointCommand, panPointCommand, doubleTapPointCommand, longPressPointCommand;
        
        /// <summary>
        /// No parameter
        /// </summary>
        private ICommand tapCommand, panCommand, doubleTapCommand, longPressCommand, swipeLeftCommand, swipeRightCommand, swipeTopCommand, swipeBottomCommand;
        
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
                //AutoProcessInertia = true //default
            };

            detector.Dragging += (sender, args) => 
            {
                TriggerCommand(panCommand, commandParameter);
                var gestureStatus = args.DraggingState switch
                {
                    DraggingState.Started => GestureStatus.Started,
                    DraggingState.Continuing => GestureStatus.Running,
                    DraggingState.Completed => GestureStatus.Completed,
                    _ => GestureStatus.Canceled
                };
                var parameters = (new Point(args.Position.X, args.Position.Y), gestureStatus);
                TriggerCommand(panPointCommand, parameters);
            };

            detector.Tapped += (sender, args) =>
            {
                if (args.TapCount == 1)
                {
                    TriggerCommand(tapCommand, commandParameter);
                    TriggerCommand(tapPointCommand, new Point(args.Position.X, args.Position.Y));
                }
                else if (args.TapCount == 2) 
                {
                    TriggerCommand(doubleTapCommand, commandParameter);
                    TriggerCommand(doubleTapPointCommand, new Point(args.Position.X, args.Position.Y));
                }
            };

            detector.Holding += (sender, args) =>
            {
                if(args.HoldingState == HoldingState.Started) 
                {
                    TriggerCommand(longPressCommand, commandParameter);
                    TriggerCommand(longPressPointCommand, new Point(args.Position.X, args.Position.Y));
                }
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
                        TriggerCommand(isLeftSwipe ? swipeLeftCommand : swipeRightCommand, commandParameter);
                    }
                    else
                    {
                        var isTopSwipe = args.Delta.Translation.Y < 0;
                        TriggerCommand(isTopSwipe ? swipeTopCommand : swipeBottomCommand, commandParameter);
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
            panCommand = Gesture.GetPanCommand(Element);
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

            swipeThresholdInPoints = Gesture.GetSwipeThreshold(Element);
            commandParameter = Gesture.GetCommandParameter(Element);
        }

        protected override void OnAttached()
        {
            var control = Control ?? Container;

            control.PointerMoved += ControlOnPointerMoved;
            control.PointerPressed += ControlOnPointerPressed;
            control.PointerReleased += ControlOnPointerReleased;
            control.PointerCanceled += ControlOnPointerCanceled;
            control.PointerCaptureLost += ControlOnPointerCanceled;
            //control.ManipulationInertiaStarting += ControlOnManipulationInertiaStarting;
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
            //control.ManipulationInertiaStarting -= ControlOnManipulationInertiaStarting;
            //control.PointerExited -= ControlOnPointerCanceled;
        }

        //private void ControlOnManipulationInertiaStarting(object sender, ManipulationInertiaStartingRoutedEventArgs e)
        //{
        //    detector.ProcessInertia();
        //    e.Handled = true;
        //}

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
