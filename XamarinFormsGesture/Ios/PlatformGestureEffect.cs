using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using UIKit;
using Vapolia.Ios.Lib.Effects;
using Vapolia.Lib.Ui;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ResolutionGroupName("Vapolia")]
[assembly: ExportEffect(typeof(PlatformGestureEffect), nameof(PlatformGestureEffect))]

namespace Vapolia.Ios.Lib.Effects
{
    [Xamarin.Forms.Internals.Preserve (Conditional=true, AllMembers = true)]
    public class PlatformGestureEffect : PlatformEffect
    {
        private readonly UITapGestureRecognizer tapDetector, doubleTapDetector;
        private readonly UILongPressGestureRecognizer longPressDetector;
        private readonly UISwipeGestureRecognizer swipeLeftDetector, swipeRightDetector, swipeUpDetector, swipeDownDetector;
        private readonly UIImmediatePanGestureRecognizer panDetector;
        private readonly UIImmediatePinchGestureRecognizer pinchDetector;
        private readonly List<UIGestureRecognizer> recognizers;
        private (Point Origin0, Point Origin1) pinchOrigin, lastPinch;

        /// <summary>
        /// Take a Point parameter
        /// Except panPointCommand which takes a (Point,GestureStatus) parameter (its a tuple) 
        /// </summary>
        private ICommand tapPointCommand, panPointCommand, doubleTapPointCommand, longPressPointCommand;
        
        /// <summary>
        /// No parameter
        /// </summary>
        private ICommand tapCommand, panCommand, doubleTapCommand, longPressCommand, swipeLeftCommand, swipeRightCommand, swipeTopCommand, swipeBottomCommand;

        /// <summary>
        /// 1 parameter: PinchEventArgs
        /// </summary>
        private ICommand pinchCommand;
        
        private object commandParameter;

        public static void Init()
        {
        }

        public PlatformGestureEffect()
        {
            //if (!allSubviews)
            //    tapDetector.ShouldReceiveTouch = (s, args) => args.View != null && (args.View == view || view.Subviews.Any(v => v == args.View));
            //else
            //    tapDetector.ShouldReceiveTouch = (s, args) => true;

            tapDetector = CreateTapRecognizer(() => (tapCommand,tapPointCommand));
            doubleTapDetector = CreateTapRecognizer(() => (doubleTapCommand, doubleTapPointCommand));
            doubleTapDetector.NumberOfTapsRequired = 2;
            longPressDetector = CreateLongPressRecognizer(() => (longPressCommand, longPressPointCommand));
            panDetector = CreatePanRecognizer(() => (panCommand, panPointCommand));
            pinchDetector = CreatePinchRecognizer(() => pinchCommand);

            swipeLeftDetector = CreateSwipeRecognizer(() => swipeLeftCommand, UISwipeGestureRecognizerDirection.Left);
            swipeRightDetector = CreateSwipeRecognizer(() => swipeRightCommand, UISwipeGestureRecognizerDirection.Right);
            swipeUpDetector = CreateSwipeRecognizer(() => swipeTopCommand, UISwipeGestureRecognizerDirection.Up);
            swipeDownDetector = CreateSwipeRecognizer(() => swipeBottomCommand, UISwipeGestureRecognizerDirection.Down);


            recognizers = new List<UIGestureRecognizer>
            {
                tapDetector, doubleTapDetector, longPressDetector, panDetector, pinchDetector,
                swipeLeftDetector, swipeRightDetector, swipeUpDetector, swipeDownDetector
            };
        }

        private UITapGestureRecognizer CreateTapRecognizer(Func<(ICommand? Command,ICommand? PointCommand)> getCommand)
        {
            return new UITapGestureRecognizer(recognizer =>
            {
                var (command, pointCommand) = getCommand();
                if (command != null || pointCommand != null)
                {
                    var control = Control ?? Container;
                    var point = recognizer.LocationInView(control).ToPoint();
                    if (command?.CanExecute(commandParameter) == true)
                        command.Execute(commandParameter);
                    if(pointCommand?.CanExecute(point) == true)
                        pointCommand.Execute(point);
                }
            })
            {
                Enabled = false,
                ShouldRecognizeSimultaneously = (recognizer, gestureRecognizer) => true,
                //ShouldReceiveTouch = (recognizer, touch) => true,
            };
        }

        private UILongPressGestureRecognizer CreateLongPressRecognizer(Func<(ICommand? Command, ICommand? PointCommand)> getCommand)
        {
            return new UILongPressGestureRecognizer(recognizer =>
            {
                if (recognizer.State == UIGestureRecognizerState.Began)
                {
                    var (command, pointCommand) = getCommand();
                    if (command != null || pointCommand != null)
                    {
                        var control = Control ?? Container;
                        var point = recognizer.LocationInView(control).ToPoint();
                        if (command?.CanExecute(commandParameter) == true)
                            command.Execute(commandParameter);
                        if (pointCommand?.CanExecute(point) == true)
                            pointCommand.Execute(point);
                    }
                }
            })
            {
                Enabled = false,
                ShouldRecognizeSimultaneously = (recognizer, gestureRecognizer) => true,
                //ShouldReceiveTouch = (recognizer, touch) => true,
            };
        }


        private UISwipeGestureRecognizer CreateSwipeRecognizer(Func<ICommand> getCommand, UISwipeGestureRecognizerDirection direction)
        {
            return new UISwipeGestureRecognizer(() =>
            {
                var handler = getCommand();
                if (handler?.CanExecute(commandParameter) == true)
                    handler.Execute(commandParameter);
            })
            {
                Enabled = false,
                ShouldRecognizeSimultaneously = (recognizer, gestureRecognizer) => true,
                Direction = direction
            };
        }
        
