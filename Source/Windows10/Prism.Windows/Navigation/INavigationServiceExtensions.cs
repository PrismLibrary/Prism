using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace Prism.Navigation
{
    public static class INavigationServiceExtensions
    {
        internal static Frame GetXamlFrame(this INavigationService service)
        {
            return ((service as IPlatformNavigationService2).FrameFacade as IFrameFacade2).Frame;
        }

        public static INavigationService SetAsWindowContent(this INavigationService service, Window window, bool activate)
        {
            window.Content = service.GetXamlFrame();
            if (activate)
            {
                window.Activate();
            }
            return service;
        }

        public static async Task<INavigationResult> NavigateAsync(this INavigationService service, string path, params (string Name, string Value)[] parameters)
        {
            return await service.NavigateAsync(PathBuilder.Create(path, parameters).ToString());
        }

        public static async Task<INavigationResult> NavigateAsync(this INavigationService service, string path, NavigationTransitionInfo infoOverride = null, params (string Name, string Value)[] parameters)
        {
            return await service.NavigateAsync(PathBuilder.Create(path, parameters).ToString(), null, infoOverride);
        }

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
