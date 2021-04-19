namespace Prism.Navigation
{
    /// <summary>
    /// Provides a way for the <see cref="INavigationService" /> to know whether the Flyout should be presented after navigation.
    /// </summary>
    public interface IFlyoutPageOptions
    {
        /// <summary>
        /// The INavigationService uses the result of this property to determine if the FlyoutPage.Flyout should be presented after navigation.
        /// </summary>
        bool IsPresentedAfterNavigation { get; }
    }
}
