using System;
using System.ComponentModel;
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
            return ((service as IFrameFacadeProvider).FrameFacade as IFrameProvider).Frame;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static INavigationService SetAsWindowContent(this INavigationService service, Window window, bool activate)
        {
            window.Content = service.GetXamlFrame();
            if (activate)
            {
                window.Activate();
            }
            return service;
        }

        public static async Task<INavigationResult> NavigateAsync(this INavigationService service, string path, params (string Name, object Value)[] parameters)
        {
            return await service.NavigateAsync(path, GetNavigationParameters(parameters));
        }

        public static async Task<INavigationResult> NavigateAsync(this INavigationService service, string path, NavigationTransitionInfo infoOverride = null, params (string Name, object Value)[] parameters)
        {
            return await service.NavigateAsync(path, GetNavigationParameters(parameters), infoOverride);
        }

        public static Task RefreshAsync(this INavigationService service)
            => (service as IPlatformNavigationService).RefreshAsync();

        public static bool CanGoBack(this INavigationService service)
            => (service as IPlatformNavigationService).CanGoBack();

        public static bool CanGoForward(this INavigationService service)
            => (service as IPlatformNavigationService).CanGoForward();

        public static Task GoForwardAsync(this INavigationService service, INavigationParameters parameter)
            => (service as IPlatformNavigationService).GoForwardAsync(parameter);

        public static Task<INavigationResult> NavigateAsync(this INavigationService service, string path, INavigationParameters parameter, NavigationTransitionInfo infoOverride)
            => (service as IPlatformNavigationService).NavigateAsync(path, parameter, infoOverride);

        public static Task<INavigationResult> NavigateAsync(this INavigationService service, Uri path, INavigationParameters parameter, NavigationTransitionInfo infoOverride)
            => (service as IPlatformNavigationService).NavigateAsync(path, parameter, infoOverride);

        public static Task<INavigationResult> GoBackAsync(this INavigationService navigationService, params (string Name, object Value)[] parameters)
        {
            return navigationService.GoBackAsync(GetNavigationParameters(parameters));
        }

        public static Task<INavigationResult> NavigateAsync(this INavigationService navigationService, Uri uri, params (string Name, object Value)[] parameters)
        {
            return navigationService.NavigateAsync(uri, GetNavigationParameters(parameters));
        }

        private static INavigationParameters GetNavigationParameters((string Name, object Value)[] parameters)
        {
            var navParams = new NavigationParameters();
            foreach (var (Name, Value) in parameters)
            {
                navParams.Add(Name, Value);
            }
            return navParams;
        }
    }
}
