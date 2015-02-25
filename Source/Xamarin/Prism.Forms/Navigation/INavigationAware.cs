namespace Prism.Navigation
{
    public interface INavigationAware
    {
        void OnNavigatedFrom(NavigationParameters parameters);
        void OnNavigatedTo(NavigationParameters parameters);
    }
}
