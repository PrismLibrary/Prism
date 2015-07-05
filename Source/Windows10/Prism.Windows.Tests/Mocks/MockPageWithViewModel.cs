using Windows.UI.Xaml.Controls;

namespace Prism.Windows.Tests.Mocks
{
    public class MockPageWithViewModel : Page
    {
        public MockPageWithViewModel()
        {
            var viewModel = new MockViewModelWithRestorableStateAttributes();
            viewModel.Title = "testtitle";
            this.DataContext = viewModel;
        }
    }
}
