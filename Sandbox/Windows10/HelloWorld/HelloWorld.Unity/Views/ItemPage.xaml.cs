using Windows.UI.Xaml.Controls;

namespace Sample.Views
{
    public sealed partial class ItemPage : Page
    {
        public ItemPage()
        {
            InitializeComponent();
        }

        ViewModels.ItemPageViewModel ViewModel => DataContext as ViewModels.ItemPageViewModel;
    }
}
