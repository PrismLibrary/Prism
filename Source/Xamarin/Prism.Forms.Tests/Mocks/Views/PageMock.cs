using System.Threading.Tasks;
using Prism.Mvvm;
using Prism.Navigation;
using Xamarin.Forms;

namespace Prism.Forms.Tests.Mocks.Views
{
    public class PageMock : Page, INavigationAware, IConfirmNavigationAsync, IDestructible, IPageNavigationEventRecordable
    {
        public PageNavigationEventRecorder PageNavigationEventRecorder { get; set; }

        public PageMock() : this(null)
        {
            
        }

        public PageMock(PageNavigationEventRecorder recorder)
        {
            PageNavigationEventRecorder = recorder;
        }

        public void OnNavigatedFrom(INavigationParameters parameters)
        {
            PageNavigationEventRecorder?.Record(this, PageNavigationEvent.OnNavigatedFrom);
        }

        public void OnNavigatedTo(INavigationParameters parameters)
        {
            PageNavigationEventRecorder?.Record(this, PageNavigationEvent.OnNavigatedTo);
        }

        public void OnNavigatingTo(INavigationParameters parameters)
        {
            PageNavigationEventRecorder?.Record(this, PageNavigationEvent.OnNavigatingTo);
        }

        public Task<bool> CanNavigateAsync(INavigationParameters parameters)
        {
            return Task.Run(() =>
            {
                if (parameters.ContainsKey("canNavigate"))
                    return (bool)parameters["canNavigate"];

                return true;
            });
        }

        public void Destroy()
        {
            PageNavigationEventRecorder?.Record(this, PageNavigationEvent.Destroy);
        }
    }
}
