using System;
using Prism.AppModel;
using Prism.Common;
using Xamarin.Forms;

namespace Prism.Behaviors
{
    public class PageLifeCycleAwareBehavior : BehaviorBase<Page>
    {
        protected override void OnAttachedTo(Page bindable)
        {
            base.OnAttachedTo(bindable);
            bindable.Appearing += OnAppearing;
            bindable.Disappearing += OnDisappearing;
        }

        protected override void OnDetachingFrom(Page bindable)
        {
            base.OnDetachingFrom(bindable);
            bindable.Appearing -= OnAppearing;
            bindable.Disappearing -= OnDisappearing;
        }

        private void OnAppearing(object sender, EventArgs e)
        {
            PageUtilities.InvokeViewAndViewModelAction<IPageLifecycleAware>(AssociatedObject, aware => aware.OnAppearing());
        }

        private void OnDisappearing(object sender, EventArgs e)
        {
            PageUtilities.InvokeViewAndViewModelAction<IPageLifecycleAware>(AssociatedObject, aware => aware.OnDisappearing());
        }
    }
}
