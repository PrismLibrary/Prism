using Prism.Navigation;

namespace Prism.Maui.Tests.Mocks.Views;

public class RelativeNavigationPageMock : ContentPage, IInitialize, INavigationAware, IConfirmNavigationAsync, IDestructible
{
    public bool CanNavigate { get; set; } = true;
    public TaskCompletionSource<bool> Confirmation { get; set; }
    public TaskCompletionSource<bool> ConfirmationStarted { get; } = new(TaskCreationOptions.RunContinuationsAsynchronously);
    public int Confirmations { get; private set; }
    public int Initialized { get; private set; }
    public int NavigatedFromCount { get; private set; }
    public int NavigatedToCount { get; private set; }
    public int Destroyed { get; private set; }
    public INavigationParameters FromParameters { get; private set; }
    public INavigationParameters ToParameters { get; private set; }

    public Task<bool> CanNavigateAsync(INavigationParameters parameters)
    {
        Confirmations++;
        ConfirmationStarted.TrySetResult(true);
        return Confirmation?.Task ?? Task.FromResult(CanNavigate);
    }

    public void Initialize(INavigationParameters parameters) => Initialized++;

    public void OnNavigatedFrom(INavigationParameters parameters)
    {
        NavigatedFromCount++;
        FromParameters = parameters;
    }

    public void OnNavigatedTo(INavigationParameters parameters)
    {
        NavigatedToCount++;
        ToParameters = parameters;
    }

    public void Destroy() => Destroyed++;
}
