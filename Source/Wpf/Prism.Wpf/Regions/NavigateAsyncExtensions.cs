using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Prism.Common;

namespace Prism.Regions
{
    public static class NavigateAsyncExtensions
    {
        /// <summary>
        /// Initiates navigation to the target specified by the <see cref="Uri" />.
        /// </summary>
        /// <param name="target">The navigation target</param>
        /// <returns>The navigation result.</returns>
        public static Task<NavigationResult> RequestNavigateAsync(this INavigateAsync navigation, Uri target)
        {
            if (navigation == null)
                throw new ArgumentNullException(nameof(navigation));

            return CallbackHelper.AwaitCallbackResult<NavigationResult>(callback => navigation.RequestNavigate(target, callback));
        }

        /// <summary>
        /// Initiates navigation to the target specified by the <see cref="Uri"/>.
        /// </summary>
        /// <param name="target">The navigation target</param>
        /// <param name="navigationParameters">The navigation parameters specific to the navigation request.</param>
        /// <returns>The navigation result.</returns>
        public static Task<NavigationResult> RequestNavigateAsync(this INavigateAsync navigation, Uri target, NavigationParameters navigationParameters)
        {
            if (navigation == null)
                throw new ArgumentNullException(nameof(navigation));

            return CallbackHelper.AwaitCallbackResult<NavigationResult>(callback => navigation.RequestNavigate(target, callback, navigationParameters));
        }
    }
}
