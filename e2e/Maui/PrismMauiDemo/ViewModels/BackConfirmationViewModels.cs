using Prism.Mvvm;

namespace PrismMauiDemo.ViewModels;

public abstract class BackConfirmationViewModel : BindableBase, INavigationAware, IDestructible
{
    private bool _allowBack;
    private string _status = "No back requests";
    private int _requests;
    private int _navigatedFrom;
    private int _destroyed;

    protected BackConfirmationViewModel(INavigationService navigationService)
    {
        BackCommand = new DelegateCommand(async () => await navigationService.GoBackAsync());
        RootCommand = new DelegateCommand(async () => await navigationService.GoBackToRootAsync());
        PushCommand = new DelegateCommand(async () => await navigationService.NavigateAsync("AsyncBackConfirmation"));
    }

    public bool AllowBack { get => _allowBack; set => SetProperty(ref _allowBack, value); }
    public string Status { get => _status; set => SetProperty(ref _status, value); }
    public DelegateCommand BackCommand { get; }
    public DelegateCommand RootCommand { get; }
    public DelegateCommand PushCommand { get; }

    protected void Requested()
    {
        Status = $"Requests: {++_requests}; pending";
        Console.WriteLine($"BACK LAB {GetType().Name}: {Status}");
    }

    protected bool Complete()
    {
        Status = $"Requests: {_requests}; allowed: {AllowBack}";
        Console.WriteLine($"BACK LAB {GetType().Name}: {Status}");
        return AllowBack;
    }

    public void OnNavigatedFrom(INavigationParameters parameters) =>
        Console.WriteLine($"BACK LAB {GetType().Name}: OnNavigatedFrom {++_navigatedFrom}");

    public void OnNavigatedTo(INavigationParameters parameters) { }

    public void Destroy() => Console.WriteLine($"BACK LAB {GetType().Name}: Destroy {++_destroyed}");
}

public sealed class SyncBackConfirmationViewModel(INavigationService navigationService)
    : BackConfirmationViewModel(navigationService), IConfirmNavigation
{
    public bool CanNavigate(INavigationParameters parameters)
    {
        Requested();
        return Complete();
    }
}

public sealed class AsyncBackConfirmationViewModel(INavigationService navigationService)
    : BackConfirmationViewModel(navigationService), IConfirmNavigationAsync
{
    public async Task<bool> CanNavigateAsync(INavigationParameters parameters)
    {
        Requested();
        await Task.Delay(2000);
        return Complete();
    }
}
