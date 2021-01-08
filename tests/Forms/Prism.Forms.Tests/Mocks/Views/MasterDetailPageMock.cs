using System;
using Prism.Mvvm;
using Prism.Navigation;
using Xamarin.Forms;

namespace Prism.Forms.Tests.Mocks.Views
{
    public class MasterDetailPageMock : FlyoutPage, IMasterDetailPageOptions, IDestructible, IPageNavigationEventRecordable
    {
        public PageNavigationEventRecorder PageNavigationEventRecorder { get; set; }

        public MasterDetailPageMock() : this(null)
        {
        }

        public MasterDetailPageMock(PageNavigationEventRecorder recorder)
        {
            Flyout = new ContentPageMock(recorder) { Title = "Master" };
            Detail = new ContentPageMock(recorder);

            ViewModelLocator.SetAutowireViewModel(this, true);

            PageNavigationEventRecorder = recorder;
            ((IPageNavigationEventRecordable)BindingContext).PageNavigationEventRecorder = recorder;
        }

        public MasterDetailPageMock(PageNavigationEventRecorder recorder, Page masterPage, Page detailPage)
        {
            Flyout = masterPage;
            Detail = detailPage;

            ViewModelLocator.SetAutowireViewModel(this, true);

            PageNavigationEventRecorder = recorder;
            ((IPageNavigationEventRecordable)BindingContext).PageNavigationEventRecorder = recorder;
        }

        public bool IsPresentedAfterNavigation { get; set; }
        public void Destroy()
        {
            PageNavigationEventRecorder.Record(this, PageNavigationEvent.Destroy);
        }
    }

    public class MasterDetailPageEmptyMock : FlyoutPage
    {
        public MasterDetailPageEmptyMock()
        {
            ViewModelLocator.SetAutowireViewModel(this, true);
            Flyout = new ContentPageMock { Title = "Master" };
        }
    }
}
