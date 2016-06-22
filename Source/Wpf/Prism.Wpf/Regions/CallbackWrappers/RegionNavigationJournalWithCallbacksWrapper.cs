
using System.Threading.Tasks;

using Prism.Regions;

namespace Prism.Regions.CallbackWrappers
{
    /// <summary>
    /// Creates an instance of <see cref="IRegionNavigationJournal"/> based on an instance of <see cref="IRegionNavigationJournalWithCallbacks"/>
    /// </summary>
    /// <seealso cref="Prism.Regions.IRegionNavigationJournal" />
    public class RegionNavigationJournalWithCallbacksWrapper : IRegionNavigationJournal
    {
        private IRegionNavigationJournalWithCallbacks _journal;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegionNavigationJournalWithCallbacksWrapper"/> class.
        /// </summary>
        /// <param name="journal">The underlying journal.</param>
        public RegionNavigationJournalWithCallbacksWrapper(IRegionNavigationJournalWithCallbacks journal)
        {
            _journal = journal;
        }

        /// <summary>
        /// Gets a value that indicates whether there is at least one entry in the back navigation history.
        /// </summary>
        /// <value>
        /// <c>true</c> if the journal can go back; otherwise, <c>false</c>.
        /// </value>
        public bool CanGoBack
        {
            get { return _journal.CanGoBack; }
        }

        /// <summary>
        /// Gets a value that indicates whether there is at least one entry in the forward navigation history.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can go forward; otherwise, <c>false</c>.
        /// </value>
        public bool CanGoForward
        {
            get { return _journal.CanGoForward; }
        }

        /// <summary>
        /// Gets the current navigation entry of the content that is currently displayed.
        /// </summary>
        /// <value>The current entry.</value>
        public IRegionNavigationJournalEntry CurrentEntry
        {
            get { return _journal.CurrentEntry; }
        }

        /// <summary>
        /// Gets or sets the target that implements INavigateAsync.
        /// </summary>
        /// <value>The INavigate implementation.</value>
        /// <remarks>
        /// This is set by the owner of this journal.
        /// </remarks>
        public INavigateAsync NavigationTarget
        {
            get { return _journal.NavigationTarget; }
            set { _journal.NavigationTarget = value; }
        }

        /// <summary>
        /// Clears the journal of current, back, and forward navigation histories.
        /// </summary>
        public void Clear()
        {
            _journal.Clear();
        }

        /// <summary>
        /// Navigates to the most recent entry in the back navigation history, or does nothing if no entry exists in back navigation.
        /// </summary>
        /// <returns>The navigation result</returns>
        /// <remarks>
        /// If no entry exists in back navigation stack (CanGoBack false), the <see cref="NavigationContext"/> in
        /// returned <see cref="NavigationResult"/> is set to <see langword="null"/>.
        /// </remarks>
        public Task<NavigationResult> GoBackAsync()
        {
            _journal.GoBack();
            return Task.FromResult(new NavigationResult(null, true));
        }

        /// <summary>
        /// Navigates to the most recent entry in the forward navigation history, or does nothing if no entry exists in forward navigation.
        /// </summary>
        /// <returns>The navigation result</returns>
        /// <remarks>
        /// If no entry exists in forward navigation stack (CanGoForward false), the <see cref="NavigationContext"/> in
        /// returned <see cref="NavigationResult"/> is set to <see langword="null"/>.
        /// </remarks>
        public Task<NavigationResult> GoForwardAsync()
        {
            _journal.GoForward();
            return Task.FromResult(new NavigationResult(null, true));
        }

        /// <summary>
        /// Records the navigation to the entry..
        /// </summary>
        /// <param name="entry">The entry to record.</param>
        public void RecordNavigation(IRegionNavigationJournalEntry entry)
        {
            _journal.RecordNavigation(entry);
        }
    }
}
