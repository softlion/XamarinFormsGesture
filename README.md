[![Build status](https://ci.appveyor.com/api/projects/status/8t8m8n0do3p0304n?svg=true)](https://ci.appveyor.com/project/softlion/xamarinformsgesture)

[![NuGet](https://img.shields.io/nuget/v/Vapolia.XamarinFormsGesture.svg?style=for-the-badge)](https://www.nuget.org/packages/Vapolia.XamarinFormsGesture/)

# Supported Platforms

iOS, Android, UWP

# Xamarin Form Gesture Effects

Add "advanced" gestures to Xamarin Forms. Available on all views.
Most gesture commands include the event position.

```xaml
    <Label Text="Click here" IsEnabled="True" ui:Gesture.TapCommand="{Binding OpenLinkCommand}" />
```
Or in code:
```csharp
    var label = new Label();
    Vapolia.Lib.Ui.Gesture.SetTapCommand(label, new Command(() => { /*your code*/ }));
```

# Quick start
Add the above nuget package to your Xamarin Forms project (only the netstandard one is enough).

In your platform projects (android,ios,uwp), before initializing xamarin forms, call `PlatformGestureEffect.Init()` to force the discovery of this extension by the Xamarin Forms plugin engine.

The views on which the gesture is applied should have the property **IsEnabled="True"** and **InputTransparent="False"** which activates user interaction on them.

# Examples

Add Gesture.TapCommand on any supported xaml view:
```xaml
        <StackLayout ui:Gesture.TapCommand="{Binding OpenLinkCommand}">
            <Label Text="1.Tap this to open an url"  />
        </StackLayout>
```
Declare the corresponding namespace:
```xaml
    <ContentPage xmlns="http://xamarin.com/schemas/2014/forms"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             ...
             xmlns:ui="clr-namespace:Vapolia.Lib.Ui;assembly=XamarinFormsGesture"
    >
```
And in the viewmodel:
   ```csharp     
    public Command OpenLinkCommand => new Command(() =>
    {
        //do something
    });
```
# Supported Gestures

 *  `TapCommand (ICommand)`
 *  `DoubleTapCommand (ICommand)`
 *  `PanCommand (ICommand)`
 *  `LongPressCommand (ICommand)`
 *  `TapPointCommand (Command<Point>)` where point is the tap position in the view
 *  `DoubleTapPoinCommand (Command<Point>)` where point is the double tap position in the view
 *  `PanPointCommand (Command<Point>) `where point is the translation in the view from the start point of the pan gesture
 *  `LongPressPointCommand (Command<Point>) ` where point is the tap position in the view
 *  `SwipeLeftCommand`
 *  `SwipeRightCommand`
 *  `SwipeTopCommand`
 *  `SwipeBottomCommand`

# Limitations

Only commands are supported (PR welcome for events). No .NET events. 
So you must use the MVVM pattern (https://developer.xamarin.com/guides/xamarin-forms/xaml/xaml-basics/data_bindings_to_mvvm/).

Swipe commands are not supported on UWP due to a bug (event not received). If you find it, notify me!

If your command is not receiving events, make sure that:
- you used the correct handler. Ie: the `LongPressPointCommand` must be `new Command<Point>(pt => ...)` and won't work with `new Command(() => ...)`.
- you set IsEnabled="True" and InputTransparent="False" on the element

UWP requires fall creator update  
