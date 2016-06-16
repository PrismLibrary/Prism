using Prism.Mvvm;
using Xamarin.Forms;

namespace Prism.Forms.Tests.Mocks.Views
{
    public class CustomTabbedPageMock : MultiPage<Page>
    {
        public CustomTabbedPageMock()
        {
            ViewModelLocator.SetAutowireViewModel(this, true);
        }

        protected override Page CreateDefault(object item)
        {
            return new ContentPageMock();
        }
    }
}