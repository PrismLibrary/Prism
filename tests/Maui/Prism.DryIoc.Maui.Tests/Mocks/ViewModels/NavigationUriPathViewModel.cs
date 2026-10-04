namespace Prism.DryIoc.Maui.Tests.Mocks.ViewModels;

public class NavigationUriPathViewModel
{
    public NavigationUriPathViewModel(INavigationService navigationService)
    {
        NavigationService = navigationService;
        ConstructorPath = navigationService.GetNavigationUriPath();
    }

    public INavigationService NavigationService { get; }
    public string ConstructorPath { get; }
}
