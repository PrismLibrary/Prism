using System;

namespace Prism.Regions.Navigation
{
    /// <summary>
    /// Represents the result of navigating to a URI.
    /// </summary>
    public class RegionNavigationResult : IRegionNavigationResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegionNavigationResult"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="result">The result.</param>
        public RegionNavigationResult(INavigationContext context, bool? result)
        {
            Context = context;
            Result = result;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegionNavigationResult"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="error">The error.</param>
        public RegionNavigationResult(INavigationContext context, Exception error)
        {
            Context = context;
            Error = error;
            Result = false;
        }

        /// <summary>
        /// Gets the result.
        /// </summary>
        /// <value>The result.</value>
        public bool? Result { get; }

        /// <summary>
        /// Gets an exception that occurred while navigating.
        /// </summary>
        /// <value>The exception.</value>
        public Exception Error { get; }

        /// <summary>
        /// Gets the navigation context.
        /// </summary>
        /// <value>The navigation context.</value>
        public INavigationContext Context { get; }
    }
}
