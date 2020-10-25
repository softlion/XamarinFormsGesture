[![Build status](https://ci.appveyor.com/api/projects/status/8t8m8n0do3p0304n?svg=true)](https://ci.appveyor.com/project/softlion/xamarinformsgesture)

[![NuGet](https://img.shields.io/nuget/v/Vapolia.XamarinFormsGesture.svg?style=for-the-badge)](https://www.nuget.org/packages/Vapolia.XamarinFormsGesture/)  
[![NuGet](https://img.shields.io/nuget/vpre/Vapolia.XamarinFormsGesture.svg?style=for-the-badge)](https://www.nuget.org/packages/Vapolia.XamarinFormsGesture/)  
![Nuget](https://img.shields.io/nuget/dt/Vapolia.XamarinFormsGesture)

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
 *  `TapPointCommand (ICommand or Command<Point>)` where point is the absolute tap position relative to the view
 *  `DoubleTapPoinCommand (ICommand or Command<Point>)` where point is the absolute double tap position relative to the view
 *  `PanPointCommand (ICommand or Command<(Point,GestureStatus)>)` where point is the absolute position relative to the view
 *  `LongPressPointCommand (ICommand or Command<Point>) ` where point is the absolute tap position relative to the view
 *  `SwipeLeftCommand`
 *  `SwipeRightCommand`
 *  `SwipeTopCommand`
 *  `SwipeBottomCommand`
 *  `PinchCommand` (Command<PinchEventArgs>) where PinchEventArgs contains StartingPoints, CurrentPoints, Center, Scale, Rotation (radians), Status
 
 Properties:
 
 * `IsPanImmediate` Set to true to receive the PanCommand or PanPointCommand event on touch down, instead of after a minimum move distance. Default to false.
 
# Examples

```xml
<StackLayout ui:Gesture.TapCommand="{Binding OpenCommand}" IsEnabled="True">
    <Label Text="1.Tap this text to open an url" />
</StackLayout>

<StackLayout ui:Gesture.DoubleTapPointCommand="{Binding OpenPointCommand}" IsEnabled="True">
    <Label Text="2.Double tap this text to open an url" />
</StackLayout>

<BoxView
    ui:Gesture.PanPointCommand="{Binding PanPointCommand}"
    HeightRequest="200" WidthRequest="300"
    InputTransparent="False"
    IsEnabled="True"
     />
```

In the viewmodel:

```csharp
public ICommand OpenCommand => new Command(async () =>
{
   //...
});

public ICommand OpenPointCommand => new Command<Point>(point =>
{
    PanX = point.X;
    PanY = point.Y;
    //...
});

public ICommand PanPointCommand => new Command<(Point Point,GestureStatus Status)>(args =>
{
    var point = args.Point;
    PanX = point.X;
    PanY = point.Y;
    //...
});

``` 
        

# Limitations

Only commands are supported (PR welcome for events). No .NET events. 
So you must use the MVVM pattern (https://developer.xamarin.com/guides/xamarin-forms/xaml/xaml-basics/data_bindings_to_mvvm/).

Swipe commands are not supported on UWP due to a bug (event not received). If you find it, notify me!
PinchCommand is not supported (yet) on UWP. PR welcome.

If your command is not receiving events, make sure that:
- you used the correct handler. Ie: the `LongPressPointCommand` should be `new Command<Point>(pt => ...)`
- you set IsEnabled="True" and InputTransparent="False" on the element

UWP requires fall creator update  

# Breaking changes

Version 3.3.0 has breaking changes:  
- Command names have changed
- PanPointCommand returns an absolute position, not a relative position anymore. It also returns the gesture state.
