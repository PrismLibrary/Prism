namespace Prism.Navigation
{
    public interface INavigationAware : INavigationServiceAware
    {
        void OnNavigatedFrom(NavigationParameters parameters);
        void OnNavigatedTo(NavigationParameters parameters);
    }
}
