namespace HelloWorld.Views;

public sealed partial class Shell : Page, ILoadableShell
{
    private readonly IRegionManager _regionManager;

    public Shell(IRegionManager regionManager)
    {
        _regionManager = regionManager;
        InitializeComponent();
    }

    public void FinishLoading()
    {
        var navigationService = _regionManager.Regions["ContentRegion"].NavigationService;
        navigationService.Navigated += OnNavigated;
        MainNavigation.IsBackEnabled = navigationService.Journal.CanGoBack;
        LoadingProgress.IsActive = false;
        LoadingOverlay.Visibility = Visibility.Collapsed;
    }

    private void OnNavigated(object? sender, RegionNavigationEventArgs e)
    {
        MainNavigation.IsBackEnabled = _regionManager.Regions["ContentRegion"].NavigationService.Journal.CanGoBack;
    }
}
