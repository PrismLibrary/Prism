using System;
using System.Collections.Generic;
using System.Text;
using Prism.Regions.Navigation;
using Xamarin.Forms;

namespace Prism.Common
{
    internal static class MvvmHelpers
    {
        public static void ViewAndViewModelAction<T>(object view, Action<T> action) where T : class =>
            PageUtilities.InvokeViewAndViewModelAction(view, action);

        public static bool GetImplementerFromViewOrViewModel<T>(object view, out T implementer) where T : class =>
            (implementer = GetImplementerFromViewOrViewModel<T>(view)) != null;

        public static T GetImplementerFromViewOrViewModel<T>(object view) where T : class
        {
            if (view is T viewAsT)
            {
                return viewAsT;
            }

            if (view is VisualElement element)
            {
                var vmAsT = element.BindingContext as T;
                return vmAsT;
            }

            return null;
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
