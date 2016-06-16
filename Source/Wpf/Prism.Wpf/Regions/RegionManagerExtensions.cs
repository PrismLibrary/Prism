using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prism.Regions
{
    public static class RegionManagerExtensions
    {
        public static async void RequestNavigate(this IRegionManager regionManager, string regionName, Uri source, Action<NavigationResult> navigationCallback)
        {
            NavigationResult result = await regionManager.RequestNavigateAsync(regionName, source);
            navigationCallback(result);
        }

        public static async void RequestNavigate(this IRegionManager regionManager, string regionName, string source, Action<NavigationResult> navigationCallback)
        {
            NavigationResult result = await regionManager.RequestNavigateAsync(regionName, source);
            navigationCallback(result);
        }

        public static async void RequestNavigate(this IRegionManager regionManager, string regionName, Uri target, Action<NavigationResult> navigationCallback, NavigationParameters navigationParameters)
        {
            NavigationResult result = await regionManager.RequestNavigateAsync(regionName, target, navigationParameters);
            navigationCallback(result);
        }

        public static async void RequestNavigate(this IRegionManager regionManager, string regionName, string target, Action<NavigationResult> navigationCallback, NavigationParameters navigationParameters)
        {
            NavigationResult result = await regionManager.RequestNavigateAsync(regionName, target, navigationParameters);
            navigationCallback(result);
        }
    }
}
