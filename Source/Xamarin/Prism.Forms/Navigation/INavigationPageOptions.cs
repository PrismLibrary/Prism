namespace Prism.Navigation
{
    /// <summary>
    /// Provides a way for the INavigationService to make decisions regarding a NavigationPage during navigation.
    /// </summary>
    public interface INavigationPageOptions
    {
        /// <summary>
        /// The INavigationService uses the result of this property to determine if the NavigationPage should clear the NavigationStack when navigating to a new Page.
        /// </summary>
        /// <remarks>This is equivalant to calling PopToRoot, and then replacing the current Page with the target Page being navigated to.</remarks>
        bool ClearNavigationStackOnNavigation { get; }
    }
}
