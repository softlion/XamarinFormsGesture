using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public double PanX { get; set; }
        public double PanY { get; set; }

        public MainPageViewModel(INavigation navigation)
        {
            this.navigation = navigation;
        }

        public Command OpenVapoliaCommand => new Command(async () =>
        {
            await navigation.PushAsync(new ContentPage {
                Title = "Web",
                Content = new Grid {
                BackgroundColor = Color.Yellow,
                Children = { new WebView { Source = new UrlWebViewSource { Url = "https://vapolia.fr" }, HorizontalOptions = LayoutOptions.Fill, VerticalOptions = LayoutOptions.Fill} }}});
        });

        public Command<Point> PanCommand => new Command<Point>(point =>
        {
            PanX = point.X;
            PanY = point.Y;
            OnPropertyChanged(nameof(PanX));
            OnPropertyChanged(nameof(PanY));
        });
    }
}
