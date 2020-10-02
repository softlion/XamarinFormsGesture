using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using UIKit;
using Vapolia.Ios.Lib.Effects;
using Vapolia.Lib.Ui;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.iOS;

[assembly: ResolutionGroupName("Vapolia")]
[assembly: ExportEffect(typeof(PlatformGestureEffect), nameof(PlatformGestureEffect))]

namespace Vapolia.Ios.Lib.Effects
{

    [Preserve (Conditional=true, AllMembers = true)]
    public class PlatformGestureEffect : PlatformEffect
    {
        private readonly UITapGestureRecognizer tapDetector, doubleTapDetector;
        private readonly UILongPressGestureRecognizer longPressDetector;
        private readonly UISwipeGestureRecognizer swipeLeftDetector, swipeRightDetector, swipeUpDetector, swipeDownDetector;
        private readonly UIPanGestureRecognizer panDetector;
        private readonly List<UIGestureRecognizer> recognizers;

        private Command<Point> tapPointCommand, panPointCommand, doubleTapPointCommand, longPressPointCommand;
        private ICommand tapCommand, panCommand, doubleTapCommand, longPressCommand, swipeLeftCommand, swipeRightCommand, swipeTopCommand, swipeBottomCommand;
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

            tapDetector = CreateTapRecognizer(() => Tuple.Create(tapCommand,tapPointCommand));
            doubleTapDetector = CreateTapRecognizer(() => Tuple.Create(doubleTapCommand, doubleTapPointCommand));
            doubleTapDetector.NumberOfTapsRequired = 2;
            longPressDetector = CreateLongPressRecognizer(() => Tuple.Create(longPressCommand, longPressPointCommand));
            panDetector = CreatePanRecognizer(() => Tuple.Create(panCommand, panPointCommand));

            swipeLeftDetector = CreateSwipeRecognizer(() => swipeLeftCommand, UISwipeGestureRecognizerDirection.Left);
            swipeRightDetector = CreateSwipeRecognizer(() => swipeRightCommand, UISwipeGestureRecognizerDirection.Right);
            swipeUpDetector = CreateSwipeRecognizer(() => swipeTopCommand, UISwipeGestureRecognizerDirection.Up);
            swipeDownDetector = CreateSwipeRecognizer(() => swipeBottomCommand, UISwipeGestureRecognizerDirection.Down);


            recognizers = new List<UIGestureRecognizer>
            {
                tapDetector, doubleTapDetector, longPressDetector, panDetector,
                swipeLeftDetector, swipeRightDetector, swipeUpDetector, swipeDownDetector
            };
        }

        private UITapGestureRecognizer CreateTapRecognizer(Func<Tuple<ICommand,Command<Point>>> getCommand)
        {
            return new UITapGestureRecognizer(recognizer =>
            {
                var handler = getCommand();
                if (handler != null)
                {
                    var control = Control ?? Container;
                    var point = recognizer.LocationInView(control);
                    var pt = new Point(point.X, point.Y);
                    if (handler.Item2?.CanExecute(pt) == true)
                        handler.Item2.Execute(pt);
                    if(handler.Item1?.CanExecute(commandParameter) == true)
                        handler.Item1.Execute(commandParameter);
                }
            })
            {
                Enabled = false,
                ShouldRecognizeSimultaneously = (recognizer, gestureRecognizer) => true,
                //ShouldReceiveTouch = (recognizer, touch) => true,
            };
        }

        private UILongPressGestureRecognizer CreateLongPressRecognizer(Func<Tuple<ICommand, Command<Point>>> getCommand)
        {
            return new UILongPressGestureRecognizer(recognizer =>
            {
                if (recognizer.State == UIGestureRecognizerState.Began)
                {
                    var handler = getCommand();
                    if (handler != null)
                    {
                        var control = Control ?? Container;
                        var point = recognizer.LocationInView(control);
                        var pt = new Point(point.X, point.Y);
                        if (handler.Item2?.CanExecute(pt) == true)
                            handler.Item2.Execute(pt);
                        if (handler.Item1?.CanExecute(commandParameter) == true)
                            handler.Item1.Execute(commandParameter);
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

        private UIPanGestureRecognizer CreatePanRecognizer(Func<Tuple<ICommand, Command<Point>>> getCommand)
        {
            return new UIPanGestureRecognizer(recognizer =>
            {
                var handler = getCommand();
                if (handler != null)
                {
                    var control = Control ?? Container;
                    var point = recognizer.TranslationInView(control);
                    var pt = new Point(point.X, point.Y);
                    if (handler.Item2?.CanExecute(pt) == true)
                        handler.Item2.Execute(pt);
                    if (handler.Item1?.CanExecute(commandParameter) == true)
                        handler.Item1.Execute(commandParameter);
                }
            })
            {
                Enabled = false,
                ShouldRecognizeSimultaneously = (recognizer, gestureRecognizer) => true,
                MaximumNumberOfTouches = 1,
            };
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
