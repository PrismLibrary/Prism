using System;
using System.Collections.Generic;

namespace Prism.Regions.Navigation
{
    /// <summary>
    /// Provides journaling of current, back, and forward navigation within regions.    
    /// </summary>
    public class RegionNavigationJournal : IRegionNavigationJournal
    {
        private readonly Stack<IRegionNavigationJournalEntry> _backStack = new Stack<IRegionNavigationJournalEntry>();
        private readonly Stack<IRegionNavigationJournalEntry> _forwardStack = new Stack<IRegionNavigationJournalEntry>();

        private bool _isNavigatingInternal;

        /// <summary>
        /// Gets or sets the target that implements INavigate.
        /// </summary>
        /// <value>The INavigate implementation.</value>
        /// <remarks>
        /// This is set by the owner of this journal.
        /// </remarks>
        public INavigateAsync NavigationTarget { get; set; }

        /// <summary>
        /// Gets the current navigation entry of the content that is currently displayed.
        /// </summary>
        /// <value>The current entry.</value>
        public IRegionNavigationJournalEntry CurrentEntry { get; private set; }

        /// <summary>
        /// Gets a value that indicates whether there is at least one entry in the back navigation history.
        /// </summary>
        /// <value><c>true</c> if the journal can go back; otherwise, <c>false</c>.</value>
        public bool CanGoBack => _backStack.Count > 0;

        /// <summary>
        /// Gets a value that indicates whether there is at least one entry in the forward navigation history.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance can go forward; otherwise, <c>false</c>.
        /// </value>
        public bool CanGoForward => _forwardStack.Count > 0;

        /// <summary>
        /// Navigates to the most recent entry in the back navigation history, or does nothing if no entry exists in back navigation.
        /// </summary>
        public void GoBack()
        {
            if (CanGoBack)
            {
                IRegionNavigationJournalEntry entry = _backStack.Peek();
                InternalNavigate(
                    entry,
                    result =>
                    {
                        if (result)
                        {
                            if (CurrentEntry != null)
                            {
                                _forwardStack.Push(CurrentEntry);
                            }

                            _backStack.Pop();
                            CurrentEntry = entry;
                        }
                    });
            }
        }

        /// <summary>
        /// Navigates to the most recent entry in the forward navigation history, or does nothing if no entry exists in forward navigation.
        /// </summary>
        public void GoForward()
        {
            if (CanGoForward)
            {
                IRegionNavigationJournalEntry entry = _forwardStack.Peek();
                InternalNavigate(
                    entry,
                    result =>
                    {
                        if (result)
                        {
                            if (CurrentEntry != null)
                            {
                                _backStack.Push(CurrentEntry);
                            }

                            _forwardStack.Pop();
                            CurrentEntry = entry;
                        }
                    });
            }
        }

        /// <summary>
        /// Records the navigation to the entry..
        /// </summary>
        /// <param name="entry">The entry to record.</param>
        /// <param name="persistInHistory">Determine if the view is added to the back stack or excluded from the history.</param>
        public void RecordNavigation(IRegionNavigationJournalEntry entry, bool persistInHistory)
        {
            if (!_isNavigatingInternal)
            {
                if (CurrentEntry != null)
                {
                    _backStack.Push(CurrentEntry);
                }

                _forwardStack.Clear();

                if (persistInHistory)
                    CurrentEntry = entry;
                else
                    CurrentEntry = null;
            }
        }

        /// <summary>
        /// Clears the journal of current, back, and forward navigation histories.
        /// </summary>
        public void Clear()
        {
            CurrentEntry = null;
            _backStack.Clear();
            _forwardStack.Clear();
        }

        private void InternalNavigate(IRegionNavigationJournalEntry entry, Action<bool> callback)
        {
            _isNavigatingInternal = true;
            NavigationTarget.RequestNavigate(
                entry.Uri,
                nr =>
                {
                    _isNavigatingInternal = false;

                    if (nr.Result.HasValue)
                    {
                        callback(nr.Result.Value);
                    }
                },
                entry.Parameters);
        }
    }
}
