using Xamarin.Forms.Xaml;

namespace Prism.DI.Forms.Tests.Mocks.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PartialView
    {
        public PartialView()
        {
            InitializeComponent();
        }

        public void Navigate() =>
            navigateButton.SendClicked();
    }
}