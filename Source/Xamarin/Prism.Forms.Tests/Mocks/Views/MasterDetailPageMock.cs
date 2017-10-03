using System;
using Prism.Mvvm;
using Prism.Navigation;
using Xamarin.Forms;

namespace Prism.Forms.Tests.Mocks.Views
{
    public class MasterDetailPageMock : MasterDetailPage, IMasterDetailPageOptions, IDestructible, IPageNavigationEventRecordable
    {
        public PageNavigationEventRecorder PageNavigationEventRecorder { get; set; }

        public MasterDetailPageMock() : this(null)
        {
        }

        public MasterDetailPageMock(PageNavigationEventRecorder recorder)
        {
            Master = new ContentPageMock(recorder) { Title = "Master" };
            Detail = new ContentPageMock(recorder);

            ViewModelLocator.SetAutowireViewModel(this, true);

            PageNavigationEventRecorder = recorder;
            SetRecorder(recorder);
        }

        public MasterDetailPageMock(PageNavigationEventRecorder recorder, Page masterPage, Page detailPage)
        {
            Master = masterPage;
            Detail = detailPage;

            ViewModelLocator.SetAutowireViewModel(this, true);

            PageNavigationEventRecorder = recorder;
            SetRecorder(recorder);
        }

        public bool IsPresentedAfterNavigation { get; set; }
        public void Destroy()
        {
            PageNavigationEventRecorder.Record(this, PageNavigationEvent.Destroy);
        }

        void SetRecorder(PageNavigationEventRecorder recorder)
        {
            var recordable = BindingContext as IPageNavigationEventRecordable;
            if (recordable != null)
                recordable.PageNavigationEventRecorder = recorder;
        }
    }

    public class MasterDetailPageEmptyMock : MasterDetailPage
    {
        public MasterDetailPageEmptyMock()
        {
            ViewModelLocator.SetAutowireViewModel(this, true);
            Master = new ContentPageMock {Title = "Master"};
        }
    }
}
