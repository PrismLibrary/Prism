using Prism.Mvvm;
using Prism.Navigation;
using Xamarin.Forms;

namespace Prism.Forms.Tests.Mocks.Views
{
    public class TabbedPageMock : TabbedPage, IDestructible
    {
        public bool DestroyCalled { get; private set; } = false;

        public TabbedPageMock()
        {
            ViewModelLocator.SetAutowireViewModel(this, true);

            Children.Add(new ContentPageMock() { Title = "Page 1" });
            Children.Add(new PageMock() { Title = "Page 2" });
            Children.Add(new ContentPageMock() { Title = "Page 3" });
        }

        public void Destroy()
        {
            DestroyCalled = true;
            PageNavigationEventRecoder.Record(this, PageNavigationEvent.Destroy);
        }
    }
}
