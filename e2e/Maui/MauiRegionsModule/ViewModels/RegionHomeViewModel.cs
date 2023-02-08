namespace MauiRegionsModule.ViewModels;

internal class RegionHomeViewModel
{
    private INavigationService _navigationService { get; }

    public RegionHomeViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;
        NavigateCommand = new DelegateCommand<string>(OnNavigateCommandExecuted);
    }

    public DelegateCommand<string> NavigateCommand { get; }

    private void OnNavigateCommandExecuted(string uri)
    {
        _navigationService.NavigateAsync(uri);
    }
}
