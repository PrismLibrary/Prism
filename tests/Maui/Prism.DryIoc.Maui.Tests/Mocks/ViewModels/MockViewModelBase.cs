using Prism.Common;

namespace Prism.DryIoc.Maui.Tests.Mocks.ViewModels;

public abstract class MockViewModelBase : IConfirmNavigation
{
    private readonly IPageAccessor _pageAccessor;

    protected MockViewModelBase(IPageAccessor pageAccessor, INavigationService navigationService)
    {
        _pageAccessor = pageAccessor;
        NavigationService = navigationService;
    }

    public INavigationService NavigationService { get; }

    public Task<INavigationResult> GoBack() => NavigationService.GoBackAsync();

    public Task<INavigationResult> NavigateTo(string uri) => NavigationService.NavigateAsync(uri);

    public Page Page => _pageAccessor.Page;

    public bool StopNavigation { get; set; }

    public bool CanNavigate(INavigationParameters parameters) =>
        !StopNavigation;
}
