namespace PrismMauiDemo.ViewModels;

internal class MainPageViewModel
{
    private INavigationService _navigationService { get; }

    public MainPageViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;
        NavigateCommand = new DelegateCommand<string>(OnNavigateCommandExecuted);
    }

    public DelegateCommand<string> NavigateCommand { get; }

    private void OnNavigateCommandExecuted(string uri)
    {
        _navigationService.NavigateAsync(uri)
            .OnNavigationError(ex => Console.WriteLine(ex));
    }
}
