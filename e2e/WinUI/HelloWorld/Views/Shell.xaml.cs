namespace HelloWorld.Views;

public sealed partial class Shell : Page, ILoadableShell
{
    private readonly IRegionManager _regionManager;
    private readonly Prism.Windows.IWindowManager _windows;

    public Shell(IRegionManager regionManager, Prism.Windows.IWindowManager windows)
    {
        _regionManager = regionManager;
        _windows = windows;
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

    private void OpenWindow(object sender, RoutedEventArgs args)
    {
        _windows.OpenWindow(new Window { Title = $"Prism playground {_windows.Windows.Count + 1}" },
            container => container.Resolve<Shell>(),
            window => window.RegionManager.RequestNavigate("ContentRegion", "HomeView"));
    }

    private void OnNavigated(object? sender, RegionNavigationEventArgs e)
    {
        MainNavigation.IsBackEnabled = _regionManager.Regions["ContentRegion"].NavigationService.Journal.CanGoBack;
    }
}
