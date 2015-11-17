using Prism.Mvvm;
using Xamarin.Forms;

namespace Prism.Forms.Tests.Mocks.Views
{
    public class MasterDetailPageMock : MasterDetailPage
    {
        public MasterDetailPageMock()
        {
            Detail = new ContentPageMock();

            ViewModelLocator.SetAutowireViewModel(this, true);
        }
    }

    public class MasterDetailPageEmptyMock : MasterDetailPage
    {
        public MasterDetailPageEmptyMock()
        {

        }
    }
}
