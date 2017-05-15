using Prism.Mvvm;
using Prism.Navigation;
using Xamarin.Forms;

namespace Prism.Forms.Tests.Mocks.Views
{
    public class PageMock : ContentPageMock
    {
        public PageMock() : this(null)
        {
        }

        public PageMock(PageNavigationEventRecorder recorder) : base(recorder)
        {
            ViewModelLocator.SetAutowireViewModel(this, true);
        }
    }
}
