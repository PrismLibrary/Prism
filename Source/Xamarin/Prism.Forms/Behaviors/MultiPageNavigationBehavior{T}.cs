using System;
using Prism.Navigation;
using Xamarin.Forms;

namespace Prism.Behaviors
{
    public class MultiPageNavigationBehavior<T> : BehaviorBase<MultiPage<T>> where T : Page
    {
        private Page _lastSelectedPage;

        protected override void OnAttachedTo(MultiPage<T> bindable)
        {
            base.OnAttachedTo(bindable);
            bindable.CurrentPageChanged += CurrentPageChangedHandler;
        }

        protected override void OnDetachingFrom(MultiPage<T> bindable)
        {
            base.OnDetachingFrom(bindable);
            bindable.CurrentPageChanged -= CurrentPageChangedHandler;
        }

        private void CurrentPageChangedHandler(object sender, EventArgs args)
        {
            NavigationParameters parameters = new NavigationParameters();

            var lastPageContextAware = _lastSelectedPage?.BindingContext as IMultiPageNavigationAware;
            lastPageContextAware?.OnInternalNavigatedFrom(parameters);

            var newPageContextAware = AssociatedObject.CurrentPage.BindingContext as IMultiPageNavigationAware;
            newPageContextAware?.OnInternalNavigatedTo(parameters);

            _lastSelectedPage = AssociatedObject.CurrentPage;
        }
    }
}

