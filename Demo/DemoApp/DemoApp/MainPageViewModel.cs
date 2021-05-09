using System;
using System.Windows.Input;
using Vapolia.Lib.Ui;
using Xamarin.Forms;

namespace DemoApp
{
    public class MainPageViewModel : BindableObject
    {
        private readonly INavigation navigation;
        private Point pan, pinch;
        private double rotation, scale;

        public Point Pan { get => pan; set { pan = value; OnPropertyChanged(); } }
        public Point Pinch { get => pinch; set { pinch = value; OnPropertyChanged(); } }
        public double Rotation { get => rotation; set { rotation = value; OnPropertyChanged(); } }
        public double Scale { get => scale; set { scale = value; OnPropertyChanged(); } }

        public MainPageViewModel(INavigation navigation)
        {
            this.navigation = navigation;
        }

        
        public ICommand PanPointCommand => new Command<PanEventArgs>(args =>
        {
            var point = args.Point;
            Pan = point;
        });
        
        public ICommand PinchCommand => new Command<PinchEventArgs>(args =>
        {
            Pinch = args.Center;
            Rotation = args.RotationDegrees;
            Scale = args.Scale;
        });

        public ICommand OpenVapoliaCommand => new Command(async () =>
        {
            await navigation.PushAsync(new ContentPage {
                Title = "Web",
                Content = new Grid {
                    BackgroundColor = Color.Yellow,
                    Children = { new WebView { Source = new UrlWebViewSource { Url = "https://vapolia.fr" }, HorizontalOptions = LayoutOptions.Fill, VerticalOptions = LayoutOptions.Fill} }}});
        });

        
        public ICommand OpenVapoliaPointCommand => new Command<Point>(point =>
        {
            Pan = point;
            OpenVapoliaCommand.Execute(null);
        });
    }
}
