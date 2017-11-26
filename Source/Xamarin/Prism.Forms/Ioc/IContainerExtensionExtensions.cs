using Prism.Common;
using Prism.Navigation;
using Xamarin.Forms;

namespace Prism.Ioc
{
    public static class IContainerExtensionExtensions
    {
        public static INavigationService CreateNavigationService(this IContainerExtension containerExtension, Page page)
        {
            var navigationService = containerExtension.Resolve<INavigationService>(PrismApplicationBase.NavigationServiceName);
            ((IPageAware)navigationService).Page = page;
            return navigationService;
        }
    }
}
