

using System;

namespace Prism.Regions
{
    /// <summary>
    /// Provides navigation for regions.
    /// </summary>
    public interface IRegionNavigationService : INavigateAsync
    {
        /// <summary>
        /// Gets or sets the region owning this service.
        /// </summary>
        /// <value>A Region.</value>
        IRegion Region { get; set; }

        /// <summary>
        /// Gets the journal.
        /// </summary>
        /// <value>The journal.</value>
        IRegionNavigationJournal Journal { get; }

        /// <summary>
        /// Raised when the region is about to be navigated to content.
        /// </summary>
        event EventHandler<RegionNavigationEventArgs> Navigating;

        /// <summary>
        /// Raised when the region is navigated to content.
        /// </summary>
        event EventHandler<RegionNavigationEventArgs> Navigated;

        /// <summary>
        /// Raised when a navigation request fails.
        /// </summary>
        event EventHandler<RegionNavigationFailedEventArgs> NavigationFailed;
    }
}
