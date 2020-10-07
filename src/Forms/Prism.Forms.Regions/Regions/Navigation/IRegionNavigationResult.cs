using System;

namespace Prism.Regions.Navigation
{
    /// <summary>
    /// Represents the result of navigating to a URI.
    /// </summary>
    public interface IRegionNavigationResult
    {
        /// <summary>
        /// Gets the navigation context.
        /// </summary>
        /// <value>The navigation context.</value>
        INavigationContext Context { get; }

        /// <summary>
        /// Gets an exception that occurred while navigating.
        /// </summary>
        /// <value>The exception.</value>
        Exception Error { get; }

        /// <summary>
        /// Gets the result.
        /// </summary>
        /// <value>The result.</value>
        bool? Result { get; }
    }
}
