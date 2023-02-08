using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace Prism.Maui.Tests.Mocks.Views;

public class ContentPageMock : ContentPage, IInitialize, IInitializeAsync, INavigationAware, IConfirmNavigationAsync, IDestructible, IPageNavigationEventRecordable
{
    public PageNavigationEventRecorder PageNavigationEventRecorder { get; set; }
    public bool OnNavigatedToCalled { get; private set; } = false;
    public bool OnNavigatedFromCalled { get; private set; } = false;
    public bool InitializeCalled { get; private set; } = false;
    public bool InitializeAsyncCalled { get; private set; } = false;

    public bool OnConfirmNavigationCalled { get; private set; } = false;

    public bool DestroyCalled { get; private set; } = false;

    public ContentPageMock() : this(null)
    {
    }

    public ContentPageMock(PageNavigationEventRecorder recorder)
    {
        PageNavigationEventRecorder = recorder;
    }

    protected override void OnBindingContextChanged()
    {
        if (BindingContext is not null && PageNavigationEventRecorder is not null)
            ((IPageNavigationEventRecordable)BindingContext).PageNavigationEventRecorder = PageNavigationEventRecorder;
    }

    public void OnNavigatedFrom(INavigationParameters parameters)
    {
        OnNavigatedFromCalled = true;
        PageNavigationEventRecorder?.Record(this, PageNavigationEvent.OnNavigatedFrom);
    }

    public void OnNavigatedTo(INavigationParameters parameters)
    {
        OnNavigatedToCalled = true;
        PageNavigationEventRecorder?.Record(this, PageNavigationEvent.OnNavigatedTo);
    }

    public void Initialize(INavigationParameters parameters)
    {
        InitializeCalled = true;
        PageNavigationEventRecorder?.Record(this, PageNavigationEvent.OnInitialized);
    }

    public Task InitializeAsync(INavigationParameters parameters)
    {
        InitializeAsyncCalled = true;
        PageNavigationEventRecorder?.Record(this, PageNavigationEvent.OnInitializedAsync);
        return Task.CompletedTask;
    }

    public Task<bool> CanNavigateAsync(INavigationParameters parameters)
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
