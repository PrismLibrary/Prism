namespace Prism.Navigation
{
    /// <summary>
    /// Interface to provide a way to prevent the injection of the MultiPageNavigationBehavior
    /// by the NavigationService for TabbedPages and CarouselPages.
    /// </summary>
    public interface IMultiPageNavigationOptions
    {
        /// <summary>
        /// If this is false it will prevent the injection of the MultiPageNavigationBehavior
        /// </summary>
        bool InjectNavigationBehavior { get; }
    }
}
