

using System;
using System.Threading.Tasks;

namespace Prism.Regions
{
    /// <summary>
    /// Provides a way for objects involved in navigation to determine if a navigation request should continue.
    /// </summary>
    public interface IConfirmNavigationRequestAsync : INavigationAware
    {
        /// <summary>
        /// Determines whether this instance accepts being navigated away from.
        /// </summary>
        /// <param name="navigationContext">The navigation context.</param>
        /// <returns>True if navigation was allowed</returns>
        Task<bool> ConfirmNavigationRequestAsync(NavigationContext navigationContext);
    }
}
