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
    }
}
