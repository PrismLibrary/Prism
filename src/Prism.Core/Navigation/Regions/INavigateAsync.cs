using System;

namespace Prism.Navigation.Regions
{
    /// <summary>
    /// Provides methods to perform navigation.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <see cref="INavigateAsync"/> is used to request navigation within a region. Navigation can involve loading a new view,
    /// activating an existing view, or passing parameters between views.
    /// </para>
    /// <para>
    /// Convenience overloads for the methods in this interface can be found as extension methods on the
    /// <see cref="NavigationAsyncExtensions"/> class.
    /// </para>
    /// </remarks>
    public interface INavigateAsync
    {
        /// <summary>
        /// Initiates navigation to the target specified by the <see cref="Uri"/>.
        /// </summary>
        /// <param name="target">The navigation target (typically a view name or URI)</param>
        /// <param name="navigationCallback">The callback executed when the navigation request is completed.</param>
        /// <param name="navigationParameters">The navigation parameters specific to the navigation request.</param>
        /// <remarks>
        /// <para>
        /// This method performs asynchronous navigation. The navigationCallback will be invoked after navigation completes,
        /// whether successfully or with an error.
        /// </para>
        /// <para>
        /// Convenience overloads for this method can be found as extension methods on the
        /// <see cref="NavigationAsyncExtensions"/> class.
        /// </para>
        /// </remarks>
        void RequestNavigate(Uri target, Action<NavigationResult> navigationCallback, INavigationParameters navigationParameters);
    }
}
