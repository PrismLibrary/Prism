using System;
using System.Globalization;
using Prism.Navigation;

namespace Prism.Navigation.Regions
{
    /// <summary>
    /// An entry in an IRegionNavigationJournal representing the URI navigated to.
    /// </summary>
    public class RegionNavigationJournalEntry : IRegionNavigationJournalEntry
    {
        /// <summary>
        /// Gets or sets the URI.
        /// </summary>
        /// <value>The URI.</value>
        public Uri Uri { get; set; }

        /// <summary>
        /// Gets or sets the NavigationParameters instance.
        /// </summary>
        public INavigationParameters Parameters { get; set; }

        /// <summary>
        /// Returns a <see cref="string"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="string"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            if (this.Uri != null)
            {
                return string.Format(CultureInfo.CurrentCulture, "RegionNavigationJournalEntry:'{0}'", this.Uri.ToString());
            }

            return base.ToString();
        }
    }
}
