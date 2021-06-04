using System;

namespace Prism.Navigation
{
    /// <summary>
    /// Provides navigation results indicating whether the navigation was successful or if there was an encountered <see cref="Exception"/>.
    /// </summary>
    public interface INavigationResult
    {
        /// <summary>
        /// <see langword="true"/> if the navigation was successful, <see langword="false"/> otherwise.
        /// </summary>
        bool Success { get; }

        /// <summary>
        /// The <see cref="Exception"/> thrown during navigation.
        /// </summary>
        Exception Exception { get; }
    }

    /// <inheritdoc />>
    public class NavigationResult : INavigationResult
    {
        /// <inheritdoc />>
        public bool Success { get; set; }

        /// <inheritdoc />>
        public Exception Exception { get; set; }
    }
}
