namespace Prism.Navigation;

internal class ForceLoadedNavigationService : INavigationService
{
    public Task<INavigationResult> GoBackAsync(INavigationParameters parameters)
    {
        throw new NotImplementedException();
    }

    public Task<INavigationResult> GoBackToAsync(string name, INavigationParameters parameters)
    {
        throw new NotImplementedException();
    }

    public Task<INavigationResult> GoBackToRootAsync(INavigationParameters parameters)
    {
        throw new NotImplementedException();
    }

    public Task<INavigationResult> NavigateAsync(Uri uri, INavigationParameters parameters)
    {
        throw new NotImplementedException();
    }
}
