using System;
using System.Collections.Generic;
using System.Text;
using Prism.Regions.Navigation;
using Xamarin.Forms;

namespace Prism.Common
{
    internal static class MvvmHelpers
    {
        public static void ViewAndViewModelAction<T>(object view, Action<T> action)
            where T : class
        {
            var stack = new System.Diagnostics.StackTrace();
            if (view is T viewAsT)
                action(viewAsT);

            if (view is BindableObject bindable && bindable.BindingContext is T vmAsT)
                action(vmAsT);
        }

        public static bool GetImplementerFromViewOrViewModel<T>(object view, out T implementer)
            where T : class =>
            (implementer = GetImplementerFromViewOrViewModel<T>(view)) != null;

        public static T GetImplementerFromViewOrViewModel<T>(object view)
            where T : class
        {
            if (view is T viewAsT)
            {
                return viewAsT;
            }

            if (view is VisualElement element && element.BindingContext is T vmAsT)
            {
                return vmAsT;
            }

            return null;
        }

        public static bool IsNavigationTarget(object view, INavigationContext navigationContext)
        {
            bool isViewTarget = false;
            bool isVMTarget = false;

            if (view is IRegionAware viewAsRegionAware)
                isViewTarget = viewAsRegionAware.IsNavigationTarget(navigationContext);

            if (view is BindableObject bindable && bindable.BindingContext is IRegionAware vmAsRegionAware)
                isVMTarget = vmAsRegionAware.IsNavigationTarget(navigationContext);

            return isViewTarget || isVMTarget;
        }

        public static void OnNavigatedFrom(object view, INavigationContext navigationContext)
        {
            ViewAndViewModelAction<IRegionAware>(view, x => x.OnNavigatedFrom(navigationContext));
        }

        public static void OnNavigatedTo(object view, INavigationContext navigationContext)
        {
            ViewAndViewModelAction<IRegionAware>(view, x => x.OnNavigatedTo(navigationContext));
        }
    }
}
