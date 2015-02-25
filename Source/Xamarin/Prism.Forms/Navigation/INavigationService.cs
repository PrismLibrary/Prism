
namespace Prism.Navigation
{
    public interface INavigationService
    {
        void GoBack(bool animated = true, bool useModalNavigation = true);
        void GoBack(object page, NavigationParameters parameters, bool animated = true, bool useModalNavigation = true);
        void Navigate(string name, bool animated = true, bool useModalNavigation = true);
        void Navigate(object page, string name, NavigationParameters parameters, bool animated = true, bool useModalNavigation = true);
    }
}
