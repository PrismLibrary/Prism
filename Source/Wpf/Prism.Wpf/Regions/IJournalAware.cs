namespace Prism.Regions
{
    /// <summary>
    /// Provides a way for objects involved in navigation to opt-out of being added to the IRegionNavigationJournal backstack.
    /// </summary>
    public interface IJournalAware
    {
        /// <summary>
        /// Determines if the current obect is going to be added to the navigation journal's backstack.
        /// </summary>
        /// <returns>True, add to backstack. False, remove from backstack.</returns>
        bool PersistInHistory();
    }
}
