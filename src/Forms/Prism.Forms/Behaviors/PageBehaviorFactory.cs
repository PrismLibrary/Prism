﻿using Xamarin.Forms;

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
        protected virtual void ApplyCarouselPageBehaviors(CarouselPage page)
        {
            page.Behaviors.Add(new CarouselPageActiveAwareBehavior());
        }

        /// <summary>
        /// Applies behaviors to a ContentPage.
        /// </summary>
        /// <param name="page">The ContentPage to apply the behaviors</param>
        protected virtual void ApplyContentPageBehaviors(ContentPage page)
        {

        }

        /// <summary>
        /// Applies behaviors to a MasterDetailPage.
        /// </summary>
        /// <param name="page">The MasterDetailPage to apply the behaviors</param>
        protected virtual void ApplyMasterDetailPageBehaviors(FlyoutPage page)
        {

        }

        /// <summary>
        /// Applies behaviors to a NavigationPage.
        /// </summary>
        /// <param name="page">The NavigationPage to apply the behaviors</param>
        /// <remarks>The NavigationPageSystemGoBackBehavior and NavigationPageActiveAwareBehavior are applied by default</remarks>
        protected virtual void ApplyNavigationPageBehaviors(NavigationPage page)
        {
            page.Behaviors.Add(new NavigationPageSystemGoBackBehavior());
            page.Behaviors.Add(new NavigationPageActiveAwareBehavior());
        }

        /// <summary>
        /// Applies behaviors to a page based on the page type.
        /// </summary>
        /// <param name="page">The page to apply the behaviors</param>
        /// <remarks>
        /// There is no need to call base.ApplyPageBehaviors when overriding.
        /// All Prism behaviors have already been applied.
        /// </remarks>
        protected virtual void ApplyPageBehaviors(Page page)
        {
        }

        void IPageBehaviorFactory.ApplyPageBehaviors(Page page)
        {
            switch (page)
            {
                case ContentPage contentPage:
                    ApplyContentPageBehaviors(contentPage);
                    break;
                case NavigationPage navPage:
                    ApplyNavigationPageBehaviors(navPage);
                    break;
                case FlyoutPage masterDetailPage:
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
            page.Behaviors.Add(new PageScopeBehavior());
            ApplyPageBehaviors(page);
        }

        /// <summary>
        /// Applies behaviors to a TabbedPage.
        /// </summary>
        /// <param name="page">The TabbedPage to apply the behaviors</param>
        /// <remarks>The TabbedPageActiveAwareBehavior is added by default</remarks>
        protected virtual void ApplyTabbedPageBehaviors(TabbedPage page)
        {
            page.Behaviors.Add(new TabbedPageActiveAwareBehavior());
        }
    }
}
