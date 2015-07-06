

using System;

namespace Prism.Regions
{
    /// <summary>
    /// Represents the result of navigating to a URI.
    /// </summary>
    public class NavigationResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationResult"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="result">The result.</param>
        public NavigationResult(NavigationContext context, bool? result)
        {
            this.Context = context;
            this.Result = result;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationResult"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="error">The error.</param>
        public NavigationResult(NavigationContext context, Exception error)
        {
            this.Context = context;
            this.Error = error;
            this.Result = false;
        }

        /// <summary>
        /// Gets the result.
        /// </summary>
        /// <value>The result.</value>
        public bool? Result { get; private set; }

        /// <summary>
        /// Gets an exception that occurred while navigating.
        /// </summary>
        /// <value>The exception.</value>
        public Exception Error { get; private set; }

        /// <summary>
        /// Gets the navigation context.
        /// </summary>
        /// <value>The navigation context.</value>
        public NavigationContext Context { get; private set; }
    }
}
