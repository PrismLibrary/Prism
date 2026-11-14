namespace Playground.Module.ViewModels;

internal class HomeViewModel : BindableBase
{
    private readonly IRegionManager _regionManager;

    public HomeViewModel(IRegionManager regionManager)
    {
        _regionManager = regionManager;
        GoToRegionLabCommand = new DelegateCommand(() =>
            _regionManager.RequestNavigate("ContentRegion", "RegionLabView"));
        GoToDialogLabCommand = new DelegateCommand(() =>
            _regionManager.RequestNavigate("ContentRegion", "DialogLabView"));
        GoToModuleCommand = new DelegateCommand(() =>
            _regionManager.RequestNavigate("ContentRegion", "ModuleAboutView"));
    }

    public DelegateCommand GoToRegionLabCommand { get; }

    public DelegateCommand GoToDialogLabCommand { get; }

    public DelegateCommand GoToModuleCommand { get; }
}
