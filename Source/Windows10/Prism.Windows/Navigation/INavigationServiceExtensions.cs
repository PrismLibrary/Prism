using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        public static string GetNavigationPath(this INavigationService service, bool includeParameters)
        {
            var nav = service as IPlatformNavigationService2;
            var facade = nav.FrameFacade as IFrameFacade2;
            var sb = new List<string>();
            foreach (var item in facade.Frame.BackStack)
            {
                if (PageRegistry.TryGetRegistration(item.SourcePageType, out var info))
                {
                    if (item.Parameter != null)
                    {
                        if (includeParameters)
                        {
                            sb.Add($"{info.Key}?{item.Parameter}");
                        }
                        else
                        {
                            sb.Add(info.Key);
                        }
                    }
                    else
                    {
                        sb.Add(info.Key);
                    }
                }
            }
            sb.Add(facade.CurrentNavigationPath);
            return $"/{string.Join("/", sb.ToArray())}";
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
