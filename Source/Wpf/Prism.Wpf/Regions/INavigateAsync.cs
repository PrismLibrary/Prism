

using System;
using System.Threading.Tasks;

namespace Prism.Regions
{
    /// <summary>
    /// Provides methods to perform navigation.
    /// </summary>
    /// <remarks>
    /// Convenience overloads for the methods in this interface can be found as extension methods on the 
    /// <see cref="NavigationAsyncExtensions"/> class.
    /// </remarks>
    public interface INavigateAsync
    {
        /// <summary>
        /// Initiates navigation to the target specified by the <see cref="Uri" />.
        /// </summary>
        /// <param name="target">The navigation target</param>
        /// <returns>The navigation result.</returns>
        /// <remarks>
        /// Convenience overloads for this method can be found as extension methods on the
        /// <see cref="NavigationAsyncExtensions" /> class.
        /// </remarks>
        Task<NavigationResult> RequestNavigateAsync(Uri target);

        /// <summary>
        /// Initiates navigation to the target specified by the <see cref="Uri"/>.
        /// </summary>
        /// <param name="target">The navigation target</param>
        /// <param name="navigationParameters">The navigation parameters specific to the navigation request.</param>
        /// <returns>The navigation result.</returns>
        /// <remarks>
        /// Convenience overloads for this method can be found as extension methods on the 
        /// <see cref="NavigationAsyncExtensions"/> class.
        /// </remarks>
        Task<NavigationResult> RequestNavigateAsync(Uri target, NavigationParameters navigationParameters);
    }
}
