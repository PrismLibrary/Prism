using Prism.Common;
using Prism.Navigation;
using System;
using Xamarin.Forms;

namespace Prism.Behaviors
{
    public class MultiPageActiveAwareBehavior<T> : BehaviorBase<MultiPage<T>> where T : Page
    {
        protected T _lastSelectedPage;

        /// <inheritDoc/>
        protected override void OnAttachedTo(MultiPage<T> bindable)
        {
            bindable.CurrentPageChanged += CurrentPageChangedHandler;
            bindable.Appearing += RootPageAppearingHandler;
            bindable.Disappearing += RootPageDisappearingHandler;
            base.OnAttachedTo(bindable);
        }

        /// <inheritDoc/>
        protected override void OnDetachingFrom(MultiPage<T> bindable)
        {
            bindable.CurrentPageChanged -= CurrentPageChangedHandler;
            bindable.Appearing -= RootPageAppearingHandler;
            bindable.Disappearing -= RootPageDisappearingHandler;
            base.OnDetachingFrom(bindable);
        }

        /// <summary>
        /// Event Handler for the MultiPage CurrentPageChanged event
        /// </summary>
        /// <param name="sender">The MultiPage</param>
        /// <param name="e">Event Args</param>
        protected void CurrentPageChangedHandler(object sender, EventArgs e)
        {
            if (_lastSelectedPage == null)
                _lastSelectedPage = AssociatedObject.CurrentPage;

            //inactive 
            SetIsActive(_lastSelectedPage, false);

            _lastSelectedPage = AssociatedObject.CurrentPage;

            //active
            SetIsActive(_lastSelectedPage, true);
        }

        /// <summary>
        /// Event Handler for the MultiPage Appearing event
        /// </summary>
        /// <param name="sender">The MultiPage Appearing</param>
        /// <param name="e">Event Args</param>
        protected void RootPageAppearingHandler(object sender, EventArgs e)
        {
            if (_lastSelectedPage == null)
                _lastSelectedPage = AssociatedObject.CurrentPage;

            SetIsActive(_lastSelectedPage, true);
        }

        /// <summary>
        /// Event Handler for the MultiPage Disappearing event
        /// </summary>
        /// <param name="sender">The MultiPage Disappearing</param>
        /// <param name="e">Event Args</param>
        protected void RootPageDisappearingHandler(object sender, EventArgs e)
        {
            SetIsActive(_lastSelectedPage, false);
        }

        void SetIsActive(object view, bool isActive)
        {
            var pageToSetIsActive = view is NavigationPage ? ((NavigationPage)view).CurrentPage : view;

            PageUtilities.InvokeViewAndViewModelAction<IActiveAware>(pageToSetIsActive, activeAware => activeAware.IsActive = isActive);
        }
    }
}
