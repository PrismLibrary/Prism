using System;

namespace Prism.Regions.Navigation
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
        public RegionNavigationFailedEventArgs(INavigationContext navigationContext)
        {
            NavigationContext = navigationContext ?? throw new ArgumentNullException(nameof(navigationContext));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegionNavigationFailedEventArgs"/> class.
        /// </summary>
        /// <param name="navigationContext">The navigation context.</param>
        /// <param name="error">The error.</param>
        public RegionNavigationFailedEventArgs(INavigationContext navigationContext, Exception error)
            : this(navigationContext)
        {
            Error = error;
        }

        /// <summary>
        /// Gets the navigation context.
        /// </summary>
        /// <value>The navigation context.</value>
        public INavigationContext NavigationContext { get; }

        /// <summary>
        /// Gets the error.
        /// </summary>
        /// <value>The <see cref="Exception"/>, or <see langword="null"/> if the failure was not caused by an exception.</value>
        public Exception Error { get; }

        /// <summary>
        /// Gets the navigation URI
        /// </summary>
        /// <value>The URI.</value>
        /// <remarks>
        /// This is a convenience accessor around NavigationContext.Uri.
        /// </remarks>
        public Uri Uri => NavigationContext?.Uri;
    }
}
