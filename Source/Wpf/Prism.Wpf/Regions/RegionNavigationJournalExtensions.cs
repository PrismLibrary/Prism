using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prism.Regions
{
    public static class RegionNavigationJournalExtensions
    {
        /// <summary>
        /// Navigates to the most recent entry in the back navigation history, or does nothing if no entry exists in back navigation.
        /// </summary>
        public static void GoBack(this IRegionNavigationJournal journal)
        {
            var task = journal.GoBackAsync();
        }

        /// <summary>
        /// Navigates to the most recent entry in the forward navigation history, or does nothing if no entry exists in forward navigation.
        /// </summary>
        public static void GoForward(this IRegionNavigationJournal journal)
        {
            var task = journal.GoForwardAsync();
        }
    }
}
