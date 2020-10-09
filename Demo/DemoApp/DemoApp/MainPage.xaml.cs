﻿using System.Windows.Input;
using Xamarin.Forms;

namespace DemoApp
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            BindingContext = new MainPageViewModel(Navigation);
        }
    }

    public class MainPageViewModel : BindableObject
    {
        private readonly INavigation navigation;
        private double panX, panY;

        public double PanX { get => panX; set { panX = value; OnPropertyChanged(); } }
        public double PanY { get => panY; set { panY = value; OnPropertyChanged(); } }

        public MainPageViewModel(INavigation navigation)
        {
            this.navigation = navigation;
        }

        public ICommand OpenVapoliaCommand => new Command(async () =>
        {
            await navigation.PushAsync(new ContentPage {
                Title = "Web",
                Content = new Grid {
                BackgroundColor = Color.Yellow,
                Children = { new WebView { Source = new UrlWebViewSource { Url = "https://vapolia.fr" }, HorizontalOptions = LayoutOptions.Fill, VerticalOptions = LayoutOptions.Fill} }}});
        });

        public ICommand PanPointCommand => new Command<(Point Point,GestureStatus Status)>(args =>
        {
            var point = args.Point;
            PanX = point.X;
            PanY = point.Y;
        });

        public ICommand OpenVapoliaPointCommand => new Command<Point>(point =>
        {
            PanX = point.X;
            PanY = point.Y;
            OpenVapoliaCommand.Execute(null);
        });
    }
}
