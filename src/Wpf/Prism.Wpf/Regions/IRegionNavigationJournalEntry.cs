

using System;
namespace Prism.Regions
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
        NavigationParameters Parameters { get; set; }
    }
}
