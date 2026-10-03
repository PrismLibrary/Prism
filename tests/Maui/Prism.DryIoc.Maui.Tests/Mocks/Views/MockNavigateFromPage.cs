namespace Prism.DryIoc.Maui.Tests.Mocks.Views;

public sealed class MockNavigateFromPage : ContentPage, IInitialize, IInitializeAsync, IConfirmNavigationAsync, INavigationAware, IDestructible
{
    public bool AllowNavigation { get; set; } = true;
    public TaskCompletionSource<bool> Decision { get; set; }
    public TaskCompletionSource<bool> ConfirmationStarted { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
    public int Initialized { get; private set; }
    public int Confirmations { get; private set; }
    public int NavigatedFromCount { get; private set; }
    public int NavigatedToCount { get; private set; }
    public int Destroyed { get; private set; }
    public bool ThrowOnDestroy { get; private set; }
    public INavigationParameters ReceivedParameters { get; private set; }

    public void Initialize(INavigationParameters parameters) => Initialized++;

    public async Task InitializeAsync(INavigationParameters parameters)
    {
        if (parameters.TryGetValue<List<MockNavigateFromPage>>("createdPages", out var pages))
            pages.Add(this);
        ThrowOnDestroy = parameters.TryGetValue<bool>("failDestroy", out var failDestroy) && failDestroy;
        if (parameters.TryGetValue<Func<Task>>("duringInitialize", out var initialize))
            await initialize();
        if (parameters.TryGetValue<bool>("failInitialize", out var fail) && fail)
            throw new InvalidOperationException("Initialization failed.");
    }

    public async Task<bool> CanNavigateAsync(INavigationParameters parameters)
    {
        Confirmations++;
        ConfirmationStarted.TrySetResult(true);
        return Decision is null ? AllowNavigation : await Decision.Task;
    }

    public void OnNavigatedFrom(INavigationParameters parameters) => NavigatedFromCount++;

    public void OnNavigatedTo(INavigationParameters parameters)
    {
        NavigatedToCount++;
        ReceivedParameters = parameters;
    }

    public void Destroy()
    {
        Destroyed++;
        if (ThrowOnDestroy)
            throw new InvalidOperationException("Destroy failed.");
    }
}
