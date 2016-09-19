using System;

namespace Prism.Navigation
{
    /// <summary>
    /// Provides a way for Pages or ViewModels to be aware of Internal (Sideways Navigation)
    /// </summary>
    public interface IMultiPageNavigationAware
    {
        /// <summary>
        /// Called when the Parent changes the CurrentPage or when the Parent Disappears
        /// </summary>
        /// <remarks>
        /// Not recommended for Asynchronous implementations
        /// </remarks>
        /// <param name="parameters">The navigation parameters</param>
        void OnInternalNavigatedFrom(NavigationParameters parameters);

        /// <summary>
        /// Called when the Parent changes the CurrentPage or when the Parent Appears
        /// </summary>
        /// <remarks>
        /// Not recommended for Asynchronous implementations
        /// </remarks>
        /// <param name="parameters">The navigation parameters</param>
        void OnInternalNavigatedTo(NavigationParameters parameters);
    }
}

