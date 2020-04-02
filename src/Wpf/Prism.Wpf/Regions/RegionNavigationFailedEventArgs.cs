

using System;

namespace Prism.Regions
{
    /// <summary>
    /// EventArgs used with the NavigationFailed event.
    /// </summary>
    public class RegionNavigationFailedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegionNavigationEventArgs"/> class.
        /// </summary>
        /// <param name="navigationContext">The navigation context.</param>
        public RegionNavigationFailedEventArgs(NavigationContext navigationContext)
        {
            if (navigationContext == null)
                throw new ArgumentNullException(nameof(navigationContext));

            this.NavigationContext = navigationContext;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegionNavigationFailedEventArgs"/> class.
        /// </summary>
        /// <param name="navigationContext">The navigation context.</param>
        /// <param name="error">The error.</param>
        public RegionNavigationFailedEventArgs(NavigationContext navigationContext, Exception error)
            : this(navigationContext)
        {
            this.Error = error;
        }

        /// <summary>
        /// Gets the navigation context.
        /// </summary>
        /// <value>The navigation context.</value>
        public NavigationContext NavigationContext { get; private set; }

        /// <summary>
        /// Gets the error.
        /// </summary>
        /// <value>The <see cref="Exception"/>, or <see langword="null"/> if the failure was not caused by an exception.</value>
        public Exception Error { get; private set; }

        /// <summary>
        /// Gets the navigation URI
        /// </summary>
        /// <value>The URI.</value>
        /// <remarks>
        /// This is a convenience accessor around NavigationContext.Uri.
        /// </remarks>
        public Uri Uri
        {
            get
            {
                if (this.NavigationContext != null)
                {
                    return this.NavigationContext.Uri;
                }

                return null;
            }
        }
    }
}
