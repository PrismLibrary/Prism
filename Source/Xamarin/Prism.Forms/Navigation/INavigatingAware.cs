namespace Prism.Navigation
{
    public interface INavigatingAware
    {
        /// <summary>
        /// Called when the implementer starts the process of being navigated to, but before the view has actually been added to the navigation stack.
        /// </summary>
        /// <param name="parameters">The navigation parameters.</param>
        void OnNavigatingTo(NavigationParameters parameters);
    }
}
