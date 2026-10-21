using System.Collections.Specialized;

namespace Playground.Module.ViewModels;

internal class RegionLabViewModel : BindableBase, IRegionAware
{
    private readonly IRegionManager _regionManager;
    private IRegion? _detailRegion;

    public RegionLabViewModel(IRegionManager regionManager)
    {
        _regionManager = regionManager;
        OpenAlphaCommand = new DelegateCommand(() =>
            _regionManager.RequestNavigate("DetailRegion", "RegionDetailAlphaView"),
            () => _regionManager.Regions.ContainsRegionWithName("DetailRegion"));
        OpenBetaCommand = new DelegateCommand(() =>
            _regionManager.RequestNavigate("DetailRegion", "RegionDetailBetaView"),
            () => _regionManager.Regions.ContainsRegionWithName("DetailRegion"));
        GoBackDetailCommand = new DelegateCommand(
            () => _detailRegion?.NavigationService.Journal.GoBack(),
            () => _detailRegion?.NavigationService.Journal.CanGoBack == true);
    }

    public DelegateCommand OpenAlphaCommand { get; }

    public DelegateCommand OpenBetaCommand { get; }

    public DelegateCommand GoBackDetailCommand { get; }

    public void OnNavigatedTo(NavigationContext navigationContext)
    {
        _regionManager.Regions.CollectionChanged += OnRegionsCollectionChanged;
        TryAttachDetailRegion();
    }

    public bool IsNavigationTarget(NavigationContext navigationContext) => true;

    public void OnNavigatedFrom(NavigationContext navigationContext)
    {
        _regionManager.Regions.CollectionChanged -= OnRegionsCollectionChanged;
        DetachDetailNavigated();
    }

    private void OnRegionsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        TryAttachDetailRegion();
    }

    private void TryAttachDetailRegion()
    {
        if (!_regionManager.Regions.ContainsRegionWithName("DetailRegion"))
        {
            DetachDetailNavigated();
            OpenAlphaCommand.RaiseCanExecuteChanged();
            OpenBetaCommand.RaiseCanExecuteChanged();
            return;
        }

        var region = _regionManager.Regions["DetailRegion"];
        if (ReferenceEquals(_detailRegion, region))
        {
            OpenAlphaCommand.RaiseCanExecuteChanged();
            OpenBetaCommand.RaiseCanExecuteChanged();
            return;
        }

        DetachDetailNavigated();
        _detailRegion = region;
        _detailRegion.NavigationService.Navigated += OnDetailNavigated;
        OpenAlphaCommand.RaiseCanExecuteChanged();
        OpenBetaCommand.RaiseCanExecuteChanged();
        GoBackDetailCommand.RaiseCanExecuteChanged();
    }

    private void DetachDetailNavigated()
    {
        if (_detailRegion is not null)
        {
            _detailRegion.NavigationService.Navigated -= OnDetailNavigated;
            _detailRegion = null;
        }

        GoBackDetailCommand.RaiseCanExecuteChanged();
    }

    private void OnDetailNavigated(object? sender, RegionNavigationEventArgs e)
    {
        GoBackDetailCommand.RaiseCanExecuteChanged();
    }
}
