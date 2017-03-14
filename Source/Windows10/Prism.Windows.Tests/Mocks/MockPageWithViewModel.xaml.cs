
namespace Prism.Windows.Tests.Mocks
{
    public partial class MockPageWithViewModel
    {
        public MockPageWithViewModel()
        {
            var viewModel = new MockViewModelWithRestorableStateAttributes();
            viewModel.Title = "testtitle";
            this.DataContext = viewModel;
        }
    }
}