        private UIImmediatePinchGestureRecognizer CreatePinchRecognizer(Func<ICommand> getCommand)
        {
            return new UIImmediatePinchGestureRecognizer(recognizer =>
            {
                var command = getCommand();
                if (command != null)
                {
                    var control = Control ?? Container;

                    if (recognizer.NumberOfTouches < 2)
                    {
                        if(recognizer.State == UIGestureRecognizerState.Changed)
                            return;
                    }
                    
                    if(recognizer.State == UIGestureRecognizerState.Began)
                        lastPinch = (Point.Zero, Point.Zero);

                    var current0 = lastPinch.Origin0;
                    var current1 = lastPinch.Origin1;
                    var lastCurrent0 = current0;
                    var lastCurrent1 = current1;
                    if (recognizer.NumberOfTouches >= 1)
                        current0 = lastCurrent0 = recognizer.LocationOfTouch(0, control).ToPoint();
                    if (recognizer.NumberOfTouches >= 2)
                        current1 = lastCurrent1 = recognizer.LocationOfTouch(1, control).ToPoint();
                    else if (recognizer.State == UIGestureRecognizerState.Began)
                        current1 = lastCurrent1 = current0;
                    
                    lastPinch = (lastCurrent0, lastCurrent1);
                    if (recognizer.State == UIGestureRecognizerState.Began)
                        pinchOrigin = (current0, current1);
                    
                    var status = recognizer.State switch
                    {
                        UIGestureRecognizerState.Began => GestureStatus.Started,
                        UIGestureRecognizerState.Changed => GestureStatus.Running,
                        UIGestureRecognizerState.Ended => GestureStatus.Completed,
                        UIGestureRecognizerState.Cancelled => GestureStatus.Canceled,
                        _ => GestureStatus.Canceled,
                    };

                    var parameters = new PinchEventArgs(status, (current0, current1), pinchOrigin);
                    if (command.CanExecute(parameters))
                        command.Execute(parameters);
                }
            })
            {
                Enabled = false,
                ShouldRecognizeSimultaneously = (recognizer, other) => true,
            };
        }

        private UIImmediatePanGestureRecognizer CreatePanRecognizer(Func<(ICommand? Command, ICommand? PointCommand)> getCommand)
        {
            return new UIImmediatePanGestureRecognizer(recognizer =>
            {
                var (command, pointCommand) = getCommand();
                if (command != null || pointCommand != null)
                {
                    if (recognizer.NumberOfTouches > 1 && recognizer.State != UIGestureRecognizerState.Cancelled && recognizer.State != UIGestureRecognizerState.Ended)
                        return;
                    
                    var control = Control ?? Container;
                    var point = recognizer.LocationInView(control).ToPoint();
                    
                    if (command?.CanExecute(commandParameter) == true)
                        command.Execute(commandParameter);

                    if (pointCommand != null && recognizer.State != UIGestureRecognizerState.Began)
                    {
                        //GestureStatus.Started has already been sent by ShouldBegin. Don't sent it twice.

                        var gestureStatus = recognizer.State switch
                        {
                            UIGestureRecognizerState.Began => GestureStatus.Started,
                            UIGestureRecognizerState.Changed => GestureStatus.Running,
                            UIGestureRecognizerState.Ended => GestureStatus.Completed,
                            UIGestureRecognizerState.Cancelled => GestureStatus.Canceled,
                            _ => GestureStatus.Canceled,
                        };
                        
                        var parameter = new PanEventArgs(gestureStatus, point);
                        if (pointCommand.CanExecute(parameter))
                            pointCommand.Execute(parameter);
                    }
                }
            })
            {
                Enabled = false,
                ShouldRecognizeSimultaneously = (recognizer, other) => true,
                MaximumNumberOfTouches = 1,
                ShouldBegin = recognizer =>
                {
                    var (command, pointCommand) = getCommand();
                    if (command != null)
                    {
                        if (command.CanExecute(commandParameter))
                            command.Execute(commandParameter);
                        return true;
                    }
                            
                    if(pointCommand != null)
                    {
                        var control = Control ?? Container;
                        var point = recognizer.LocationInView(control).ToPoint();

                        var parameter = new PanEventArgs(GestureStatus.Started, point);
                        if (pointCommand.CanExecute(parameter))
                            pointCommand.Execute(parameter);
                        if (!parameter.CancelGesture)
                            return true;
                    }

                    return false;
                }
            };
        }

        protected override void OnElementPropertyChanged(PropertyChangedEventArgs args)
        {
            tapCommand = Gesture.GetTapCommand(Element);
            panCommand = Gesture.GetPanCommand(Element);
            panDetector.IsImmediate = Gesture.GetIsPanImmediate(Element);
            pinchCommand = Gesture.GetPinchCommand(Element);
            pinchDetector.IsImmediate = Gesture.GetIsPinchImmediate(Element);
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

            commandParameter = Gesture.GetCommandParameter(Element);
        }

        protected override void OnAttached()
        {
            var control = Control ?? Container;

            foreach (var recognizer in recognizers)
            {
                control.AddGestureRecognizer(recognizer);
                recognizer.Enabled = true;
            }

            OnElementPropertyChanged(new PropertyChangedEventArgs(String.Empty));
        }

        protected override void OnDetached()
        {
            var control = Control ?? Container;
            foreach (var recognizer in recognizers)
            {
                recognizer.Enabled = false;
                control.RemoveGestureRecognizer(recognizer);
            }
        }
    }
}
