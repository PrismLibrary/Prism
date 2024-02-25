using Prism.Common;

namespace Prism.DryIoc.Maui.Tests.Mocks.ViewModels;

#pragma warning disable CS0067 // The event is never used because this is a Mock
public abstract class MockViewModelBase : IActiveAware, INavigationAware, IConfirmNavigation
{
    private readonly IPageAccessor _pageAccessor;

    public event EventHandler IsActiveChanged;

    protected MockViewModelBase(IPageAccessor pageAccessor, INavigationService navigationService)
    {
        _pageAccessor = pageAccessor;
        NavigationService = navigationService;
    }

    public string Message { get; private set; }

    private List<string> _actions = [];
    public IEnumerable<string> Actions => _actions;

    public INavigationService NavigationService { get; }

    public Task<INavigationResult> GoBack() => NavigationService.GoBackAsync();

    public Task<INavigationResult> NavigateTo(string uri) => NavigationService.NavigateAsync(uri);

    public Page Page => _pageAccessor.Page;

    public bool StopNavigation { get; set; }

    public bool IsActive { get; set; }

    public bool CanNavigate(INavigationParameters parameters)
    {
        _actions.Add(nameof(CanNavigate));
        return !StopNavigation;
    }

    public void OnNavigatedFrom(INavigationParameters parameters)
    {
        _actions.Add(nameof(OnNavigatedFrom));
    }

    public void OnNavigatedTo(INavigationParameters parameters)
    {
        _actions.Add(nameof(OnNavigatedTo));
        if (parameters.TryGetValue<string>("Message", out var message))
            Message = message;
    }
}
#pragma warning restore CS0067 // The event is never used because this is a Mock
