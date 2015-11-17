using Prism.Mvvm;
using Xamarin.Forms;

namespace Prism.Forms.Tests.Mocks.Views
{
    public class TabbedPageMock : TabbedPage
    {
        public TabbedPageMock()
        {
            ViewModelLocator.SetAutowireViewModel(this, true);

            Children.Add(new ContentPageMock() { Title = "Page 1" });
            Children.Add(new PageMock() { Title = "Page 2" });
            Children.Add(new ContentPageMock() { Title = "Page 3" });
        }
    }
}
