using Prism.Mvvm;
using Prism.Navigation;
using Xamarin.Forms;

namespace Prism.Forms.Tests.Mocks.Views
{
    public class CarouselPageMock : CarouselPage, IDestructible, IPageNavigationEventRecordable
    {
        public PageNavigationEventRecorder PageNavigationEventRecorder { get; set; }

        public CarouselPageMock() : this(null)
        {
        }

        public CarouselPageMock(PageNavigationEventRecorder recorder)
        {
            ViewModelLocator.SetAutowireViewModel(this, true);

            Children.Add(new ContentPage() { Title = "Page 1" });
            Children.Add(new ContentPageMock(recorder) { Title = "Page 2" });
            Children.Add(new ContentPage() { Title = "Page 3" });

            PageNavigationEventRecorder = recorder;
            ((IPageNavigationEventRecordable) BindingContext).PageNavigationEventRecorder = recorder;
        }

        public void Destroy()
        {
            PageNavigationEventRecorder?.Record(this, PageNavigationEvent.Destroy);
        }
    }
}
