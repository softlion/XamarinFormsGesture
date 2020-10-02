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

        private ICommand tapCommand, swipeLeftCommand, swipeRightCommand, swipeTopCommand, swipeBottomCommand;
        private Command<Point> tapPointCommand, panCommand, doubleTapCommand, longPressCommand;

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
            doubleTapDetector = CreateTapRecognizer(() => Tuple.Create((ICommand)null, doubleTapCommand));
            doubleTapDetector.NumberOfTapsRequired = 2;
            longPressDetector = CreateLongPressRecognizer(() => Tuple.Create((ICommand)null, longPressCommand));

            swipeLeftDetector = CreateSwipeRecognizer(() => swipeLeftCommand, UISwipeGestureRecognizerDirection.Left);
            swipeRightDetector = CreateSwipeRecognizer(() => swipeRightCommand, UISwipeGestureRecognizerDirection.Right);
            swipeUpDetector = CreateSwipeRecognizer(() => swipeTopCommand, UISwipeGestureRecognizerDirection.Up);
            swipeDownDetector = CreateSwipeRecognizer(() => swipeBottomCommand, UISwipeGestureRecognizerDirection.Down);

            panDetector = CreatePanRecognizer(() => panCommand);

            recognizers = new List<UIGestureRecognizer>
            {
                tapDetector, doubleTapDetector, longPressDetector,
                swipeLeftDetector, swipeRightDetector, swipeUpDetector, swipeDownDetector,
                panDetector
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
                    if(handler.Item1?.CanExecute(null) == true)
                        handler.Item1.Execute(null);
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
                        if (handler.Item1?.CanExecute(null) == true)
                            handler.Item1.Execute(null);
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
                if (handler?.CanExecute(null) == true)
                    handler.Execute(null);
            })
            {
                Enabled = false,
                ShouldRecognizeSimultaneously = (recognizer, gestureRecognizer) => true,
                Direction = direction
            };
        }

        private UIPanGestureRecognizer CreatePanRecognizer(Func<Command<Point>> getCommand)
        {
            return new UIPanGestureRecognizer(recognizer =>
            {
                var handler = getCommand();
                if (handler != null)
                {
                    var control = Control ?? Container;
                    var point = recognizer.TranslationInView(control);
                    var pt = new Point(point.X, point.Y);
                    if (handler.CanExecute(pt))
                        handler.Execute(pt);
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
            tapPointCommand = Gesture.GetTapPointCommand(Element);
            doubleTapCommand = Gesture.GetDoubleTapPointCommand(Element);
            longPressCommand = Gesture.GetLongPressPointCommand(Element);

            swipeLeftCommand = Gesture.GetSwipeLeftCommand(Element);
            swipeRightCommand = Gesture.GetSwipeRightCommand(Element);
            swipeTopCommand = Gesture.GetSwipeTopCommand(Element);
            swipeBottomCommand = Gesture.GetSwipeBottomCommand(Element);
            panCommand = Gesture.GetPanPointCommand(Element);
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
