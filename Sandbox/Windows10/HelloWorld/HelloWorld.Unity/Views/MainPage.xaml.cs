using Windows.UI.Xaml.Controls;

namespace Sample.Views
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
        }

        ViewModels.MainPageViewModel ViewModel => DataContext as ViewModels.MainPageViewModel;
    }
}
