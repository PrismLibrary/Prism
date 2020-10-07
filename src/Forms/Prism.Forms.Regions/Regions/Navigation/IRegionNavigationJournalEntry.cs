using System;
using Prism.Navigation;

namespace Prism.Regions.Navigation
{
    /// <summary>
    /// An entry in an IRegionNavigationJournal representing the URI navigated to.
    /// </summary>
    public interface IRegionNavigationJournalEntry
    {
        /// <summary>
        /// Gets or sets the URI.
        /// </summary>
        /// <value>The URI.</value>
        Uri Uri { get; set; }

        /// <summary>
        /// Gets or sets the NavigationParameters instance.
        /// </summary>
        INavigationParameters Parameters { get; set; }
    }
}
