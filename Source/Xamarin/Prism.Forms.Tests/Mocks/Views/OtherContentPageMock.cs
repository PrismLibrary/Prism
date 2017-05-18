using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using Prism.Navigation;
using Xamarin.Forms;

namespace Prism.Forms.Tests.Mocks.Views
{
    public class OtherContentPageMock : ContentPage, INavigationAware, IDestructible, IPageNavigationEventRecordable
    {
        public OtherContentPageMock() : this(null)
        {
        }

        public OtherContentPageMock(PageNavigationEventRecorder recorder)
        {
            ViewModelLocator.SetAutowireViewModel(this, true);
            PageNavigationEventRecorder = recorder;
            ((IPageNavigationEventRecordable)BindingContext).PageNavigationEventRecorder = recorder;
        }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {
            PageNavigationEventRecorder?.Record(this, PageNavigationEvent.OnNavigatedFrom);
        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {
            PageNavigationEventRecorder?.Record(this, PageNavigationEvent.OnNavigatedTo);
        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {
            PageNavigationEventRecorder?.Record(this, PageNavigationEvent.OnNavigatingTo);
        }

        public void Destroy()
        {
            PageNavigationEventRecorder?.Record(this, PageNavigationEvent.Destroy);
        }

        public PageNavigationEventRecorder PageNavigationEventRecorder { get; set; }
    }
}
