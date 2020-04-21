using System;
using Prism.Ioc.Internals;
using Prism.Navigation;

namespace Prism.Regions.Navigation
{
    /// <summary>
    /// Encapsulates information about a navigation request.
    /// </summary>
    public interface INavigationContext
    {
        /// <summary>
        /// Gets the region navigation service.
        /// </summary>
        /// <value>The navigation service.</value>
        IRegionNavigationService NavigationService { get; }

        /// <summary>
        /// Gets the <see cref="INavigationParameters"/> extracted from the URI and the object parameters passed in navigation.
        /// </summary>
        /// <value>The URI query.</value>
        INavigationParameters Parameters { get; }

        /// <summary>
        /// Gets the navigation URI.
        /// </summary>
        /// <value>The navigation URI.</value>
        Uri Uri { get; }
    }
}