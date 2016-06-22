

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

using Prism.Regions;
using Prism.Common;
using System.Threading.Tasks;

namespace Prism.Regions.CallbackWrappers
{
    /// <summary>
    /// Provides journaling of current, back, and forward navigation within regions.
    /// </summary>
    public class RegionNavigationJournalWithCallbacksWrapper : IRegionNavigationJournal
    {
        private IRegionNavigationJournalWithCallbacks _journal;

        public RegionNavigationJournalWithCallbacksWrapper(IRegionNavigationJournalWithCallbacks journal)
        {
            _journal = journal;
        }

        public bool CanGoBack
        {
            get { return _journal.CanGoBack; }
        }

        public bool CanGoForward
        {
            get { return _journal.CanGoForward; }
        }

        public IRegionNavigationJournalEntry CurrentEntry
        {
            get { return _journal.CurrentEntry; }
        }

        public INavigateAsync NavigationTarget
        {
            get { return _journal.NavigationTarget; }
            set { _journal.NavigationTarget = value; }
        }

        public void Clear()
        {
            _journal.Clear();
        }

        public Task<NavigationResult> GoBackAsync()
        {
            _journal.GoBack();
            return Task.FromResult(new NavigationResult(null, true));
        }

        public Task<NavigationResult> GoForwardAsync()
        {
            _journal.GoForward();
            return Task.FromResult(new NavigationResult(null, true));
        }

        public void RecordNavigation(IRegionNavigationJournalEntry entry)
        {
            _journal.RecordNavigation(entry);
        }
    }
}
