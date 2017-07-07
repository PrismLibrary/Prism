

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        /// <returns>The navigation result</returns>
        /// <remarks>
        /// If no entry exists in back navigation stack (CanGoBack false), the <see cref="NavigationContext"/> in
        /// returned <see cref="NavigationResult"/> is set to <see langword="null"/>.
        /// </remarks>
        public async Task<NavigationResult> GoBackAsync()
        {
            if (this.CanGoBack)
            {
                IRegionNavigationJournalEntry entry = this.backStack.Peek();

                NavigationResult result = await this.InternalNavigate(entry);

                if (result.Result == true)
                {
                    if (this.CurrentEntry != null)
                    {
                        this.forwardStack.Push(this.CurrentEntry);
                    }

                    this.backStack.Pop();
                    this.CurrentEntry = entry;
                }

                return result;
            }
            else
            {
                return new NavigationResult(null, false);
            }
        }

        /// <summary>
        /// Navigates to the most recent entry in the forward navigation history, or does nothing if no entry exists in forward navigation.
        /// </summary>
        /// <returns>The navigation result</returns>
        /// <remarks>
        /// If no entry exists in forward navigation stack (CanGoForward false), the <see cref="NavigationContext"/> in
        /// returned <see cref="NavigationResult"/> is set to <see langword="null"/>.
        /// </remarks>
        public async Task<NavigationResult> GoForwardAsync()
        {
            if (this.CanGoForward)
            {
                IRegionNavigationJournalEntry entry = this.forwardStack.Peek();

                NavigationResult result = await this.InternalNavigate(entry);

                if (result.Result == true)
                {
                    if (this.CurrentEntry != null)
                    {
                        this.backStack.Push(this.CurrentEntry);
                    }

                    this.forwardStack.Pop();
                    this.CurrentEntry = entry;
                }

                return result;
            }
            else
            {
                return new NavigationResult(null, false);
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

        private async Task<NavigationResult> InternalNavigate(IRegionNavigationJournalEntry entry)
        {
            this.isNavigatingInternal = true;
            NavigationResult result = await this.NavigationTarget.RequestNavigateAsync(entry.Uri, entry.Parameters);
            this.isNavigatingInternal = false;

            return result;
        }
    }
}
