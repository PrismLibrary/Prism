namespace Prism.DryIoc.Maui.Tests.Mocks.Views;

public sealed class MockPendingPage : ContentPage, IConfirmNavigationAsync, INavigationAware, IDestructible
{
    public TaskCompletionSource<bool> Decision { get; private set; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
    public TaskCompletionSource<bool> Started { get; private set; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
    public int Confirmations { get; private set; }
    public int NavigatedFrom { get; private set; }
    public int Destroyed { get; private set; }

    public async Task<bool> CanNavigateAsync(INavigationParameters parameters)
    {
        Confirmations++;
        Started.TrySetResult(true);
        return await Decision.Task;
    }

    public void ResetConfirmation()
    {
        Decision = new(TaskCreationOptions.RunContinuationsAsynchronously);
        Started = new(TaskCreationOptions.RunContinuationsAsynchronously);
    }

    public void OnNavigatedTo(INavigationParameters parameters)
    {
    }

    public void OnNavigatedFrom(INavigationParameters parameters) => NavigatedFrom++;
    public void Destroy() => Destroyed++;
}
