using Xamarin.Forms;

namespace Prism.Behaviors
{
    /// <summary>
    /// Applies behaviors to the Xamarin.Forms pages when they are created during navigation.
    /// </summary>
    public class PageBehaviorFactory : IPageBehaviorFactory
    {
        /// <summary>
        /// Applies behaviors to a CarouselPage.
        /// </summary>
        /// <param name="page">The CarouselPage to apply the behaviors</param>
        /// <remarks>The CarouselPageActiveAwareBehavior is applied by default</remarks>
        public virtual void ApplyCarouselPageBehaviors(CarouselPage page)
        {
            page.Behaviors.Add(new CarouselPageActiveAwareBehavior());
        }

        /// <summary>
        /// Applies behaviors to a ContentPage.
        /// </summary>
        /// <param name="page">The ContentPage to apply the behaviors</param>
        public virtual void ApplyContentPageBehaviors(ContentPage page)
        {
            
        }

        /// <summary>
        /// Applies behaviors to a MasterDetailPage.
        /// </summary>
        /// <param name="page">The MasterDetailPage to apply the behaviors</param>
        public virtual void ApplyMasterDetailPageBehaviors(MasterDetailPage page)
        {
            
        }

        /// <summary>
        /// Applies behaviors to a NavigationPage.
        /// </summary>
        /// <param name="page">The NavigationPage to apply the behaviors</param>
        /// <remarks>The NavigationPageSystemGoBackBehavior and NavigationPageActiveAwareBehavior are applied by default</remarks>
        public virtual void ApplyNavigationPageBehaviors(NavigationPage page)
        {
            page.Behaviors.Add(new NavigationPageSystemGoBackBehavior());
            page.Behaviors.Add(new NavigationPageActiveAwareBehavior());
        }

        /// <summary>
        /// Applies behaviors to a page based on the page type.
        /// </summary>
        /// <param name="page">The page to apply the behaviors</param>
        /// <remarks>The PageLifeCycleAwareBehavior is applied to all pages</remarks>
        public virtual void ApplyPageBehaviors(Page page)
        {
            switch (page)
            {
                case ContentPage contentPage:
                    ApplyContentPageBehaviors(contentPage);
                    break;
                case NavigationPage navPage:
                    ApplyNavigationPageBehaviors(navPage);
                    break;
                case MasterDetailPage masterDetailPage:
                    ApplyMasterDetailPageBehaviors(masterDetailPage);
                    break;
                case TabbedPage tabbedPage:
                    ApplyTabbedPageBehaviors(tabbedPage);
                    break;
                case CarouselPage carouselPage:
                    ApplyCarouselPageBehaviors(carouselPage);
                    break;
            }

            page.Behaviors.Add(new PageLifeCycleAwareBehavior());
        }

        /// <summary>
        /// Applies behaviors to a TabbedPage.
        /// </summary>
        /// <param name="page">The TabbedPage to apply the behaviors</param>
        /// <remarks>The TabbedPageActiveAwareBehavior is added by default</remarks>
        public virtual void ApplyTabbedPageBehaviors(TabbedPage page)
        {
            page.Behaviors.Add(new TabbedPageActiveAwareBehavior());
        }
    }
}
