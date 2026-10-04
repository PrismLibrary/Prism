using Prism.Navigation;

namespace Prism.Maui.Tests.Mocks.Views;

public class RelativeNavigationPageMock : ContentPage, IInitialize, IInitializeAsync, INavigationAware, IConfirmNavigationAsync, IDestructible
{
    public bool CanNavigate { get; set; } = true;
    public TaskCompletionSource<bool> Confirmation { get; set; }
    public TaskCompletionSource<bool> ConfirmationStarted { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
    public int Confirmations { get; private set; }
    public int Initialized { get; private set; }
    public int NavigatedFromCount { get; private set; }
    public int NavigatedToCount { get; private set; }
    public int Destroyed { get; private set; }
    public bool ThrowOnDestroy { get; set; }
    public List<string> Lifecycle { get; } = new();
    public INavigationParameters FromParameters { get; private set; }
    public INavigationParameters ToParameters { get; private set; }

    public Task<bool> CanNavigateAsync(INavigationParameters parameters)
    {
        Confirmations++;
        ConfirmationStarted.TrySetResult(true);
        return Confirmation?.Task ?? Task.FromResult(CanNavigate);
    }

    public void Initialize(INavigationParameters parameters)
    {
        Initialized++;
        if (parameters.TryGetValue<bool>("throwOnDestroy", out var throwOnDestroy))
            ThrowOnDestroy = throwOnDestroy;
        if (parameters.TryGetValue<Action<RelativeNavigationPageMock>>("initializeObserver", out var observer))
            observer(this);
    }

    public Task InitializeAsync(INavigationParameters parameters)
    {
        if (parameters.TryGetValue<Func<Task>>("initializeAsync", out var initialize))
            return initialize();
        return parameters.TryGetValue<bool>("throwOnInitialize", out var shouldThrow) && shouldThrow
            ? Task.FromException(new InvalidOperationException("Destination initialization failed."))
            : Task.CompletedTask;
    }

    public void OnNavigatedFrom(INavigationParameters parameters)
    {
        NavigatedFromCount++;
        Lifecycle.Add("From");
        FromParameters = parameters;
    }

    public void OnNavigatedTo(INavigationParameters parameters)
    {
        NavigatedToCount++;
        ToParameters = parameters;
    }

    public void Destroy()
    {
        Destroyed++;
        Lifecycle.Add("Destroy");
        if (ThrowOnDestroy)
            throw new InvalidOperationException("Destination cleanup failed.");
    }
}
