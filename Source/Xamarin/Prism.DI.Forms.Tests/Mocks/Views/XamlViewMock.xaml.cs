using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Prism.DI.Forms.Tests.Mocks.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class XamlViewMock : ContentPage
    {
        public XamlViewMock()
        {
            InitializeComponent();
        }

        public Entry TestEntry => this.testEntry;
    }
}