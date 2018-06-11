using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Prism.DI.Forms.Tests.Mocks.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class XamlViewMockB : ContentPage
    {
        public XamlViewMockB()
        {
            InitializeComponent();
        }

        public Button TestButton => this.testButton;
    }
}