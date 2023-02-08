using Microsoft.Maui.Controls;

namespace Prism.Maui.Tests.Mocks.Views;

public class NavigationPageMock : NavigationPage, IDestructible, IPageNavigationEventRecordable, INavigationPageOptions, IInitialize, INavigationAware
{
    public bool DestroyCalled { get; private set; } = false;
    public PageNavigationEventRecorder PageNavigationEventRecorder { get; set; }

    public NavigationPageMock() : this(null)
    {
    }

    public NavigationPageMock(PageNavigationEventRecorder recorder) : this(recorder, new ContentPageMock(recorder))
    {
    }

    public NavigationPageMock(PageNavigationEventRecorder recorder, Page page) : base(page)
    {
        //ViewModelLocator.SetAutowireViewModel(this, true);

        PageNavigationEventRecorder = recorder;
        ((IPageNavigationEventRecordable)BindingContext).PageNavigationEventRecorder = recorder;
    }

    public void Destroy()
    {
        DestroyCalled = true;
        PageNavigationEventRecorder?.Record(this, PageNavigationEvent.Destroy);
    }

    public bool ClearNavigationStackOnNavigation { get; set; } = true;
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
}
