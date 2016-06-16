using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prism.Regions
{
    public static class NavigateAsyncExtensions
    {
        public static async void RequestNavigate(this INavigateAsync navigator, Uri target, Action<NavigationResult> navigationCallback)
        {
            NavigationResult result = await navigator.RequestNavigateAsync(target);
            navigationCallback(result);
        }
        public static async void RequestNavigate(this INavigateAsync navigator,  Uri target, Action<NavigationResult> navigationCallback, NavigationParameters navigationParameters)
        {
            NavigationResult result = await navigator.RequestNavigateAsync(target, navigationParameters);
            navigationCallback(result);
        }
    }
}
