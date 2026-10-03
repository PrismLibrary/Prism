namespace Prism.DryIoc.Maui.Tests.Mocks.ViewModels;

public sealed class MockNavigateFromLifecycle : IConfirmNavigationAsync, INavigationAware
{
    public bool AllowNavigation { get; set; } = true;
    public TaskCompletionSource<bool> Decision { get; set; }
    public TaskCompletionSource<bool> ConfirmationStarted { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
    public int Confirmations { get; private set; }
    public int Departures { get; private set; }
    public int Arrivals { get; private set; }

    public Task<bool> CanNavigateAsync(INavigationParameters parameters)
    {
        Confirmations++;
        ConfirmationStarted.TrySetResult(true);
        return Decision?.Task ?? Task.FromResult(AllowNavigation);
    }

    public void OnNavigatedFrom(INavigationParameters parameters) => Departures++;
    public void OnNavigatedTo(INavigationParameters parameters) => Arrivals++;
}
