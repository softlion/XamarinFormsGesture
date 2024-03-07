using System;
using System.ComponentModel;
using System.Windows.Input;
using Vapolia.Lib.Ui;

#if IOS
using Xamarin.Forms.Platform.iOS;
#endif

namespace Vapolia.Lib.Effects;

public partial class PlatformGestureEffect
{
    private object? commandParameter;
    
    /// <summary>
    /// Takes a Point parameter
    /// Except panPointCommand which takes a (Point,GestureStatus) parameter (it's a tuple) 
    /// </summary>
    private ICommand? tapPointCommand, panPointCommand, doubleTapPointCommand, longPressPointCommand;
    
    /// <summary>
    /// Takes a CommandParameter parameter
    /// </summary>
    private ICommand? tapCommand, panCommand, doubleTapCommand, longPressCommand, swipeLeftCommand, swipeRightCommand, swipeTopCommand, swipeBottomCommand;
    
    /// <summary>
    /// 1 parameter: PinchEventArgs
    /// </summary>
    private ICommand? pinchCommand;
    
    protected override void OnElementPropertyChanged(PropertyChangedEventArgs args)
    {
        tapCommand = Gesture.GetTapCommand(Element);
        panCommand = Gesture.GetPanCommand(Element);

        pinchCommand = Gesture.GetPinchCommand(Element);
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
        
#if IOS
        panDetector.IsImmediate = Gesture.GetIsPanImmediate(Element);
        pinchDetector.IsImmediate = Gesture.GetIsPinchImmediate(Element);
#endif
    }
}