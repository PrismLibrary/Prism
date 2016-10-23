namespace Prism.Navigation
{
    public interface INavigatedAware
    {
        /// <summary>
        /// Called when the implementer is being navigated away from.
        /// </summary>
        /// <param name="parameters">The navigation parameters.</param>
        void OnNavigatedFrom(NavigationParameters parameters);

        /// <summary>
        /// Called when the implementer has been navigated to.
        /// </summary>
        /// <param name="parameters">The navigation parameters.</param>
        void OnNavigatedTo(NavigationParameters parameters);
    }
}
