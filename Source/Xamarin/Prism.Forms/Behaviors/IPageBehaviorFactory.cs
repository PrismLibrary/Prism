using Xamarin.Forms;

namespace Prism.Behaviors
{
    /// <summary>
    /// Applies behaviors to the Xamarin.Forms pages when they are created during navigation.
    /// </summary>
    public interface IPageBehaviorFactory
    {
        /// <summary>
        /// Applies behaviors to a page based on the page type.
        /// </summary>
        /// <param name="page">The page to apply the behaviors</param>
        /// <remarks>The PageLifeCycleAwareBehavior is applied to all pages</remarks>
        void ApplyPageBehaviors(Page page);

        /// <summary>
        /// Applies behaviors to a CarouselPage.
        /// </summary>
        /// <param name="page">The CarouselPage to apply the behaviors</param>
        /// <remarks>The CarouselPageActiveAwareBehavior is applied by default</remarks>
        void ApplyCarouselPageBehaviors(CarouselPage page);

        /// <summary>
        /// Applies behaviors to a ContentPage.
        /// </summary>
        /// <param name="page">The ContentPage to apply the behaviors</param>
        void ApplyContentPageBehaviors(ContentPage page);

        /// <summary>
        /// Applies behaviors to a MasterDetailPage.
        /// </summary>
        /// <param name="page">The MasterDetailPage to apply the behaviors</param>
        void ApplyMasterDetailPageBehaviors(MasterDetailPage page);

        /// <summary>
        /// Applies behaviors to a NavigationPage.
        /// </summary>
        /// <param name="page">The NavigationPage to apply the behaviors</param>
        /// <remarks>The NavigationPageSystemGoBackBehavior and NavigationPageActiveAwareBehavior are applied by default</remarks>
        void ApplyNavigationPageBehaviors(NavigationPage page);

        /// <summary>
        /// Applies behaviors to a TabbedPage.
        /// </summary>
        /// <param name="page">The TabbedPage to apply the behaviors</param>
        /// <remarks>The TabbedPageActiveAwareBehavior is added by default</remarks>
        void ApplyTabbedPageBehaviors(TabbedPage page);
    }
}
