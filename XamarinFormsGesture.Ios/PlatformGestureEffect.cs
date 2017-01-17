// Do not remove this notice
// Copyright (c)2016 Vapolia. All rights reserved.
// Usage licence automatically acquired by Vapolia's customers when added to their product's source code under a contract signed with Vapolia.

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
    internal class PlatformGestureEffect : PlatformEffect
    {
        private readonly UITapGestureRecognizer tapDetector;
        private readonly UISwipeGestureRecognizer swipeLeftDetector, swipeRightDetector, swipeUpDetector, swipeDownDetector;
        private readonly List<UIGestureRecognizer> recognizers;

        private Command<Point> tapCommand2;
        private ICommand tapCommand, swipeLeftCommand, swipeRightCommand, swipeTopCommand, swipeBottomCommand;

        public PlatformGestureEffect()
        {
            //if (!allSubviews)
            //    tapDetector.ShouldReceiveTouch = (s, args) => args.View != null && (args.View == view || view.Subviews.Any(v => v == args.View));
            //else
            //    tapDetector.ShouldReceiveTouch = (s, args) => true;

            tapDetector = CreateTapRecognizer(() => Tuple.Create(tapCommand,tapCommand2));

            swipeLeftDetector = CreateSwipeRecognizer(() => swipeLeftCommand, UISwipeGestureRecognizerDirection.Left);
            swipeRightDetector = CreateSwipeRecognizer(() => swipeRightCommand, UISwipeGestureRecognizerDirection.Right);
            swipeUpDetector = CreateSwipeRecognizer(() => swipeTopCommand, UISwipeGestureRecognizerDirection.Up);
            swipeDownDetector = CreateSwipeRecognizer(() => swipeBottomCommand, UISwipeGestureRecognizerDirection.Down);

            recognizers = new List<UIGestureRecognizer>
            {
                tapDetector,
                swipeLeftDetector, swipeRightDetector, swipeUpDetector, swipeDownDetector,
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
                    var tapPoint = recognizer.LocationInView(control);
                    var point = PxToDp(new Point(tapPoint.X, tapPoint.Y));
                    //Log.WriteLine(LogPriority.Debug, "gesture", $"Tap detected at {x} x {y} in forms: {point.X} x {point.Y}");

                    if (handler.Item2?.CanExecute(point) == true)
                        handler.Item2.Execute(point);
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
                //ShouldReceiveTouch = (recognizer, touch) => true,
                Direction = direction
            };
        }

        private Point PxToDp(Point point)
        {
            //point.X = point.X / displayMetrics.Density;
            //point.Y = point.Y / displayMetrics.Density;
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
