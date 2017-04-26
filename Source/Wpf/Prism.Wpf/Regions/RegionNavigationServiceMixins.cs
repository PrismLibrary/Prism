using System;
using System.Threading.Tasks;

namespace Prism.Regions
{
    public static class NavigationServiceMixins
    {
        public static async Task<NavigationResult> GoBackAsync(this IRegionNavigationService service)
        {
            return await CatchNavigationResult(service, () => service.Journal.GoBack());
        }

        public static async Task<NavigationResult> GoForwardAsync(this IRegionNavigationService service)
        {
            return await CatchNavigationResult(service, () => service.Journal.GoForward());
        }

        private static async Task<NavigationResult> CatchNavigationResult(IRegionNavigationService service, Action navigationAction)
        {
            var tcs = new TaskCompletionSource<NavigationResult>();

            EventHandler<RegionNavigationEventArgs> navigated =
                (s, a) => tcs.SetResult(new NavigationResult(a.NavigationContext, true));
            EventHandler<RegionNavigationFailedEventArgs> failed =
                (s, a) => tcs.SetResult(new NavigationResult(a.NavigationContext, a.Error));

            service.Navigated += navigated;
            service.NavigationFailed += failed;

            navigationAction();
            var result = await tcs.Task;

            service.Navigated -= navigated;
            service.NavigationFailed -= failed;

            return result;
        }
    }
}
