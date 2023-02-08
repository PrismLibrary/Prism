using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace Prism.Maui.Tests.Mocks.Views;

public class PageMock : Page, IInitialize, INavigationAware, IConfirmNavigationAsync, IDestructible, IPageNavigationEventRecordable
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

    public void Initialize(INavigationParameters parameters)
    {
        PageNavigationEventRecorder?.Record(this, PageNavigationEvent.OnInitialized);
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
