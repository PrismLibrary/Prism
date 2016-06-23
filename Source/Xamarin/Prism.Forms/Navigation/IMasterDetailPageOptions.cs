namespace Prism.Navigation
{
    /// <summary>
    /// Provides a way for the INavigationService to make decisions regarding a MasterDetailPage during navigation.
    /// </summary>
    public interface IMasterDetailPageOptions
    {
        /// <summary>
        /// The INavigationService uses the result of this property to determine if the MasterDetailPage.Master should be presented after navigation.
        /// </summary>
        bool IsPresentedAfterNavigation { get; }
    }
}
