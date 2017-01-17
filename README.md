# XamarinFormsGesture
Xamarin Form Gesture Effects  
Already available in nuget.

Add support for all platform gestures in Xamarin Forms. Available on all views. Sample use:

    <Label Text="Click here" Style="{StaticResource LinkLabel}" ui:Gesture.TapCommand="{Binding OpenLinkCommand}" />

# Quick start

Add Gesture.TapCommand on any xaml view:

    <Label Text="Click here" Style="{StaticResource LinkLabel}" ui:Gesture.TapCommand="{Binding OpenLinkCommand}" />

Declare the corresponding namespace:

    <ContentPage xmlns="http://xamarin.com/schemas/2014/forms"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             ...
             xmlns:ui="clr-namespace:Vapolia.Lib.Ui;assembly=XamarinFormsGesture"
    >

Supports iOS and Android.

# Doc

Supported Gestures:

TapCommand (ICommand)
TapCommand2 (Command<Point>)
SwipeLeftCommand
SwipeRightCommand
SwipeTopCommand
SwipeBottomCommand

Only commands are supported. No .NET handler. So you must use the MVVM pattern (https://developer.xamarin.com/guides/xamarin-forms/xaml/xaml-basics/data_bindings_to_mvvm/).
