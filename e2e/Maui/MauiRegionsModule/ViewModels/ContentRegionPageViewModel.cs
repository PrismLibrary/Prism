namespace MauiRegionsModule.ViewModels;

public class ContentRegionPageViewModel : IInitialize
{
    private IRegionManager _regionManager { get; }
    private int _count;

    public ContentRegionPageViewModel(IRegionManager regionManager)
    {
        _regionManager = regionManager;
        NavigateCommand = new DelegateCommand<string>(OnNavigateCommandExecuted);
    }

    public void Initialize(INavigationParameters parameters)
    {
        //_regionManager.RequestNavigate("ContentRegion", "RegionViewA");
    }

    public DelegateCommand<string> NavigateCommand { get; }

    private void OnNavigateCommandExecuted(string uri)
    {
        var message = $"Hello from Content Region Page ({_count++})";
        _regionManager.RequestNavigate("ContentRegion", uri, new NavigationParameters { { "Message", message } });
    }
}
