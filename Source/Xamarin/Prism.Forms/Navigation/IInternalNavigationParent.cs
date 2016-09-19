namespace Prism.Navigation
{
    /// <summary>
    /// Interface for sharing NavigationParameters between a MultiPage and it's Children
    /// </summary>
    public interface IInternalNavigationParent
    {
        /// <summary>
        /// Provides NavigationParameters to be shared with Children
        /// </summary>
        NavigationParameters SharedParameters { get; set; }
    }
}
