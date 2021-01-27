namespace Prism.Navigation
{
    /// <summary>
    /// The NavigationMode provides information about the navigation operation that has been invoked.
    /// </summary>
    public enum NavigationMode
    {
        /// <summary>
        /// Indicates that a navigation operation occured that resulted in navigating backwards in the navigation stack.
        /// </summary>
        Back,
        /// <summary>
        /// Indicates that a new navigation operation has occured and a new page has been added to the navigation stack.
        /// </summary>
        New,
    }
}
