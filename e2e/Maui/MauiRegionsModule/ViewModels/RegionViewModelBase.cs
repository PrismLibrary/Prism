using Prism.Common;

namespace MauiRegionsModule.ViewModels;

public abstract class RegionViewModelBase : BindableBase, IRegionAware, IPageLifecycleAware
{
    protected string Name => GetType().Name.Replace("ViewModel", string.Empty);
    protected INavigationService _navigationService { get; }
    private IPageAccessor _pageAccessor { get; }
    protected IRegionNavigationService? _regionNavigation { get; private set; }

    protected RegionViewModelBase(INavigationService navigationService, IPageAccessor pageAccessor)
    {
        _navigationService = navigationService;
        _pageAccessor = pageAccessor;
    }

    public bool IsNavigationTarget(NavigationContext navigationContext) =>
        navigationContext.NavigatedName() == Name;

    private string? _message;
    public string? Message
    {
        get => _message;
        set => SetProperty(ref _message, value);
    }

    private int _viewCount;
    public int ViewCount
    {
        get => _viewCount;
        set => SetProperty(ref _viewCount, value);
    }

    public string? PageName => _pageAccessor.Page?.GetType()?.Name;

    public void OnNavigatedFrom(NavigationContext navigationContext)
    {

    }

    public void OnNavigatedTo(NavigationContext navigationContext)
    {
        if (navigationContext.Parameters.ContainsKey(nameof(Message)))
            Message = navigationContext.Parameters.GetValue<string>(nameof(Message));

        _regionNavigation = navigationContext.NavigationService;
        ViewCount = navigationContext.NavigationService.Region.Views.Count();
    }

    public void OnAppearing()
    {
        RaisePropertyChanged(nameof(PageName));
    }

    public void OnDisappearing()
    {
    }
}
