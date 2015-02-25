namespace Prism.Navigation
{
    public interface IConfirmNavigation : INavigationAware
    {
        bool CanNavigate(NavigationParameters parameters);
    }
}
