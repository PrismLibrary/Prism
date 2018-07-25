using Prism.Navigation;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Animation;

namespace Prism.Navigation
{
    public static partial class IContainerRegistryExtensions
    {
        public static Task RefreshAsync(this INavigationService service)
            => (service as IPlatformNavigationService).RefreshAsync();

        public static bool CanGoBack(this INavigationService service)
            => (service as IPlatformNavigationService).CanGoBack();

        public static Task GoBackAsync(this INavigationService service, INavigationParameters parameters, NavigationTransitionInfo infoOverride)
            => (service as IPlatformNavigationService).RefreshAsync();

        public static bool CanGoForward(this INavigationService service)
            => (service as IPlatformNavigationService).CanGoForward();

        public static Task GoForwardAsync(this INavigationService service, INavigationParameters parameter)
            => (service as IPlatformNavigationService).GoForwardAsync(parameter);

        public static Task<INavigationResult> NavigateAsync(this INavigationService service, string path, INavigationParameters parameter, NavigationTransitionInfo infoOverride)
            => (service as IPlatformNavigationService).NavigateAsync(path, parameter, infoOverride);

        public static Task<INavigationResult> NavigateAsync(this INavigationService service, Uri path, INavigationParameters parameter, NavigationTransitionInfo infoOverride)
            => (service as IPlatformNavigationService).NavigateAsync(path, parameter, infoOverride);
    }
}
