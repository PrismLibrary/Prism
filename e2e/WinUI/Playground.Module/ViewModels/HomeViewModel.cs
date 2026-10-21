namespace Playground.Module.ViewModels;

internal class HomeViewModel : BindableBase
{
    private readonly IRegionManager _regionManager;

    public HomeViewModel(IRegionManager regionManager)
    {
        _regionManager = regionManager;
        GoToRegionLabCommand = new DelegateCommand(() =>
            _regionManager.RequestNavigate("ContentRegion", "RegionLabView"));
    }

    public DelegateCommand GoToRegionLabCommand { get; }
}
