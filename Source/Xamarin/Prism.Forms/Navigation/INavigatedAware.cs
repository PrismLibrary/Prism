namespace Prism.Navigation
{
    /// <summary>
    /// Provides a way for ViewModels involved in navigation to be notified of navigation activities after the target Page has been added to the navigation stack.
    /// </summary>
    public interface INavigatedAware
    {
        /// <summary>
        /// Called when the implementer has been navigated away from.
        /// </summary>
        /// <param name="parameters">The navigation parameters.</param>
        void OnNavigatedFrom(INavigationParameters parameters);

        /// <summary>
        /// Called when the implementer has been navigated to.
        /// </summary>
        /// <param name="parameters">The navigation parameters.</param>
        void OnNavigatedTo(INavigationParameters parameters);
    }
}
