

using System;
using System.Collections.Generic;

namespace Prism.Regions
{
    /// <summary>
    /// Provides journaling of current, back, and forward navigation within regions.    
    /// </summary>
    public class RegionNavigationJournal : IRegionNavigationJournal
    {
        private Stack<IRegionNavigationJournalEntry> backStack = new Stack<IRegionNavigationJournalEntry>();
        private Stack<IRegionNavigationJournalEntry> forwardStack = new Stack<IRegionNavigationJournalEntry>();

        private bool isNavigatingInternal;

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
        public bool CanGoBack
        {
            get
            {
                return this.backStack.Count > 0;
            }
        }

        /// <summary>
        /// Gets a value that indicates whether there is at least one entry in the forward navigation history.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can go forward; otherwise, <c>false</c>.
        /// </value>
        public bool CanGoForward
        {
            get
            {
                return this.forwardStack.Count > 0;
            }
        }

        /// <summary>
        /// Navigates to the most recent entry in the back navigation history, or does nothing if no entry exists in back navigation.
        /// </summary>
        public void GoBack()
        {
            if (this.CanGoBack)
            {
                IRegionNavigationJournalEntry entry = this.backStack.Peek();
                this.InternalNavigate(
                    entry,
                    result =>
                    {
                        if (result)
                        {
                            if (this.CurrentEntry != null)
                            {
                                this.forwardStack.Push(this.CurrentEntry);
                            }

                            this.backStack.Pop();
                            this.CurrentEntry = entry;
                        }
                    });
            }
        }

        /// <summary>
        /// Navigates to the most recent entry in the forward navigation history, or does nothing if no entry exists in forward navigation.
        /// </summary>
        public void GoForward()
        {
            if (this.CanGoForward)
            {
                IRegionNavigationJournalEntry entry = this.forwardStack.Peek();
                this.InternalNavigate(
                    entry,
                    result =>
                    {
                        if (result)
                        {
                            if (this.CurrentEntry != null)
                            {
                                this.backStack.Push(this.CurrentEntry);
                            }

                            this.forwardStack.Pop();
                            this.CurrentEntry = entry;
                        }
                    });
            }
        }

        /// <summary>
        /// Records the navigation to the entry..
        /// </summary>
        /// <param name="entry">The entry to record.</param>
        public void RecordNavigation(IRegionNavigationJournalEntry entry)
        {
            if (!this.isNavigatingInternal)
            {
                if (this.CurrentEntry != null)
                {
                    this.backStack.Push(this.CurrentEntry);
                }

                this.forwardStack.Clear();
                this.CurrentEntry = entry;
            }
        }

        /// <summary>
        /// Clears the journal of current, back, and forward navigation histories.
        /// </summary>
        public void Clear()
        {
            this.CurrentEntry = null;
            this.backStack.Clear();
            this.forwardStack.Clear();
        }

        private void InternalNavigate(IRegionNavigationJournalEntry entry, Action<bool> callback)
        {
            this.isNavigatingInternal = true;
            this.NavigationTarget.RequestNavigate(
                entry.Uri,
                nr =>
                {
                    this.isNavigatingInternal = false;

                    if (nr.Result.HasValue)
                    {
                        callback(nr.Result.Value);
                    }
                },
                entry.Parameters);
        }
    }
}
