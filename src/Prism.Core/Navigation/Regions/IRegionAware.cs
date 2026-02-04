namespace Prism.Navigation.Regions
{
    /// <summary>
    /// Provides a way for objects involved in navigation to be notified of navigation activities.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <see cref="IRegionAware"/> is typically implemented by ViewModel classes that need to respond to navigation events.
    /// It allows views and their ViewModels to participate in the navigation lifecycle.
    /// </para>
    /// <para>
    /// When a view is navigated to or away from, the region will check if its ViewModel implements this interface
    /// and call the appropriate methods.
    /// </para>
    /// </remarks>
    public interface IRegionAware
    {
        /// <summary>
        /// Called when the implementer has been navigated to.
        /// </summary>
        /// <param name="navigationContext">The navigation context containing parameters and other navigation information.</param>
        /// <remarks>
        /// This method is called after the view has been added to the region and is about to be activated.
        /// Use this method to initialize the view based on navigation parameters.
        /// </remarks>
        void OnNavigatedTo(NavigationContext navigationContext);

        /// <summary>
        /// Called to determine if this instance can handle the navigation request.
        /// </summary>
        /// <param name="navigationContext">The navigation context containing parameters and other navigation information.</param>
        /// <returns>
        /// <see langword="true"/> if this instance accepts the navigation request and should be reused; otherwise, <see langword="false"/> to create a new instance.
        /// </returns>
        /// <remarks>
        /// Return <see langword="true"/> if this view should be reused for the navigation (e.g., showing the same item with updated parameters).
        /// Return <see langword="false"/> if a new instance should be created.
        /// </remarks>
        bool IsNavigationTarget(NavigationContext navigationContext);

        /// <summary>
        /// Called when the implementer is being navigated away from.
        /// </summary>
        /// <param name="navigationContext">The navigation context containing parameters and other navigation information.</param>
        /// <remarks>
        /// This method is called when the view is about to be deactivated or removed from the region.
        /// Use this method to clean up resources or save state if needed.
        /// </remarks>
        void OnNavigatedFrom(NavigationContext navigationContext);
    }
}
