using Prism.Commands;
using Prism.Navigation.Regions;

using SampleApp.Views;

namespace SampleApp.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly IRegionManager _regionManager;
    private bool _isPaneOpened;

    public MainWindowViewModel(IRegionManager regionManager)
    {
        // Since this is a basic ShellWindow, there's not much to do here.
        // For enterprise apps, you could register up subscriptions
        // or other startup background tasks so that they get triggered
        // on startup, rather than putting them in the DashboardViewModel.
        //
        // For example, initiate the pulling of News Feeds, etc.

        _regionManager = regionManager;
        Title = "Sample Prism.Avalonia";
        IsPaneOpened = true;
    }

    public DelegateCommand CmdDashboard => new(() =>
    {
        // _journal.Clear();
        _regionManager.RequestNavigate(RegionNames.ContentRegion, nameof(DashboardView));
    });

    public DelegateCommand CmdFlyoutMenu => new(() =>
    {
        IsPaneOpened = !IsPaneOpened;
    });

    public DelegateCommand CmdSettings => new(() => _regionManager.RequestNavigate(RegionNames.ContentRegion, nameof(SettingsView)));

    public bool IsPaneOpened { get => _isPaneOpened; set => SetProperty(ref _isPaneOpened, value); }
}
