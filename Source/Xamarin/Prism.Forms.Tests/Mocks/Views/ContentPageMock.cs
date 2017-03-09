using System;
using Prism.Mvvm;
using Prism.Navigation;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace Prism.Forms.Tests.Mocks.Views
{
    public class ContentPageMock : ContentPage, INavigationAware, IConfirmNavigationAsync, IDestructible, IPageNavigationEventRecordable
    {
        public PageNavigationEventRecorder PageNavigationEventRecorder { get; set; }
        public bool OnNavigatedToCalled { get; private set; } = false;
        public bool OnNavigatedFromCalled { get; private set; } = false;
        public bool OnNavigatingToCalled { get; private set; } = false;

        public bool OnConfirmNavigationCalled { get; private set; } = false;

        public bool DestroyCalled { get; private set; } = false;

        public ContentPageMock() : this(null)
        {
        }

        public ContentPageMock(PageNavigationEventRecorder recorder)
        {
            ViewModelLocator.SetAutowireViewModel(this, true);

            PageNavigationEventRecorder = recorder;
            ((IPageNavigationEventRecordable)BindingContext).PageNavigationEventRecorder = recorder;
        }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {
            OnNavigatedFromCalled = true;
            PageNavigationEventRecorder?.Record(this, PageNavigationEvent.OnNavigatedFrom);
        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {
            OnNavigatedToCalled = true;
            PageNavigationEventRecorder?.Record(this, PageNavigationEvent.OnNavigatedTo);
        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {
            OnNavigatingToCalled = true;
            PageNavigationEventRecorder?.Record(this, PageNavigationEvent.OnNavigatingTo);
        }

        public Task<bool> CanNavigateAsync(NavigationParameters parameters)
        {
            return Task.Run(() =>
            {
                OnConfirmNavigationCalled = true;

                if (parameters.ContainsKey("canNavigate"))
                    return (bool)parameters["canNavigate"];

                return true;
            });
        }

        public void Destroy()
        {
            DestroyCalled = true;
            PageNavigationEventRecorder?.Record(this, PageNavigationEvent.Destroy);
        }
    }
}
