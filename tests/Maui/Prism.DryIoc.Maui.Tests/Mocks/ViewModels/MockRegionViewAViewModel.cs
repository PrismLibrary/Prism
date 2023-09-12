using Prism.Common;

namespace Prism.DryIoc.Maui.Tests.Mocks.ViewModels;

public class MockRegionViewAViewModel : BindableBase, IRegionAware, IInitialize
{
    private IPageAccessor _accessor;

    public MockRegionViewAViewModel(IPageAccessor accessor)
    {
        _accessor = accessor;
    }

    public bool Initialized { get; private set; }

    public Page Page => _accessor.Page;

    private string _message;
    public string Message
    {
        get => _message;
        set => SetProperty(ref _message, value);
    }

    public void Initialize(INavigationParameters parameters)
    {
        Initialized = true;
        if (parameters.TryGetValue<string>(nameof(Message), out var message))
            Message = message;
    }

    public bool IsNavigationTarget(NavigationContext navigationContext)
    {
        return navigationContext.NavigatedName() == "MockRegionViewA";
    }

    public void OnNavigatedFrom(NavigationContext navigationContext)
    {

    }

    public void OnNavigatedTo(NavigationContext navigationContext)
    {

    }
}
