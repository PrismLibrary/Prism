using System;
using Prism.Regions.Navigation;
using Xamarin.Forms;

namespace Prism.Common
{
    internal static class MvvmHelpers
    {
        public static void AutowireViewModel(object view) => PageUtilities.SetAutowireViewModel((VisualElement)view);

        public static void ViewAndViewModelAction<T>(object view, Action<T> action)
            where T : class
        {
            if (view is T viewAsT)
                action(viewAsT);

            if (view is BindableObject bindable && bindable.BindingContext is T vmAsT)
                action(vmAsT);
        }

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
            if (view is IRegionAware viewAsRegionAware)
            {
                return viewAsRegionAware.IsNavigationTarget(navigationContext);
            }

            if (view is BindableObject bindable && bindable.BindingContext is IRegionAware vmAsRegionAware)
            {
                return vmAsRegionAware.IsNavigationTarget(navigationContext);
            }

            var uri = navigationContext.Uri;
            if (!uri.IsAbsoluteUri)
                uri = new Uri(new Uri("app://prism.regions"), uri);
            var path = uri.LocalPath.Substring(1);
            var viewType = view.GetType();

            return path == viewType.Name || path == viewType.FullName;
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
