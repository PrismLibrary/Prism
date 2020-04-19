using System.Linq;
using Xamarin.Forms;

namespace Prism.Regions
{
    /// <summary>
    /// Region that allows a maximum of one active view at a time.
    /// </summary>
    public class SingleActiveRegion : Region
    {
        /// <summary>
        /// Marks the specified view as active.
        /// </summary>
        /// <param name="view">The view to activate.</param>
        /// <remarks>If there is an active view before calling this method,
        /// that view will be deactivated automatically.</remarks>
        public override void Activate(VisualElement view)
        {
            var currentActiveView = ActiveViews.FirstOrDefault();

            if (currentActiveView != null && currentActiveView != view && Views.Contains(currentActiveView))
            {
                base.Deactivate(currentActiveView);
            }

            base.Activate(view);
        }
    }
}
