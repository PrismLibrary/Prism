
namespace Prism.Navigation
{
    public interface INavigationService
    {
        void GoBack(bool useModalNavigation = true, bool animated = true);
        void Navigate<T>(NavigationParameters parameters = null, bool useModalNavigation = true, bool animated = true);
        void Navigate(string name, NavigationParameters parameters = null, bool useModalNavigation = true, bool animated = true);
    }
}
