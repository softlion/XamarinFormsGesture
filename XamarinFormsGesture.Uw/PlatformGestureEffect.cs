// Do not remove this notice
// Copyright (c)2016 Vapolia. All rights reserved.
// Usage licence automatically acquired by Vapolia's customers when added to their product's source code under a contract signed with Vapolia.

using System;
using System.ComponentModel;
using System.Linq;
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
[assembly: LinkerSafe]

namespace Vapolia.Uw.Lib.Effects
{
    [Preserve (Conditional=true, AllMembers = true)]
    public class PlatformGestureEffect : PlatformEffect
    {
        private Windows.UI.Input.GestureRecognizer detector;
        private Command<Point> tapCommand2;
        private ICommand tapCommand, swipeLeftCommand, swipeRightCommand, swipeTopCommand, swipeBottomCommand;
        private Command<Point> panCommand;

        public static void Init()
        {
        }

        public PlatformGestureEffect()
        {
            detector = new Windows.UI.Input.GestureRecognizer
            {
                GestureSettings = GestureSettings.Tap | GestureSettings.Drag | GestureSettings.ManipulationTranslateInertia,
                ShowGestureFeedback = false,
                //CrossSlideHorizontally = true
            };

            detector.Dragging += (sender, args) => TriggerCommand(panCommand, new Point(args.Position.X, args.Position.Y));

            detector.Tapped += (sender, args) =>
            {
                TriggerCommand(tapCommand, null);
                TriggerCommand(tapCommand2, new Point(args.Position.X, args.Position.Y));
            };

            detector.ManipulationInertiaStarting += (sender, args) =>
            {
                var isHorizontalSwipe = Math.Abs(args.Delta.Translation.Y) < 20;
                var isVerticalSwipe = Math.Abs(args.Delta.Translation.X) < 20;
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
            swipeLeftCommand = Gesture.GetSwipeLeftCommand(Element);
            swipeRightCommand = Gesture.GetSwipeRightCommand(Element);
            swipeTopCommand = Gesture.GetSwipeTopCommand(Element);
            swipeBottomCommand = Gesture.GetSwipeBottomCommand(Element);
            panCommand = Gesture.GetPanCommand(Element);
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

        private void ControlOnPointerCanceled(object sender, PointerRoutedEventArgs pointerRoutedEventArgs)
        {
            detector.CompleteGesture();
            pointerRoutedEventArgs.Handled = true;
        }

        private void ControlOnPointerReleased(object sender, PointerRoutedEventArgs pointerRoutedEventArgs)
        {
            detector.ProcessUpEvent(pointerRoutedEventArgs.GetCurrentPoint(Control ?? Container));
            pointerRoutedEventArgs.Handled = true;
        }
    }
}
